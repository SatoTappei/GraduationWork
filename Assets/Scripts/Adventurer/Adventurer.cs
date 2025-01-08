using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class Adventurer : Actor
    {
        AdventurerSheet _adventurerSheet;
        Status _status;
        Vector2Int _coords;
        Vector2Int _direction;
        string _selectedAction;
        bool _isInitialized;

        GamePlay _gamePlay;
        RolePlay _rolePlay;
        TalkThemeSelector _talkTheme;
        
        StatusBarBinder _statusBar;
        ProfileWindowBinder _profileWindow;
        CameraFocusBinder _cameraFocus;
        
        EntryAction _entry;
        DirectionMoveAction _directionMove;
        TargetMoveAction _targetMove;
        AttackAction _attack;
        TalkAction _talk;
        ScavengeAction _scavenge;
        DefeatedAction _defeated;
        EscapeAction _escape;

        PreActionEvaluator _preEvaluator;
        PostActionEvaluator _postEvaluator;

        SubGoalPath _subGoal;
        HoldInformation _information;
        HungryStatusEffect _hungry;
        AdventureResultReporter _result;

        StatusEffect[] _statusEffects;

        CommentDisplayer _commentDisplayer;       

        public AdventurerSheet AdventurerSheet => _adventurerSheet;
        public Status Status => _status;
        public override Vector2Int Coords => _coords;
        public override Vector2Int Direction => _direction;
        public string SelectedAction => _selectedAction;

        void Awake()
        {
            _gamePlay = GetComponent<GamePlay>();
            _rolePlay = GetComponent<RolePlay>();
            _talkTheme = GetComponent<TalkThemeSelector>();
            _statusBar = GetComponent<StatusBarBinder>();
            _profileWindow = GetComponent<ProfileWindowBinder>();
            _cameraFocus = GetComponent<CameraFocusBinder>();
            _entry = GetComponent<EntryAction>();
            _directionMove = GetComponent<DirectionMoveAction>();
            _targetMove = GetComponent<TargetMoveAction>();
            _attack = GetComponent<AttackAction>();
            _talk = GetComponent<TalkAction>();
            _scavenge = GetComponent<ScavengeAction>();
            _defeated = GetComponent<DefeatedAction>();
            _escape = GetComponent<EscapeAction>();
            _preEvaluator = GetComponent<PreActionEvaluator>();
            _postEvaluator = GetComponent<PostActionEvaluator>();
            _subGoal = GetComponent<SubGoalPath>();
            _information = GetComponent<HoldInformation>();
            _hungry = GetComponent<HungryStatusEffect>();
            _result = GetComponent<AdventureResultReporter>();
            _statusEffects = GetComponents<StatusEffect>();
            _commentDisplayer = CommentDisplayer.Find();
        }

        void Start()
        {
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        public void Initialize(AdventurerSheet adventurerSheet)
        {
            _adventurerSheet = adventurerSheet;
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
            _status.ActionLog.Delete();
            _status.ExploreRecord.Delete();
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
            _cameraFocus.Register();
            
            // キャラクターの頭上に表示されるネームタグの表示名を反映。
            if (this.TryGetComponentInChildren(out NameTag nameTag))
            {
                nameTag.SetName(AdventurerSheet.DisplayName);
            }

            // 登場時の演出。台詞を表示させるので、UIに自身を登録した後に呼ぶ。
            _entry.Play();

            // コメントを流し、心情の値を変化させる。
            IReadOnlyCollection<CommentData> comment = _commentDisplayer.Display(AdventurerSheet.FullName);
            if (!(comment == null || comment.Count == 0))
            {
                float score = 1; // コメントの仕様書が来るまで仮の値。
                float add = (_status.MaxEmotion / 100.0f) * (20.0f * score);
                _status.CurrentEmotion += Mathf.CeilToInt(add);
            }

            // 変化した心情をUIに反映。
            _statusBar.Apply();

            // サブゴールを決める。
            _subGoal.Initialize(await SubGoalSelector.SelectAsync(AdventurerSheet, token));

            // ここまでが1ターン目開始までの処理。以降の処理は毎ターン繰り返される。
            while (!token.IsCancellationRequested)
            {
                // 経過ターン数、空腹を増加。
                _status.ElapsedTurn++;
                _status.CurrentHunger++;

                // 空腹が最大の場合、空腹のステータス効果を付与。
                if (_status.IsHungry) _hungry.Set();
                else _hungry.Remove();

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
                await _talkTheme.SelectAsync(token);

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
                else
                {
                    Debug.LogWarning($"行動を選ぶAIの出力が、指定した形式と違う。: {selectedAction}");
                    await UniTask.Yield(cancellationToken: token);
                }

                // 行動結果を基に、それぞれの選択肢をスコア付け。
                if (actionResult != null)
                {
                    _postEvaluator.Evaluate(actionResult);
                }

                // 行動結果を反映。
                if (actionResult != null)
                {
                    DungeonManager.RemoveActor(Coords, this);
                    _coords = actionResult.Coords;
                    DungeonManager.AddActor(_coords, this);

                    _direction = actionResult.Direction;

                    Status.ActionLog.Add(actionResult.Log);
                }

                // インスペクター上で確認できるよう、適当なメンバ変数に反映させておく。
                _selectedAction = selectedAction;

                // 足元に罠等がある場合に起動。
                foreach (Actor actor in DungeonManager.GetActors(Coords))
                {
                    if (actor.ID == "Trap" && actor is DungeonEntity e)
                    {
                        e.Interact(this);
                    }
                }

                // ステータス効果を反映。
                foreach (StatusEffect e in _statusEffects) e.Apply();

                // 撃破されたもしくは脱出した場合。
                if (await _defeated.PlayAsync(token) || await _escape.PlayAsync(token))
                {
                    _result.Send();
                    break;
                }

                // サブゴールを達成した場合、次のサブゴールを設定。
                if (_subGoal.IsAchieve(out string result))
                {
                    if (result == "Completed") result = "達成";
                    else if (result == "Retire") result = "諦めた";

                    GameLog.Add(
                        $"システム",
                        $"{AdventurerSheet.DisplayName}が「{_subGoal.GetCurrent().Description.Japanese}」を{result}。",
                        GameLogColor.White
                    );

                    _subGoal.SetNext();
                }

                await UniTask.Yield(cancellationToken: token);
            }

            // セルから削除。
            DungeonManager.RemoveActor(Coords, this);

            Destroy(gameObject);
        }
    }
}