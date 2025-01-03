using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class Adventurer : Character
    {
        AdventurerSheet _adventurerSheet;
        Blackboard _blackboard;
        Vector2Int _currentCoords;
        Vector2Int _currentDirection;
        string _selectedAction;
        bool _isInitialized;
        
        public AdventurerSheet AdventurerSheet => _adventurerSheet;
        public override Vector2Int Coords => _currentCoords;
        public override Vector2Int Direction => _currentDirection;
        public string SelectedAction => _selectedAction;

        void Awake()
        {
            _blackboard = GetComponent<Blackboard>();
        }

        void Start()
        {
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        // スプレッドシートから読み込んだデータを渡す。
        // もしくはデータをシリアライズしてインスペクターから触れるようにしても良い。
        public void Initialize(AdventurerSheet adventurerSheet)
        {
            _adventurerSheet = adventurerSheet;
            
            // レベルに応じて体力を設定。
            _blackboard.MaxHp = CalculationFormula.GetHp(adventurerSheet.Level);
            _blackboard.CurrentHp = _blackboard.MaxHp;
            // 心情は生成時、自身へのコメントで上下する。
            _blackboard.MaxEmotion = 100;
            _blackboard.CurrentEmotion = 50;            
            // 空腹はターン経過で増加していく。
            _blackboard.MaxHunger = 100;
            _blackboard.CurrentHunger = 99;
            // レベルに応じて攻撃力を設定。
            _blackboard.Attack = CalculationFormula.GetAttack(adventurerSheet.Level);
            _blackboard.AttackMagnification = 1.0f;
            // レベルに応じて行動速度を設定。
            _blackboard.Speed = CalculationFormula.GetSpeed(adventurerSheet.Level);
            _blackboard.SpeedMagnification = 1.0f;
            // ダンジョンの入り口が固定で1箇所。
            _blackboard.Coords = new Vector2Int(11, 8);
            // 上以外の向きの場合、回転させる処理が必要。
            _blackboard.Direction = Vector2Int.up;

            _isInitialized = true;
        }

        public void SetCoords(Vector2Int coords) => _currentCoords = coords;
        public void SetDirection(Vector2Int direction) => _currentDirection = direction;

        // これも後でリファクタする。
        public void Cleanup()
        {
            if (TryGetComponent(out ActionLog log)) log.Delete();
            if (TryGetComponent(out ExploreRecord record)) record.Delete();
            if (TryGetComponent(out InformationStock information)) information.RequestDelete();
            if (TryGetComponent(out GamePlayAI ai)) ai.PreInitialize();
        }

        async UniTask UpdateAsync(CancellationToken token)
        {
            // 初期化が完了するまで待つ。
            await UniTask.WaitUntil(() => _isInitialized, cancellationToken: token);

            // 各種AIを初期化。
            TryGetComponent(out GamePlayAI gamePlayAI);
            gamePlayAI.PreInitialize();
            TryGetComponent(out RolePlay rolePlayAI);
            rolePlayAI.Initialize();

            // 生成したセル上に自身を移動と追加。
            DungeonManager.AddActor(Coords, this);
            transform.position = DungeonManager.GetCell(Coords).Position;

            // UIに反映。
            if (TryGetComponent(out StatusBarBinder statusBar)) statusBar.Register();
            if (TryGetComponent(out ProfileWindowBinder profileWindow)) profileWindow.Register();
            if (TryGetComponent(out CameraFocusBinder cameraFocus)) cameraFocus.Register();
            if (this.TryGetComponentInChildren(out NameTag nameTag)) nameTag.SetName(AdventurerSheet.DisplayName);

            // 登場時の演出。台詞を表示させるので、UIに自身を反映した後に呼ぶ。
            if (TryGetComponent(out EntryAction entry)) entry.Play();

            // コメントを流し、心情の値を変化させる。
            CommentDisplayer.TryFind(out CommentDisplayer commentDisplayer);
            IReadOnlyCollection<CommentData> comment = commentDisplayer.Display(AdventurerSheet.FullName);
            if (!(comment == null || comment.Count == 0))
            {
                float score = 1; // コメントの仕様書が来るまで仮の値。
                float add = (_blackboard.MaxEmotion / 100.0f) * (20.0f * score);
                _blackboard.CurrentEmotion += Mathf.CeilToInt(add);
            }

            // UIに反映。
            if (statusBar != null) statusBar.Apply();

            // サブゴールを決める。
            TryGetComponent(out SubGoalPath subGoalPath);
            subGoalPath.Initialize(await SubGoalSelector.SelectAsync(AdventurerSheet, token));

            TryGetComponent(out HungryStatusEffect hungryStatusEffect);
            TryGetComponent(out MadnessStatusEffect madnessStatusEffect);
            TryGetComponent(out BuffStatusEffect buffStatusEffect);
            TryGetComponent(out InformationStock informationStock);
            TryGetComponent(out TalkThemeSelector talkThemeSelectAI);
            TryGetComponent(out DirectionMoveAction moveToDirection);
            TryGetComponent(out TargetMoveAction moveToTarget);
            TryGetComponent(out AttackAction attack);
            TryGetComponent(out ScavengeAction scavenge);
            TryGetComponent(out TalkAction talk);
            TryGetComponent(out DefeatedAction defeated);
            TryGetComponent(out EscapeAction escape);
            TryGetComponent(out AvailableActions availableActions);
            TryGetComponent(out ActionEvaluator actionEvaluator);
            TerrainFeature.TryFind(out TerrainFeature terrainFeature);
            while (!token.IsCancellationRequested)
            {
                // ターン数を更新。
                _blackboard.ElapsedTurn++;

                // 空腹を増加。
                _blackboard.CurrentHunger++;

                // 空腹が最大の場合、空腹のステータス効果を付与。
                if (_blackboard.IsHungry) hungryStatusEffect.Apply();
                else hungryStatusEffect.Remove();

                statusBar.Apply();

                // 現在いるセルについて、地形の特徴に関する情報がある場合、
                // AIが次の行動を選択する際に、考慮する情報の候補として追加する。
                if (terrainFeature.TryGetInformation(Coords, out IReadOnlyList<Information> features))
                {
                    // プレイする度、行動にバラつきを持たせるため、複数ある場合はランダムで1つ選ぶ。
                    int random = Random.Range(0, features.Count);
                    informationStock.AddPending(features[random]);
                }

                // 周囲の状況やゴール、ステータスを調べ、それぞれの選択肢にスコア付け。
                foreach ((string action, float score) value in actionEvaluator.Evaluate())
                {
                    availableActions.SetScore(value.action, value.score);
                }

                // 保持している情報を更新。
                // 新しい情報を知った場合、このタイミングで保持している情報に追加される。
                await informationStock.RefreshAsync(token);
                await talkThemeSelectAI.SelectAsync(token);

                // 情報を更新したのでUIに反映。
                if (profileWindow != null) profileWindow.Apply();

                // AIが次の行動を選択し、実行。
                _selectedAction = await gamePlayAI.RequestNextActionAsync(token);
                string actionResult;
                if (_selectedAction == "MoveNorth")
                {
                    actionResult = await moveToDirection.PlayAsync(Vector2Int.up, token);
                }
                else if (_selectedAction == "MoveSouth")
                {
                    actionResult = await moveToDirection.PlayAsync(Vector2Int.down, token);
                }
                else if (_selectedAction == "MoveEast")
                {
                    actionResult = await moveToDirection.PlayAsync(Vector2Int.right, token);
                }
                else if (_selectedAction == "MoveWest")
                {
                    actionResult = await moveToDirection.PlayAsync(Vector2Int.left, token);
                }
                else if (_selectedAction == "MoveToEntrance")
                {
                    actionResult = await moveToTarget.PlayAsync("Entrance", token);
                }
                else if (_selectedAction == "AttackToEnemy")
                {
                    actionResult = await attack.PlayAsync<Enemy>(token);
                }
                else if (_selectedAction == "AttackToAdventurer")
                {
                    actionResult = await attack.PlayAsync<Adventurer>(token);
                }
                else if (_selectedAction == "TalkWithAdventurer")
                {
                    actionResult = await talk.PlayAsync(token);
                }
                else if (_selectedAction == "Scavenge")
                {
                    actionResult = await scavenge.PlayAsync(token);
                }

                // 足元に罠等がある場合に起動。
                foreach (Actor actor in DungeonManager.GetActors(Coords))
                {
                    if (actor.ID == "Trap" && actor is DungeonEntity e)
                    {
                        e.Interact(this);
                    }
                }

                // ステータス効果を反映。
                hungryStatusEffect.Tick();
                madnessStatusEffect.Tick();
                buffStatusEffect.Tick();

                // 行動結果による選択肢のスコア付け。

                // 撃破されたもしくは脱出した場合。
                if (await defeated.PlayAsync(token) || await escape.PlayAsync(token))
                {
                    TryGetComponent(out AdventureResultReporter adventureResult);
                    adventureResult.Send();
                    break;
                }

                // サブゴールを達成した場合、次のサブゴールを設定。
                if (subGoalPath.IsAchieve())
                {
                    GameLog.Add(
                        $"システム",
                        $"{AdventurerSheet.DisplayName}が「{subGoalPath.GetCurrent().Text.Japanese}」を達成。",
                        GameLogColor.White
                    );

                    subGoalPath.SetNext();
                }

                await UniTask.Yield(cancellationToken: token);
            }

            // セルから削除。
            DungeonManager.RemoveActor(Coords, this);

            Destroy(gameObject);
        }
    }
}