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
        UiManager _uiManager;
        int _id;

        string IProfileWindowDisplayStatus.FullName => _blackboard.FullName;
        string IProfileWindowDisplayStatus.Job => _blackboard.Job;
        string IProfileWindowDisplayStatus.Background => _blackboard.Background;
        SubGoal IProfileWindowDisplayStatus.CurrentSubGoal => _subGoalPath.Current;
        IEnumerable<ItemInventory.Entry> IProfileWindowDisplayStatus.Item => _itemInventory.GetEntries();
        IEnumerable<string> IProfileWindowDisplayStatus.Effect => GetEnabledStatusEffects();
        IReadOnlyList<SharedInformation> IProfileWindowDisplayStatus.Information => _informationStock.Stock;

        void Awake()
        {
            _blackboard = GetComponent<Blackboard>();
            _subGoalPath = GetComponent<SubGoalPath>();
            _itemInventory = GetComponent<ItemInventory>();
            _informationStock = GetComponent<InformationStock>();
            UiManager.TryFind(out _uiManager);
        }

        public void Register()
        {
            _id = _uiManager.RegisterToProfileWindow(this);
        }

        public void Apply()
        {
            _uiManager.UpdateProfileWindowStatus(_id, this);
        }

        void OnDestroy()
        {
            if (_uiManager != null)
            {
                _uiManager.DeleteProfileWindowStatus(_id);
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
