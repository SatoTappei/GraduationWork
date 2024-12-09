using Cysharp.Threading.Tasks;
using System.Collections;
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
            _blackboard.MaxHp = 100;                    // 自由に設定可能。
            _blackboard.CurrentHp = 100;                // 自由に設定可能。
            _blackboard.MaxEmotion = 100;               // 自由に設定可能。
            _blackboard.CurrentEmotion = 50;            // 生成時、自身へのコメントで上下する。
            _blackboard.MaxFatigue = 100;               // 自由に設定可能。
            _blackboard.CurrentFatigue = 0;             // ターン経過で増加していく。
            _blackboard.Coords = new Vector2Int(11, 8); // ダンジョンの入り口が固定で1箇所。
            _blackboard.Direction = Vector2Int.up;      // 上以外の向きの場合、回転させる処理が必要。

            _isInitialized = true;
        }

        public void Talk(BilingualString text, string source, Vector2Int coords)
        {
            if (_isInitialized && TryGetComponent(out TalkApply talk))
            {
                talk.Talk(text, source);
            }
        }

        public void StatusBuff(float attack, float speed)
        {
            if (_isInitialized && TryGetComponent(out StatusBuffApply statusBuff))
            {
                statusBuff.Buff(attack, speed);
            }
        }

        public sealed override string Damage(string id, string weapon, int value, Vector2Int coords)
        {
            if (_isInitialized && TryGetComponent(out DamageApply damage))
            {
                return damage.Damage(id, weapon, value, coords);
            }
            else return "Miss";
        }

        async UniTask UpdateAsync(CancellationToken token)
        {
            // 初期化が完了するまで待つ。
            await UniTask.WaitUntil(() => _isInitialized, cancellationToken: token);

            // 初期化時に黒板に書き込まれる値を、AwakeやStartのタイミングで参照するコンポーネント群を追加。
            gameObject.AddComponent<RolePlayAI>();
            gameObject.AddComponent<GamePlayAI>();
            gameObject.AddComponent<ScoreEvaluateAI>();
            gameObject.AddComponent<TurnEvaluateAI>();
            gameObject.AddComponent<TalkThemeSelectAI>();
            gameObject.AddComponent<CommentReactionAI>();

            // 生成したセル上に自身を移動と追加。
            DungeonManager.TryFind(out DungeonManager dungeonManager);
            dungeonManager.AddActorOnCell(Coords, this);
            transform.position = dungeonManager.GetCell(Coords).Position;

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
            await subGoalPath.PlanningAsync(token);

            TryGetComponent(out FatigueDamageApply fatigueDamage);
            TryGetComponent(out GamePlayAI gamePlayAI);
            TryGetComponent(out InformationStock informationStock);
            TryGetComponent(out MovementToDirection moveToDirection);
            TryGetComponent(out MovementToTarget moveToTarget);
            TryGetComponent(out AttackToSurrounding attack);
            TryGetComponent(out ScavengeToSurrounding scavenge);
            TryGetComponent(out TalkToSurrounding talk);
            TryGetComponent(out Defeated defeated);
            TryGetComponent(out EscapeFromDungeon escape);
            TryGetComponent(out FootTrapApply foot);
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
                if (dungeonManager.TryGetTerrainFeature(Coords, out SharedInformation feature))
                {
                    informationStock.AddPending(feature);
                }

                // 保持している情報を更新。
                // 新しい情報を知った場合、このタイミングで保持している情報に追加される。
                await informationStock.RefreshAsync(token);

                // 情報を更新したのでUIに反映。
                if (profileWindow != null) profileWindow.Apply();

                // AIが次の行動を選択し、実行。
                _selectedAction = await gamePlayAI.RequestNextActionAsync(token);
                if (_selectedAction == "Idle") await UniTask.Yield(cancellationToken: token);
                if (_selectedAction == "Move North") await moveToDirection.MoveAsync(Vector2Int.up, token);
                if (_selectedAction == "Move South") await moveToDirection.MoveAsync(Vector2Int.down, token);
                if (_selectedAction == "Move East") await moveToDirection.MoveAsync(Vector2Int.right, token);
                if (_selectedAction == "Move West") await moveToDirection.MoveAsync(Vector2Int.left, token);
                if (_selectedAction == "Return To Entrance") await moveToTarget.MoveAsync("Entrance", token);
                if (_selectedAction == "Attack Surrounding") await attack.AttackAsync<Enemy>(token);
                if (_selectedAction == "Scavenge Surrounding") await scavenge.ScavengeAsync(token);
                if (_selectedAction == "Talk Surrounding") await talk.TalkAsync(token);

                // 足元に罠等がある場合に起動。
                if (foot != null) foot.Activate();

                // 撃破された場合。
                if (await defeated.DefeatedAsync(token))
                {
                    GameManager.ReportAdventureResult(this, "Defeated");
                    break;
                }

                // 脱出した場合。
                if (await escape.EscapeAsync(token))
                {
                    GameManager.ReportAdventureResult(this, "Escape");
                    break;
                }

                // サブゴールを達成した場合、次のサブゴールを設定。
                if (subGoalPath.Current.IsCompleted())
                {
                    // 利用可能な行動の選択肢がある場合は追加。
                    TryGetComponent(out AvailableActions availableActions);
                    availableActions.Add(subGoalPath.Current.GetAdditionalActions());

                    // サブゴールを達成した際の演出。
                    if (TryGetComponent(out SubGoalEffect subGoalEffect)) subGoalEffect.Play();

                    subGoalPath.HeadingNext();
                }

                await UniTask.Yield(cancellationToken: token);
            }

            Destroy(gameObject);
        }
    }
}