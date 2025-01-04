using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Game
{
    public class ProfileWindowBinder : MonoBehaviour, IProfileWindowDisplayable
    {
        StatusEffect[] _statusEffects;
        Adventurer _adventurer;
        SubGoalPath _subGoalPath;
        ItemInventory _itemInventory;
        HoldInformation _information;
        ProfileWindow _profileWindow;
        int _id;
        bool _isRegistered;

        string IProfileWindowDisplayable.FullName
        {
            get => _adventurer.AdventurerSheet.FullName;
        }

        string IProfileWindowDisplayable.Job
        {
            get => _adventurer.AdventurerSheet.Job;
        }
        
        string IProfileWindowDisplayable.Background
        {
            get => _adventurer.AdventurerSheet.Background;
        }
        
        SubGoal IProfileWindowDisplayable.CurrentSubGoal
        {
            get => _subGoalPath.GetCurrent();
        }
        
        IReadOnlyList<Information> IProfileWindowDisplayable.Information
        {
            get => _information.Information;
        }
        
        IEnumerable<ItemInventory.Entry> IProfileWindowDisplayable.Item
        {
            get => _itemInventory.GetEntries();
        }
        
        IEnumerable<string> IProfileWindowDisplayable.Effect
        {
            get => _statusEffects.Where(e => e.IsValid).Select(e => e.Description);
        }

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
            _subGoalPath = GetComponent<SubGoalPath>();
            _itemInventory = GetComponent<ItemInventory>();
            _information = GetComponent<HoldInformation>();
            _statusEffects = GetComponents<StatusEffect>();
            ProfileWindow.TryFind(out _profileWindow);
        }

        public void Register()
        {
            if (_isRegistered)
            {
                Debug.LogWarning("ä˘Ç…ìoò^çœÇ›ÅB");
            }
            else
            {
                _isRegistered = true;
                _id = _profileWindow.RegisterStatus(this);
            }
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
