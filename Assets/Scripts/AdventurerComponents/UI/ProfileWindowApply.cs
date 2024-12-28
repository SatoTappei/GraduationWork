using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    // ProfileWindowのEffectの欄に表示できる。
    // ステータス効果が有効な場合、効果の概要を表示する。
    public interface IStatusEffectDisplayable
    {
        bool IsEnabled();
        string GetEntry();
    }

    public class ProfileWindowApply : MonoBehaviour, IProfileWindowDisplayStatus
    {
        Blackboard _blackboard;
        SubGoalPath _subGoalPath;
        ItemInventory _itemInventory;
        InformationStock _informationStock;
        ProfileWindow _profileWindow;
        int _id;

        string IProfileWindowDisplayStatus.FullName => _blackboard.FullName;
        string IProfileWindowDisplayStatus.Job => _blackboard.Job;
        string IProfileWindowDisplayStatus.Background => _blackboard.Background;
        SubGoal IProfileWindowDisplayStatus.CurrentSubGoal => _subGoalPath.Current;
        IEnumerable<ItemInventory.Entry> IProfileWindowDisplayStatus.Item => _itemInventory.GetEntries();
        IEnumerable<string> IProfileWindowDisplayStatus.Effect => GetEnabledStatusEffects();
        IReadOnlyList<Information> IProfileWindowDisplayStatus.Information => _informationStock.Stock;

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
        }

        public void Apply()
        {
            _profileWindow.UpdateStatus(_id, this);
        }

        void OnDestroy()
        {
            if (_profileWindow != null)
            {
                _profileWindow.DeleteStatus(_id);
            }
        }

        IEnumerable<string> GetEnabledStatusEffects()
        {
            IStatusEffectDisplayable[] statusEffects = GetComponents<IStatusEffectDisplayable>();
            foreach (IStatusEffectDisplayable e in statusEffects)
            {
                if (e.IsEnabled()) yield return e.GetEntry();
            }
        }
    }
}
