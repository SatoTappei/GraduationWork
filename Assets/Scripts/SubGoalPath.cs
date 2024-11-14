using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Game
{
    public class SubGoalPath
    {
        IReadOnlyAdventurerContext _context;
        SubGoalPlannerAI _ai;
        SubGoal[] _path;
        int _currentIndex;

        public SubGoalPath(IReadOnlyAdventurerContext context)
        {
            _context = context;
            _ai = new SubGoalPlannerAI(context);
        }

        public SubGoal Current
        {
            get
            {
                if (_path == null) return null;
                if (_currentIndex < 0 || _path.Length <= _currentIndex) return null;

                return _path[_currentIndex];
            }
        }

        public bool IsLastSubGoal => _currentIndex == _path.Length - 1;

        public async UniTask PlanningAsync(CancellationToken token)
        {
            IReadOnlyList<string> result = await _ai.RequestAsync(token);

            _path = new SubGoal[result.Count];
            for (int i = 0; i < result.Count; i++)
            {
                _path[i] = Convert(result[i]);
            }

            _currentIndex = 0;
        }

        public void HeadingNext()
        {
            _currentIndex++;
            _currentIndex = Mathf.Min(_currentIndex, _path.Length - 1);
        }

        // AIは日本語の文字列で選択したサブゴールを返すので、対応するクラスのインスタンスに変換する。
        SubGoal Convert(string text)
        {
            if (text == GetTreasure.StaticText.Japanese) return new GetTreasure(_context);
            if (text == GetRequestedItem.StaticText.Japanese) return new GetRequestedItem(_context);
            if (text == ExploreDungeon.StaticText.Japanese) return new ExploreDungeon(_context);
            if (text == DefeatWeakEnemy.StaticText.Japanese) return new DefeatWeakEnemy(_context);
            if (text == DefeatStrongEnemy.StaticText.Japanese) return new DefeatStrongEnemy(_context);
            if (text == DefeatAdventurer.StaticText.Japanese) return new DefeatAdventurer(_context);
            if (text == ReturnToEntrance.StaticText.Japanese) return new ReturnToEntrance(_context);

            Debug.LogError($"AIが選択したサブゴールに対応するクラスが無い。: {text}");

            return null;
        }
    }
}
