using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Game
{
    public abstract class Adventurer : Character, 
        IStatusBarDisplayStatus, 
        IProfileWindowDisplayStatus, 
        ITalkable, 
        IGamePlayAIResource, 
        IRolePlayAIResource
    {
        [SerializeField] AdventurerSheet _adventurerSheet;
        [SerializeField] Vector2Int _spawnCoords;

        DungeonManager _dungeonManager;
        UiManager _uiManager;
        Animator _animator;

        Vector2Int _currentCoords;
        Vector2Int _currentDirection;
        RolePlayAI _rolePlayAI;
        GamePlayAI _gamePlayAI;
        ScoreEvaluateAI _scoreEvaluateAI;
        TurnEvaluateAI _turnEvaluateAI;
        TalkContentSelectAI _talkContentSelectAI;
        Transform _forwardAxis;
        Inventory _inventory;
        HoldedInformation _holdedInformation;
        Queue<SharedInformation> _pendingInformation;
        SharedInformation _selectedInformation;
        List<Cell> _path;
        string _pathTarget;
        int _currentPathIndex;
        SubGoal[] _subGoals;
        int _currentSubGoalIndex;
        int _statusBarID;
        int _profileWindowID;
        int _cameraFocusID;
#if UNITY_EDITOR
        string _selectedAction;
#endif

        public override Vector2Int Coords => _currentCoords;
        public override Vector2Int Direction => _currentDirection;

        public List<string> AvailableActions { get; private set; }
        public ExploreRecord ExploreRecord { get; private set; }
        public Queue<string> ActionLog { get; private set; }
        public int CurrentHp { get; private set; }
        public int CurrentEmotion { get; private set; }
        public int ElapsedTurn { get; private set; }
        public int TreasureCount { get; private set; }
        public int ItemCount { get; private set; }
        public int DefeatCount { get; private set; }

        public AdventurerSheet AdventurerSheet => _adventurerSheet;
        public IReadOnlyInventory ItemInventory => _inventory;
        public Sprite Icon => _adventurerSheet.Icon;
        public string FullName => _adventurerSheet.FullName;
        public string DisplayName => _adventurerSheet.DisplayName;
        public string Job => _adventurerSheet.Job;
        public string Background => _adventurerSheet.Background;
        public int MaxHp => 100;
        public int MaxEmotion => 100;
        public SubGoal CurrentSubGoal
        {
            get
            {
                if (_subGoals == null) return null;
                if (_currentSubGoalIndex < 0 || _subGoals.Length <= _currentSubGoalIndex) return null;
                if (_subGoals[_currentSubGoalIndex] == null) return null;

                return _subGoals[_currentSubGoalIndex];
            }
        }
        public bool IsDefeated => CurrentHp <= 0;
        public bool IsAlive => !IsDefeated;

        string IProfileWindowDisplayStatus.Goal
        {
            get
            {
                if (CurrentSubGoal == null) return string.Empty;
                else return CurrentSubGoal.Text.Japanese;
            }
        }
        IReadOnlyInventory IProfileWindowDisplayStatus.ItemInventory => _inventory;
        IReadOnlyList<SharedInformation> IProfileWindowDisplayStatus.Information => _holdedInformation.Contents;

        IReadOnlyExploreRecord IGamePlayAIResource.ExploreRecord => ExploreRecord;
        IReadOnlyCollection<string> IGamePlayAIResource.ActionLog => ActionLog;
        IReadOnlyList<SharedInformation> IGamePlayAIResource.Information => _holdedInformation.Contents;
        IReadOnlyList<string> IGamePlayAIResource.AvailableActions => AvailableActions;

        protected virtual void Awake()
        {
            _currentCoords = _spawnCoords;
            _currentDirection = Vector2Int.up;
            _forwardAxis = transform.FindChildRecursive("ForwardAxis");
            _inventory = new Inventory();
            _holdedInformation = new HoldedInformation();
            _pendingInformation = new Queue<SharedInformation>();
            _path = new List<Cell>();

            AvailableActions = new List<string>()
            {
                "Move North",
                "Move South",
                "Move East",
                "Move West",
                "Attack Surrounding",
                "Scavenge Surrounding",
                "Talk Surrounding"
            };

            ExploreRecord = new ExploreRecord(Blueprint.Height, Blueprint.Width);
            ActionLog = new Queue<string>();
            CurrentHp = MaxHp;
            CurrentEmotion = MaxEmotion;

            _dungeonManager = DungeonManager.Find();
            _uiManager = UiManager.Find();
            _animator = GetComponentInChildren<Animator>();
        }

        protected virtual void Start()
        {
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        protected virtual void OnDestroy()
        {
            DeleteStatusBar();
            DeleteCameraFocusTarget();
            DeleteProfileWindow();
        }

        public void Initialize(AdventurerSheet adventurerSheet)
        {
            _adventurerSheet = adventurerSheet;
        }

        public void Talk(BilingualString text, string source, Vector2Int coords)
        {
            AddPendingInformation(text, source);
        }

        public sealed override string Damage(string id, string weapon, int value, Vector2Int coords)
        {
            if (IsDefeated) return "Corpse";

            PlayDamageEffect(coords);
            DecreaseHp(value);
            ShowLine(RequestLineType.Damage);
            UpdateStatusBar();

            if (IsDefeated) return "Defeated";
            else return "Hit";
        }

        async UniTask UpdateAsync(CancellationToken token)
        {
            // 外部から必要な参照を渡すことを想定。
            // AwakeでもStartでも任意のタイミングで渡して大丈夫なよう、渡してくれるまで以降の処理を待つ。
            await WaitInitializeAsync(token);

            // AIクラスはAdventurerSheetが必要。
            _rolePlayAI = new RolePlayAI(this);
            _gamePlayAI = new GamePlayAI(this);
            _scoreEvaluateAI = new ScoreEvaluateAI();
            _turnEvaluateAI = new TurnEvaluateAI();
            _talkContentSelectAI = new TalkContentSelectAI();

            PlayEntryEffect();
            RegisterStatusBar();
            RegisterCameraFocusTarget();
            RegisterProfileWindow();
            AddActorOnCell();
            SetNameTag();
            SetPosition(Coords);
            ShowLine(RequestLineType.Entry);
            AddGameLog($"{AdventurerSheet.DisplayName}がダンジョンにやってきた。");

            UpdateHoldedInformationAsync(token).Forget();

            _subGoals = await SelectSubGoalAsync(token);
            _currentSubGoalIndex = 0;

            while (!token.IsCancellationRequested)
            {
                ElapsedTurn++;
                RefreshHoldedInformation();
                AddTerrainFeatureInformation();
                UpdateProfileWindow();

                string response = await _gamePlayAI.RequestNextActionAsync();
                switch (response)
                {
                    case "Move North": await MoveAsync(Vector2Int.up, token); break;
                    case "Move South": await MoveAsync(Vector2Int.down, token); break;
                    case "Move East": await MoveAsync(Vector2Int.right, token); break;
                    case "Move West": await MoveAsync(Vector2Int.left, token); break;
                    case "Return To Entrance": await MoveAsync("Entrance", token); break;
                    case "Attack Surrounding": await AttackAsync(token); break;
                    case "Scavenge Surrounding": await ScavengeAsync(token); break;
                    case "Talk Surrounding": await TalkAsync(token); break;
                }

#if UNITY_EDITOR
                _selectedAction = response;
#endif

                if (await DefeatedAsync(token) || await EscapeAsync(token)) break;

                // サブゴールを達成した場合、次のサブゴールを設定。
                if (CurrentSubGoal.IsCompleted())
                {
                    AddGameLog($"{DisplayName}が「{CurrentSubGoal.Text.Japanese}」を達成。");
                    _currentSubGoalIndex++;
                    // 利用可能な行動の選択肢がある場合は追加。
                    AddAvailableActions(CurrentSubGoal.GetAdditionalActions());
                }

                // OnUpdateAsyncが同期的の場合、無限ループに陥るので1フレーム待つ。
                await UniTask.Yield();
            }

            Destroy(gameObject);
        }

        async UniTask WaitInitializeAsync(CancellationToken token)
        {
            await UniTask.WaitUntil(() => _adventurerSheet != null, cancellationToken: token);
        }

        async UniTask UpdateHoldedInformationAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            while (!token.IsCancellationRequested)
            {
                // 保留中の情報がある場合、評価して既存の情報と比較＆入れ替えを行う。
                // 自身が保持している情報が更新されたので、他冒険者との会話の際に伝える情報も更新する。
                if (_pendingInformation.TryDequeue(out SharedInformation info))
                {
                    info.Score = await _scoreEvaluateAI.EvaluateAsync(info, token);
                    info.RemainingTurn = await _turnEvaluateAI.EvaluateAsync(info, token);
                    _holdedInformation.Add(info);
                    _selectedInformation = await _talkContentSelectAI.SelectAsync(_holdedInformation.Contents, token);
                }

                await UniTask.Yield(cancellationToken: token);
            }
        }

        // キャラクターの設定を考慮してサブゴールを選択するよう、AIにリクエストして結果を返す。
        public async UniTask<SubGoal[]> SelectSubGoalAsync(CancellationToken token)
        {
            await WaitInitializeAsync(token);

            IReadOnlyList<string> result = await _rolePlayAI.RequestSubGoalAsync(token);

            SubGoal[] subGoals = new SubGoal[result.Count];
            for (int i = 0; i < result.Count; i++)
            {
                subGoals[i] = Convert(result[i]);
            }

            return subGoals;

            // AIは日本語の文字列で選択したサブゴールを返すので、対応するクラスのインスタンスに変換する。
            SubGoal Convert(string text)
            {
                if (text == GetTreasure.StaticText.Japanese) return new GetTreasure(this);
                if (text == GetRequestedItem.StaticText.Japanese) return new GetRequestedItem(this);
                if (text == ExploreDungeon.StaticText.Japanese) return new ExploreDungeon(this);
                if (text == DefeatWeakEnemy.StaticText.Japanese) return new DefeatWeakEnemy(this);
                if (text == DefeatStrongEnemy.StaticText.Japanese) return new DefeatStrongEnemy(this);
                if (text == DefeatAdventurer.StaticText.Japanese) return new DefeatAdventurer(this);
                if (text == ReturnToEntrance.StaticText.Japanese) return new ReturnToEntrance(this);

                Debug.LogError($"AIが選択したサブゴールに対応するクラスが無い。: {text}");

                return null;
            }
        }

        async UniTask MoveAsync(Vector2Int direction, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            PathfindingToNeighbourCell(direction);
            await MoveNextCellAsync(token);
        }

        async UniTask MoveAsync(string target, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            PathfindingIfTargetChanged(target);
            await MoveNextCellAsync(token);
        }

        async UniTask AttackAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            if (TryGetAttackTarget(out Actor target))
            {
                await RotateToActorDirectionAsync(target, token);
                PlayAnimation("Attack");
                ShowLine(RequestLineType.Attack);

                switch (ApplyDamage(target as IDamageable))
                {
                    case "Defeated":
                        ShowLine(RequestLineType.DefeatEnemy);
                        AddGameLog($"{DisplayName}が敵を倒した。");
                        AddActionLog("I attacked the enemy. I defeated the enemy.");
                        DefeatCount++;
                        break;
                    case "Hit":
                        AddActionLog("I attacked the enemy. The enemy is still alive.");
                        break;
                    case "Corpse":
                        AddActionLog("I tried to attack it, but it was already dead.");
                        break;
                }
            }
            else
            {
                AddActionLog("There are no enemies around to attack.");
            }
        }

        async UniTask ScavengeAsync(CancellationToken token)
        {
            if (TryGetScavengeTarget(out Actor target))
            {
                await RotateToActorDirectionAsync(target, token);
                PlayAnimation("Scav");

                string foundItem = ApplyScavenge(target as IScavengeable);
                switch (foundItem)
                {
                    case "Treasure":
                        AddActionLog("I scavenged the surrounding treasure chests. I got the treasure.");
                        ShowLine(RequestLineType.GetTreasureSuccess);
                        AddGameLog($"{DisplayName}が宝物を入手。");
                        TreasureCount++;
                        break;
                    case "Empty":
                        AddActionLog("I scavenged the surrounding boxes. There was nothing in them.");
                        ShowLine(RequestLineType.GetItemFailure);
                        break;
                    default:
                        AddActionLog($"I scavenged the surrounding boxes. I got the {foundItem}.");
                        ShowLine(RequestLineType.GetItemSuccess);
                        AddGameLog($"{DisplayName}がアイテムを入手。");
                        break;
                }
            }
            else
            {
                AddActionLog("There are no areas around that can be scavenged.");
            }
        }

        async UniTask TalkAsync(CancellationToken token)
        {
            const float PlayTime = 7.0f;

            token.ThrowIfCancellationRequested();

            if (TryGetTalkTarget(out Actor target))
            {
                await RotateToActorDirectionAsync(target, token);
                PlayAnimation("Talk");
                ShowLine(RequestLineType.Greeting);
                PlayTalkEffect();
                ApplyTalk(target as ITalkable);
                AddActionLog("I talked to the adventurers around me about what I knew.");
                await WaitForSecondsAsync(PlayTime, token);
            }
            else
            {
                AddActionLog("I tried to talk with other adventurers, but there was no one around.");
            }
        }

        async UniTask<bool> DefeatedAsync(CancellationToken token)
        {
            const float AnimationLength = 2.5f;

            if (IsAlive) return false;

            PlayDefeatedEffect();
            ShowLine(RequestLineType.Defeated);
            await WaitForSecondsAsync(AnimationLength, token);
            RemoveActorOnCell();

            return true;
        }

        async UniTask<bool> EscapeAsync(CancellationToken token)
        {
            const float AnimationLength = 1.0f * 2;

            if (IsLastSubGoal() && IsEntrancePlacedCell(Coords) && CurrentSubGoal.IsCompleted())
            {
                PlayAnimation("Jump");
                ShowLine(RequestLineType.Goal);
                AddGameLog($"{DisplayName}がダンジョンから脱出した。");

                await WaitForSecondsAsync(AnimationLength, token);
                RemoveActorOnCell();

                return true;
            }

            return false;
        }

        bool IsLastSubGoal()
        {
            return _currentSubGoalIndex == _subGoals.Length - 1;
        }

        bool IsEntrancePlacedCell(Vector2Int coords)
        {
            return Blueprint.Interaction[coords.y][coords.x] == '<';
        }

        void PathfindingToNeighbourCell(Vector2Int direction)
        {
            Cell cell = GetCell(GetNeighbourCoords(direction));
            CreatePathManually(direction.ToString(), cell);
        }

        void PathfindingIfTargetChanged(string target)
        {
            if (_pathTarget == target) return;

            switch (target)
            {
                case "Treasure": PathfindingToTreasure(); break;
                case "Entrance": PathfindingToEntrance(); break;
                default: Debug.LogWarning($"対応する目標が存在しないため経路探索が出来ない。: {target}"); break;
            }
        }

        async UniTask MoveNextCellAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            SetDirectionToNextCell();
            OpenForwardDoor();

            string directionName = GetDirectionName();

            if (IsNextCellPassable())
            {
                PlayAnimation("Walk");
                RemoveActorOnCell();
                SetCoordsToNextCell();
                AddActorOnCell();
                
                await (TranslateAsync(token), RotateToNextCellDirectionAsync(token));
                
                PlayAnimation("Idle");
                UpdateNextCell();
                AddActionLog($"Successfully moved to the {directionName}.");
                UpdateExploreRecord(Coords);
            }
            else
            {
                await RotateToNextCellDirectionAsync(token);

                UpdateNextCell();
                AddActionLog($"Failed to move to the {directionName}. Cannot move in this direction.");
            }
        }

        bool TryGetAttackTarget(out Actor target)
        {
            return TryGetTarget<IDamageable>(out target);
        }

        bool TryGetScavengeTarget(out Actor target)
        {
            // 宝箱を優先して漁る。
            if (TryGetTarget("Treasure", out target)) return true;
            if (TryGetTarget<IScavengeable>(out target)) return true;

            target = null;
            return false;
        }

        bool TryGetTalkTarget(out Actor target)
        {
            return TryGetTarget<ITalkable>(out target);
        }

        string ApplyDamage(IDamageable target)
        {
            if (target == null) return string.Empty;

            // まだ武器と攻撃力のデータを作っていない。
            return target.Damage(ID, "パンチ", 34, Coords);
        }

        string ApplyScavenge(IScavengeable target)
        {
            if (target == null) return string.Empty;

            Item result = target.Scavenge();
            if (result != null)
            {
                AddItemInventory(result);
                return result.Name.English;
            }
            else
            {
                return "Empty";
            }
        }

        void ApplyTalk(ITalkable target)
        {
            if (target == null || _selectedInformation == null) return;

            target.Talk(_selectedInformation.Text, "Adventurer", Coords);
        }

        void PathfindingToTreasure()
        {
            Actor treasure = GetAnyCell("Treasure").GetActors().Where(a => a.ID == "Treasure").FirstOrDefault();
            // 宝箱のマスへは経路探索が出来ないので、正面の位置までの経路探索。
            Pathfinding("Treasure", treasure.Coords + treasure.Direction);
        }

        void PathfindingToEntrance()
        {
            Cell cell = GetAnyCell("Entrance");
            Pathfinding("Entrance", cell.Coords);
        }

        void SetDirectionToNextCell()
        {
            _currentDirection = _path[_currentPathIndex].Coords - Coords;
        }

        void OpenForwardDoor()
        {
            Vector2Int forwardCoords = GetNeighbourCoords(Direction);
            if (IsDoorPlacedCell(forwardCoords) && TryGetDoorOnCell(forwardCoords, out DungeonEntity door))
            {
                door.Interact(this);
            }
        }

        bool IsNextCellPassable()
        {
            return _path[_currentPathIndex].IsPassable();
        }

        async UniTask TranslateAsync(CancellationToken token)
        {
            const float Speed = 1.0f;

            token.ThrowIfCancellationRequested();

            Vector3 start = GetPosition();
            Vector3 goal = GetNextCellPosition();
            for (float t = 0; t <= 1; t += Time.deltaTime * Speed)
            {
                SetPosition(Vector3.Lerp(start, goal, t));
                await UniTask.Yield(cancellationToken: token);
            }

            SetPosition(goal);
        }

        async UniTask RotateToActorDirectionAsync(Actor actor, CancellationToken token)
        {
            await RotateAsync(GetRotationToTarget(actor.Coords), token);
        }

        async UniTask RotateToNextCellDirectionAsync(CancellationToken token)
        {
            await RotateAsync(GetRotationToNextCell(), token);
        }

        async UniTask RotateAsync(Quaternion goal, CancellationToken token)
        {
            const float Speed = 4.0f;

            token.ThrowIfCancellationRequested();

            Quaternion start = GetRotation();
            for (float t = 0; t <= 1; t += Time.deltaTime * Speed)
            {
                SetRotation(Quaternion.Lerp(start, goal, t));
                await UniTask.Yield(cancellationToken: token);
            }
        }

        void AddActorOnCell()
        {
            _dungeonManager.AddActorOnCell(Coords, this);
        }

        void RemoveActorOnCell()
        {
            _dungeonManager.RemoveActorOnCell(Coords, this);
        }

        void SetCoordsToNextCell()
        {
            _currentCoords = _path[_currentPathIndex].Coords;
        }

        void UpdateNextCell()
        {
            _currentPathIndex++;
            _currentPathIndex = Mathf.Min(_currentPathIndex, _path.Count - 1);
        }

        string GetDirectionName()
        {
            if (Direction == Vector2Int.up) return "north";
            if (Direction == Vector2Int.down) return "south";
            if (Direction == Vector2Int.left) return "west";
            if (Direction == Vector2Int.right) return "east";

            return string.Empty;
        }

        Vector3 GetPosition()
        {
            return transform.position;
        }

        Vector3 GetNextCellPosition()
        {
            return _path[_currentPathIndex].Position;
        }

        Quaternion GetRotation()
        {
            return _forwardAxis.rotation;
        }

        void SetRotation(Quaternion rotation)
        {
            _forwardAxis.rotation = rotation;
        }

        Quaternion GetRotationToNextCell()
        {
            return CalculateRotation(GetPosition(), GetNextCellPosition());
        }

        Quaternion GetRotationToTarget(Vector2Int targetCoords)
        {
            Cell a = GetCell(Coords);
            Cell b = GetCell(targetCoords);
            return CalculateRotation(a.Position, b.Position);
        }

        static Quaternion CalculateRotation(Vector3 a, Vector3 b)
        {
            Vector3 dir = (b - a).normalized;
            if (dir == Vector3.zero) return Quaternion.identity;
            else return Quaternion.LookRotation(dir);
        }

        bool TryGetTarget<T>(out Actor target)
        {
            foreach (Actor actor in GetNeighboursActors())
            {
                if (actor is T _)
                {
                    target = actor;
                    return true;
                }
            }

            target = null;
            return false;
        }

        bool TryGetTarget(string id, out Actor target)
        {
            foreach (Actor actor in GetNeighboursActors())
            {
                if (actor.ID == id)
                {
                    target = actor;
                    return true;
                }
            }

            target = null;
            return false;
        }

        IEnumerable<Actor> GetNeighboursActors()
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int k = -1; k <= 1; k++)
                {
                    // 上下左右の4方向のみ。
                    if ((i == 0 && k == 0) || Mathf.Abs(i * k) > 0) continue;

                    Cell cell = GetCell(GetNeighbourCoords(new Vector2Int(k, i)));
                    if (cell.GetActors().Count > 0)
                    {
                        foreach (Actor actor in cell.GetActors()) yield return actor;
                    }
                }
            }
        }

        void RegisterStatusBar()
        {
            _statusBarID = _uiManager.RegisterToStatusBar(this);
        }

        void RegisterProfileWindow()
        {
            _profileWindowID = _uiManager.RegisterToProfileWindow(this);
        }

        void UpdateStatusBar()
        {
            _uiManager.UpdateStatusBarStatus(_statusBarID, this);
        }

        void UpdateProfileWindow()
        {
            _uiManager.UpdateProfileWindowStatus(_profileWindowID, this);
        }

        void DeleteStatusBar()
        {
            if (_uiManager == null) return;

            _uiManager.DeleteStatusBarStatus(_statusBarID);
        }

        void DeleteProfileWindow()
        {
            if (_uiManager == null) return;

            _uiManager.DeleteProfileWindowStatus(_profileWindowID);
        }

        void AddPendingInformation(BilingualString text, string source)
        {
            SharedInformation info = new SharedInformation(text, source);
            _pendingInformation.Enqueue(info);
        }

        void RefreshHoldedInformation()
        {
            _holdedInformation.DecreaseRemainingTurn();
            _holdedInformation.RemoveExpired();
        }

        void AddItemInventory(Item item)
        {
            _inventory.Add(item);
        }

        Cell GetAnyCell(string id)
        {
            return _dungeonManager.GetCells(id).FirstOrDefault();
        }

        void SetPosition(Vector2Int coords)
        {
            Cell cell = _dungeonManager.GetCell(coords);
            SetPosition(cell.Position);
        }

        void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        void ShowLine(RequestLineType type)
        {
            ShowLineAsync(type, this.GetCancellationTokenOnDestroy()).Forget();
        }

        async UniTask ShowLineAsync(RequestLineType type, CancellationToken token)
        {
            string line = await _rolePlayAI.RequestLineAsync(type, token);
            _uiManager.ShowLine(_statusBarID, line);
        }

        static bool IsDoorPlacedCell(Vector2Int coords)
        {
            // ダンジョン生成時、ドアを生成するセルは 上(8),下(2),左(4),右(6) で向きを指定している。
            return "2468".Contains(Blueprint.Doors[coords.y][coords.x]);
        }

        bool TryGetDoorOnCell(Vector2Int coords, out DungeonEntity door)
        {
            Actor actor = GetCell(coords).GetActors().Where(c => c.ID == "Door").FirstOrDefault();
            door = actor as DungeonEntity;
            return door != null;
        }

        Vector2Int GetNeighbourCoords(Vector2Int direction)
        {
            // 隣接していないセルには移動しないようにする。
            int x = System.Math.Sign(direction.x);
            int y = System.Math.Sign(direction.y);
            return Coords + new Vector2Int(x, y);
        }

        void Pathfinding(string target, Vector2Int targetCoords)
        {
            _dungeonManager.Pathfinding(Coords, targetCoords, _path);
            _pathTarget = target;
            _currentPathIndex = 0;
        }

        void CreatePathManually(string target, params Cell[] cells)
        {
            _path.Clear();
            _path.AddRange(cells);
            _pathTarget = target;
            _currentPathIndex = 0;
        }

        void IncreaseHp(int value)
        {
            DecreaseHp(-value);
        }

        void DecreaseHp(int value)
        {
            CurrentHp -= value;
            CurrentHp = Mathf.Max(0, CurrentHp);
        }

        void AddActionLog(string text)
        {
            ActionLog.Enqueue($"Turn{ElapsedTurn}: {text}");

            // 次の行動を選択するAIにリクエストすることを考慮して、適当に手動で設定。
            if (ActionLog.Count > 10) ActionLog.Dequeue();
        }

        void AddGameLog(string text)
        {
            _uiManager.AddLog(text);
        }

        void PlayAnimation(string name)
        {
            _animator.Play(name);
        }

        Cell GetCell(Vector2Int coords)
        {
            return _dungeonManager.GetCell(coords);
        }

        void UpdateExploreRecord(Vector2Int coords)
        {
            ExploreRecord.IncreaseCount(coords);
        }

        void AddAvailableActions(IEnumerable<string> actions)
        {
            AvailableActions.AddRange(actions);
        }

        void AddTerrainFeatureInformation()
        {
            if (_dungeonManager.TryGetTerrainFeature(Coords, out SharedInformation feature))
            {
                // 情報を整理する際に消されにくくするため、AI側が評価する際の最大スコアである1に設定しておく。
                // 現在いるセルの情報をAIに渡す目的なので、残りターンは1にしておく。
                feature.Score = 1.0f;
                feature.RemainingTurn = 1;
                _holdedInformation.Add(feature);
            }
        }

        void RegisterCameraFocusTarget()
        {
            _cameraFocusID = _uiManager.RegisterCameraFocusTarget(gameObject);
        }

        void DeleteCameraFocusTarget()
        {
            _uiManager.DeleteCameraFocusTarget(_cameraFocusID);
        }

        void SetNameTag()
        {
            if (this.TryGetComponentInChildren(out NameTag nameTag))
            {
                nameTag.SetName(DisplayName);
            }
        }

        void PlayEntryEffect()
        {
            if (TryGetComponent(out EntryEffect effect))
            {
                effect.Play();
            }
        }

        void PlayDamageEffect(Vector2Int attackerCoords)
        {
            if (TryGetComponent(out DamageEffect effect))
            {
                effect.Play(Coords, attackerCoords);
            }
        }

        void PlayDefeatedEffect()
        {
            if (TryGetComponent(out DefeatedEffect effect))
            {
                effect.Play();
            }
        }

        void PlayTalkEffect()
        {
            if (TryGetComponent(out TalkEffect effect))
            {
                effect.Play();
            }
        }

        async UniTask WaitForSecondsAsync(float duration, CancellationToken token)
        {
            await UniTask.WaitForSeconds(duration, cancellationToken: token);
        }
    }
}