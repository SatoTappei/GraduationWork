using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using VTNConnect;

namespace Game
{
    public class Adventurer : Actor
    {
        AdventurerSheet _sheet;
        Status _status;
        ActionLog _actionLog;
        ExploreRecord _exploreRecord;
        Vector2Int _coords;
        Vector2Int _direction;
        string _selectedAction;
        bool _isInitialized;
        bool _isCompleted;

        GamePlay _gamePlay;
        RolePlay _rolePlay;
        
        StatusBarBinder _statusBar;
        ProfileWindowBinder _profileWindow;
        CameraFocusBinder _camera;
        NameTagBinder _nameTag;
        
        EntryAction _entry;
        DirectionMoveAction _directionMove;
        TargetMoveAction _targetMove;
        AttackAction _attack;
        TalkAction _talk;
        ScavengeAction _scavenge;
        HelpAction _help;
        ThrowAction _throw;
        IdleAction _idle;
        DefeatedAction _defeated;
        EscapeAction _escape;

        PreActionEvaluator _preEvaluator;
        PostActionEvaluator _postEvaluator;
        SubGoalPath _subGoal;
        HoldInformation _information;
        ItemInventory _item;
        HungryStatusEffect _hungry;
        BuffStatusEffect _buff;
        DamageReceiver _damage;
        StatusEffect[] _statusEffects;

        CheerCommentEvent _cheerComment;

        public AdventurerSheet Sheet => _sheet;
        public Status Status => _status;
        public ActionLog ActionLog => _actionLog;
        public ExploreRecord ExploreRecord => _exploreRecord;
        public override Vector2Int Coords => _coords;
        public override Vector2Int Direction => _direction;
        public string SelectedAction => _selectedAction;
        public bool IsCompleted => _isCompleted;

        void Awake()
        {
            _actionLog = new ActionLog();
            _exploreRecord = new ExploreRecord();
            
            _gamePlay = GetComponent<GamePlay>();
            _rolePlay = GetComponent<RolePlay>();
            _statusBar = GetComponent<StatusBarBinder>();
            _profileWindow = GetComponent<ProfileWindowBinder>();
            _camera = GetComponent<CameraFocusBinder>();
            _nameTag = GetComponent<NameTagBinder>();
            _entry = GetComponent<EntryAction>();
            _directionMove = GetComponent<DirectionMoveAction>();
            _targetMove = GetComponent<TargetMoveAction>();
            _attack = GetComponent<AttackAction>();
            _talk = GetComponent<TalkAction>();
            _scavenge = GetComponent<ScavengeAction>();
            _help = GetComponent<HelpAction>();
            _throw = GetComponent<ThrowAction>();
            _idle = GetComponent<IdleAction>();
            _defeated = GetComponent<DefeatedAction>();
            _escape = GetComponent<EscapeAction>();
            _preEvaluator = GetComponent<PreActionEvaluator>();
            _postEvaluator = GetComponent<PostActionEvaluator>();
            _subGoal = GetComponent<SubGoalPath>();
            _information = GetComponent<HoldInformation>();
            _item = GetComponent<ItemInventory>();
            _hungry = GetComponent<HungryStatusEffect>();
            _buff = GetComponent<BuffStatusEffect>();
            _damage = GetComponent<DamageReceiver>();
            _statusEffects = GetComponents<StatusEffect>();

            _cheerComment = CheerCommentEvent.Find();
        }

        void Start()
        {
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        void OnDrawGizmosSelected()
        {
            if (Application.isPlaying)
            {
                // 方向を描画。
                Cell cell = DungeonManager.GetCell(Coords + Direction);
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(cell.Position, 0.33f);
            }
        }

        public void Initialize(AdventurerSheet adventurerSheet)
        {
            _sheet = adventurerSheet;
            _status = new Status(adventurerSheet.Level);

            // ダンジョンの入り口が固定で1箇所。
            _coords = new Vector2Int(11, 8);
            // 上以外の向きの場合、回転させる処理が必要。
            _direction = Vector2Int.up;

            _isInitialized = true;
        }

        // AIの挙動がおかしい場合に呼び出す。
        // とりあえずここに書いているが、なんか良い感じのクラスがあればそっちに移す。
        public void Reboot()
        {
            _actionLog.Delete();
            _exploreRecord.Delete();
            _information.RequestDelete();
            _gamePlay.PreInitialize();
        }

        async UniTask UpdateAsync(CancellationToken token)
        {
            // 初期化が完了するまで待つ。
            await UniTask.WaitUntil(() => _isInitialized, cancellationToken: token);

            // 初回のリクエスト前に、冒険者のデータを使って初期化する必要があるAIを初期化。
            _gamePlay.PreInitialize();
            _rolePlay.Initialize();

            // 生成したセル上に自身を移動と追加。
            DungeonManager.AddActor(Coords, this);
            transform.position = DungeonManager.GetCell(Coords).Position;

            // 各UIに自身を登録。
            _statusBar.Register();
            _profileWindow.Register();
            _camera.Register();
            _nameTag.Register();

            // 登場時の演出。台詞を表示させるので、UIに自身を登録した後に呼ぶ。
            _entry.Play();

            // 変化した心情をUIに反映。
            _statusBar.Apply();

            // サブゴールを決める。
            _subGoal.Initialize(await SubGoalSelector.SelectAsync(Sheet, token));

            // サブゴールをエピソードとして送信。
            GameEpisode episode = new GameEpisode(
                EpisodeCode.VCMainPurpose,
                Sheet.UserId
            );
            episode.SetEpisode(_subGoal.Path[0].Description.Japanese);
            VantanConnect.SendEpisode(episode);

            // アーティファクト所持者の場合は持ち物に追加。
            if (Sheet.IsArtifactOwner)
            {
                _item.Add(new ItemData.Artifact());
            }
            
            // ここまでが1ターン目開始までの処理。以降の処理は毎ターン繰り返される。
            while (!token.IsCancellationRequested)
            {
                // 自身へのコメントを表示し、心情の変化によって演出が発生。
                int emotion = _cheerComment.Display(Sheet.DisplayName, Sheet.DisplayID);
                if (emotion > 0)
                {
                    // バフ量を適当に設定。基準となる値に倍率をかける。
                    _buff.Set("Attack", 1.2f);
                    _buff.Set("Speed", 2.0f);
                }
                else if(emotion < 0)
                {
                    _damage.Damage(0, default, "Madness");
                }

                // 経過ターン数、空腹を増加。
                _status.ElapsedTurn++;
                _status.CurrentHunger++;

                // 空腹が最大の場合、空腹のステータス効果を付与。
                if (_status.IsHungry)
                {
                    if (!_hungry.IsValid)
                    {
                        GameLog.Add(
                            "システム",
                            $"お腹がすいてきた。", 
                            LogColor.White,
                            _sheet.DisplayID
                        );
                    }

                    _hungry.Set();
                }
                else
                {
                    _hungry.Remove();
                }

                // ステータスの変化をUIに反映。
                _statusBar.Apply();

                // 現在いるセルについて、地形の特徴に関する情報がある場合、
                // AIが次の行動を選択する際に、考慮する情報として追加する。
                if (TerrainNavigator.TryGet(Coords, out IReadOnlyList<Information> features))
                {
                    // プレイする度、行動にバラつきを持たせるため、複数ある場合はランダムで1つ選ぶ。
                    int random = Random.Range(0, features.Count);
                    _information.AddPending(features[random]);
                }

                // 周囲の状況やゴール、ステータスを調べ、それぞれの選択肢にスコア付け。
                _preEvaluator.Evaluate();

                // 保持している情報を更新。
                // 新しい情報を知った場合、このタイミングで保持している情報に追加される。
                await _information.UpdateAsync(token);

                // 保持している情報を更新したのでUIに反映。
                _profileWindow.Apply();

                // AIが次の行動を選択し、実行。
                string selectedAction = await _gamePlay.RequestAsync(token);
                ActionResult actionResult = null;
                if (selectedAction == "MoveNorth")
                {
                    actionResult = await _directionMove.PlayAsync(Vector2Int.up, token);
                }
                else if (selectedAction == "MoveSouth")
                {
                    actionResult = await _directionMove.PlayAsync(Vector2Int.down, token);
                }
                else if (selectedAction == "MoveEast")
                {
                    actionResult = await _directionMove.PlayAsync(Vector2Int.right, token);
                }
                else if (selectedAction == "MoveWest")
                {
                    actionResult = await _directionMove.PlayAsync(Vector2Int.left, token);
                }
                else if (selectedAction == "MoveToEntrance")
                {
                    actionResult = await _targetMove.PlayAsync("Entrance", token);
                }
                else if (selectedAction == "MoveToArtifact")
                {
                    actionResult = await _targetMove.PlayAsync("Artifact", token);
                }
                else if (selectedAction == "AttackToEnemy")
                {
                    actionResult = await _attack.PlayAsync<Enemy>(token);
                }
                else if (selectedAction == "AttackToAdventurer")
                {
                    actionResult = await _attack.PlayAsync<Adventurer>(token);
                }
                else if (selectedAction == "TalkWithAdventurer")
                {
                    actionResult = await _talk.PlayAsync(token);
                }
                else if (selectedAction == "Scavenge")
                {
                    actionResult = await _scavenge.PlayAsync(token);
                }
                else if (selectedAction == "RequestHelp")
                {
                    actionResult = await _help.PlayAsync(token);
                }
                else if (selectedAction == "ThrowItem")
                {
                    actionResult = await _throw.PlayAsync(token);
                }
                else if (selectedAction == "Idle")
                {
                    actionResult = await _idle.PlayAsync(token);
                }
                else
                {
                    Debug.LogWarning($"行動を選ぶAIの出力が、指定した形式と違う。: {selectedAction}");
                    await UniTask.Yield(cancellationToken: token);
                }

                // 行動結果を反映。
                if (actionResult != null)
                {
                    // 行動結果を基に、それぞれの選択肢をスコア付け。
                    _postEvaluator.Evaluate(actionResult);

                    // 座標の更新。
                    DungeonManager.RemoveActor(Coords, this);
                    _coords = actionResult.Coords;
                    DungeonManager.AddActor(_coords, this);

                    // 方向を更新。
                    _direction = actionResult.Direction;

                    // 行動ログへ追加。
                    _actionLog.Add($"Turn{_status.ElapsedTurn}: {actionResult.Log}");

                    // 探索したセルがある場合。
                    if (actionResult.Explored != null)
                    {
                        _exploreRecord.Increase((Vector2Int)actionResult.Explored);
                    }
                }

                // インスペクター上で確認できるよう、適当なメンバ変数に反映させておく。
                _selectedAction = selectedAction;

                // 足元に罠等がある場合に起動。
                // 操作と同時にセルから削除される可能性があるので、別のリスト内にコピーした後に実行する。
                List<IFootTriggerable> copy = new List<IFootTriggerable>();
                foreach (Actor actor in DungeonManager.GetActors(Coords))
                {
                    if (actor is IFootTriggerable e) copy.Add(e);
                }
                copy.ForEach(t => t.Interact(this));

                // ステータス効果を反映、UIに反映。
                foreach (StatusEffect e in _statusEffects) e.Apply();
                _statusBar.Apply();

                // 撃破されたもしくは脱出した場合。
                if (await _defeated.PlayAsync(token) || await _escape.PlayAsync(token)) break;

                // サブゴールを達成した場合
                if (_subGoal.IsAchieve())
                {
                    // 結果をチェックしてログに表示。
                    if (_subGoal.GetCurrent().Check() == SubGoal.State.Completed)
                    {
                        GameLog.Add(
                            $"システム",
                            $"「{_subGoal.GetCurrent().Description.Japanese}」を達成。",
                            LogColor.White,
                            _sheet.DisplayID
                        );
                    }
                    else if (_subGoal.GetCurrent().Check() == SubGoal.State.Failed)
                    {
                        GameLog.Add(
                            $"システム",
                            $"「{_subGoal.GetCurrent().Description.Japanese}」を諦めた。",
                            LogColor.White,
                            _sheet.DisplayID
                        );
                    }
                    else
                    {
                        Debug.LogWarning("サブゴール達成時の結果が達成もしくは失敗以外になっている。");
                    }

                    // 次のサブゴールを設定
                    _subGoal.SetNext();
                }

                await UniTask.Yield(cancellationToken: token);
            }

            // 冒険完了フラグを立てる。
            _isCompleted = true;

            // 冒険結果を送信。
            GameManager.SetAdventureResult(
                Sheet.UserId, 
                Status.CurrentHp > 0, 
                _subGoal.Path[0].Check() == SubGoal.State.Completed
            );

            // セルから削除。
            DungeonManager.RemoveActor(Coords, this);

            Destroy(gameObject);
        }
    }
}