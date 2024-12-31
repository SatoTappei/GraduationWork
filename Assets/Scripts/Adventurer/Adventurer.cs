using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class Adventurer : Character
    {
        Blackboard _blackboard;
        string _selectedAction;
        bool _isInitialized;
        
        public AdventurerSheet AdventurerSheet => _blackboard.AdventurerSheet;
        public override Vector2Int Coords => _blackboard.Coords;
        public override Vector2Int Direction => _blackboard.Direction;
        public string SelectedAction => _selectedAction;

        void Awake()
        {
            _blackboard = GetComponent<Blackboard>();
        }

        void Start()
        {
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        void OnDestroy()
        {
            _isInitialized = false;
        }

        // スプレッドシートから読み込んだデータを渡す。
        // もしくはデータをシリアライズしてインスペクターから触れるようにしても良い。
        public void Initialize(AdventurerSheet adventurerSheet)
        {
            _blackboard.AdventurerSheet = adventurerSheet;
            // レベルに応じて体力を設定。
            _blackboard.MaxHp = CalculationFormula.GetHp(adventurerSheet.Level);
            _blackboard.CurrentHp = _blackboard.MaxHp;
            // 心情は生成時、自身へのコメントで上下する。
            _blackboard.MaxEmotion = 100;
            _blackboard.CurrentEmotion = 50;            
            // 疲労はターン経過で増加していく。
            _blackboard.MaxFatigue = 100;
            _blackboard.CurrentFatigue = 0;
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

        public void Cleanup()
        {
            if (TryGetComponent(out ActionLog log)) log.Delete();
            if (TryGetComponent(out ExploreRecord record)) record.Delete();
            if (TryGetComponent(out InformationStock information)) information.RequestDelete();
            if (TryGetComponent(out GamePlayAI ai)) ai.PreInitialize();
        }

        public void Talk(BilingualString text, string source, Vector2Int coords)
        {
            if (_isInitialized && TryGetComponent(out TalkApply talk))
            {
                talk.Talk(text, source, coords);
            }
        }

        public void StatusBuff(string type, float value, Vector2Int coords)
        {
            if (_isInitialized && TryGetComponent(out StatusBuffApply statusBuff))
            {
                statusBuff.Buff(type, value, coords);
            }
        }

        public void Heal(int value, Vector2Int coords)
        {
            if (_isInitialized && TryGetComponent(out HealApply heal))
            {
                heal.Heal(value, coords);
            }
        }

        public sealed override string Damage(int value, Vector2Int coords, string effect = "")
        {
            if (_isInitialized && TryGetComponent(out DamageApply damage))
            {
                return damage.Damage(value, coords, effect);
            }
            else return "Miss";
        }

        async UniTask UpdateAsync(CancellationToken token)
        {
            // 初期化が完了するまで待つ。
            await UniTask.WaitUntil(() => _isInitialized, cancellationToken: token);

            // 各種AIを初期化。
            TryGetComponent(out GamePlayAI gamePlayAI);
            gamePlayAI.PreInitialize();
            TryGetComponent(out RolePlayAI rolePlayAI);
            rolePlayAI.Initialize();

            // 生成したセル上に自身を移動と追加。
            DungeonManager.AddActorOnCell(Coords, this);
            transform.position = DungeonManager.GetCell(Coords).Position;

            // UIに反映。
            if (TryGetComponent(out StatusBarApply statusBar)) statusBar.Register();
            if (TryGetComponent(out ProfileWindowApply profileWindow)) profileWindow.Register();
            if (TryGetComponent(out CameraFocusTargetApply cameraFocusTarget)) cameraFocusTarget.Register();
            if (this.TryGetComponentInChildren(out NameTag nameTag)) nameTag.SetName(_blackboard.DisplayName);

            // 登場時の演出。台詞を表示させるので、UIに自身を反映した後に呼ぶ。
            if (TryGetComponent(out EntryToDungeon entry)) entry.Entry();

            // コメントを流し、その内容に対して反応する。
            if (TryGetComponent(out CommentApply commentApply)) commentApply.Reaction();

            // サブゴールを決める。
            TryGetComponent(out SubGoalPath subGoalPath);
            subGoalPath.Initialize(await SubGoalSelector.SelectAsync(AdventurerSheet, token));

            TryGetComponent(out FatigueDamageApply fatigueDamage);
            TryGetComponent(out InformationStock informationStock);
            TryGetComponent(out TalkThemeSelectAI talkThemeSelectAI);
            TryGetComponent(out MovementToDirection moveToDirection);
            TryGetComponent(out MovementToTarget moveToTarget);
            TryGetComponent(out AttackToSurrounding attack);
            TryGetComponent(out ScavengeToSurrounding scavenge);
            TryGetComponent(out TalkToSurrounding talk);
            TryGetComponent(out Defeated defeated);
            TryGetComponent(out EscapeFromDungeon escape);
            TryGetComponent(out FootTrapApply foot);
            TryGetComponent(out AvailableActions availableActions);
            TryGetComponent(out ActionEvaluator actionEvaluator);
            TerrainFeature.TryFind(out TerrainFeature terrainFeature);
            while (!token.IsCancellationRequested)
            {
                // ターン数を更新。
                _blackboard.ElapsedTurn++;

                // 疲労を増加。
                _blackboard.CurrentFatigue++;

                // 疲労が最大の場合、毎ターン体力が減り続ける。
                if (_blackboard.IsFatigueMax)
                {
                    fatigueDamage.Damage();
                    if (statusBar != null) statusBar.Apply();
                }

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
                if (_selectedAction == "Idle") await UniTask.Yield(cancellationToken: token);
                if (_selectedAction == "MoveNorth") await moveToDirection.MoveAsync(Vector2Int.up, token);
                if (_selectedAction == "MoveSouth") await moveToDirection.MoveAsync(Vector2Int.down, token);
                if (_selectedAction == "MoveEast") await moveToDirection.MoveAsync(Vector2Int.right, token);
                if (_selectedAction == "MoveWest") await moveToDirection.MoveAsync(Vector2Int.left, token);
                if (_selectedAction == "MoveToEntrance") await moveToTarget.MoveAsync("Entrance", token);
                if (_selectedAction == "AttackToEnemy") await attack.AttackAsync<Enemy>(token);
                if (_selectedAction == "AttackToAdventurer") await attack.AttackAsync<Adventurer>(token);
                if (_selectedAction == "TalkWithAdventurer") await talk.TalkAsync(token);
                if (_selectedAction == "Scavenge") await scavenge.ScavengeAsync(token);

                // 足元に罠等がある場合に起動。
                if (foot != null) foot.Activate();

                // 行動結果による選択肢のスコア付け。

                // 撃破されたもしくは脱出した場合。
                if (await defeated.DefeatedAsync(token) || await escape.EscapeAsync(token))
                {
                    TryGetComponent(out AdventureResultApply adventureResult);
                    adventureResult.Send();
                    break;
                }

                // サブゴールを達成した場合、次のサブゴールを設定。
                if (subGoalPath.IsAchieve())
                {
                    // サブゴールを達成した際の演出。
                    if (TryGetComponent(out SubGoalEffect subGoalEffect)) subGoalEffect.Play();

                    subGoalPath.SetNext();
                }

                await UniTask.Yield(cancellationToken: token);
            }

            Destroy(gameObject);
        }
    }
}