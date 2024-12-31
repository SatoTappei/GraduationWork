using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ProfileWindowApply : MonoBehaviour, IProfileWindowDisplayable
    {
        Blackboard _blackboard;
        SubGoalPath _subGoalPath;
        ItemInventory _itemInventory;
        InformationStock _informationStock;
        ProfileWindow _profileWindow;
        int _id;
        bool _isRegistered;

        string IProfileWindowDisplayable.FullName => _blackboard.FullName;
        string IProfileWindowDisplayable.Job => _blackboard.Job;
        string IProfileWindowDisplayable.Background => _blackboard.Background;
        SubGoal IProfileWindowDisplayable.CurrentSubGoal => _subGoalPath.GetCurrent();
        IReadOnlyList<Information> IProfileWindowDisplayable.Information => _informationStock.Stock;
        IEnumerable<ItemInventory.Entry> IProfileWindowDisplayable.Item => _itemInventory.GetEntries();
        IEnumerable<string> IProfileWindowDisplayable.Effect => _blackboard.StatusEffects;

        void Awake()
        {
            _blackboard = GetComponent<Blackboard>();
            _subGoalPath = GetComponent<SubGoalPath>();
            _itemInventory = GetComponent<ItemInventory>();
            _informationStock = GetComponent<InformationStock>();
            ProfileWindow.TryFind(out _profileWindow);
        }

        public void Register()
        {
            _id = _profileWindow.RegisterStatus(this);
            _isRegistered = true;
        }

        public void Apply()
        {
            if (_isRegistered)
            {
                _profileWindow.UpdateStatus(_id, this);
            }
            else
            {
                Debug.LogWarning("ìoò^ÇµÇƒÇ¢Ç»Ç¢èÛë‘Ç≈îΩâfÇµÇÊÇ§Ç∆ÇµÇΩÅB");
            }
        }

        void OnDestroy()
        {
            if (_profileWindow != null) _profileWindow.DeleteStatus(_id);

            _isRegistered = false;
        }
    }
}
