using UnityEngine;

namespace Game
{
    public class FindItemGoal : SubGoal
    {
        BilingualString _description;
        Adventurer _adventurer;
        ItemInventory _itemInventory;

        State _confirmed;

        void Awake()
        {
            _description = new BilingualString(
                "依頼されたアイテムを手に入れる。", 
                "Scavenge containers and barrels to obtain items."
            );
            _adventurer = GetComponent<Adventurer>();
            _itemInventory = GetComponent<ItemInventory>();

            _confirmed = State.Running;
        }

        public override BilingualString Description => _description;

        public override State Check()
        {
            // 結果が確定した後に別の結果の条件を満たしたとしても、結果は覆らない。
            if (_confirmed == State.Completed || _confirmed == State.Failed)
            {
                return _confirmed;
            }

            // アイテムは自由に設定しても大丈夫。
            foreach (ItemInventory.Entry item in _itemInventory.GetEntries())
            {
                if (item.Name == "荷物")
                {
                    _confirmed = State.Completed;
                    return _confirmed;
                }
            }

            if (_adventurer.Status.ElapsedTurn > 100)
            {
                _confirmed = State.Failed;
                return _confirmed;
            }

            return State.Running;
        }
    }
}