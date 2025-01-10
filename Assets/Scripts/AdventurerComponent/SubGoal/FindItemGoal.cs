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
                "�˗����ꂽ�A�C�e������ɓ����B", 
                "Scavenge containers and barrels to obtain items."
            );
            _adventurer = GetComponent<Adventurer>();
            _itemInventory = GetComponent<ItemInventory>();

            _confirmed = State.Running;
        }

        public override BilingualString Description => _description;

        public override State Check()
        {
            // ���ʂ��m�肵����ɕʂ̌��ʂ̏����𖞂������Ƃ��Ă��A���ʂ͕���Ȃ��B
            if (_confirmed == State.Completed || _confirmed == State.Failed)
            {
                return _confirmed;
            }

            // �A�C�e���͎��R�ɐݒ肵�Ă����v�B
            foreach (ItemInventory.Entry item in _itemInventory.GetEntries())
            {
                if (item.Name == "�ו�")
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