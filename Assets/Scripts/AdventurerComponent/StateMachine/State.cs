using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game.FSM
{
    public abstract class State : MonoBehaviour
    {
        enum Stage { Enter, Stay, Exit }

        IReadOnlyDictionary<string, State> _states;
        State _next;
        Stage _stage;
        string _id = string.Empty;

        bool _isInitialized;

        public string ID
        {
            get
            {
                if (_id == string.Empty) _id = GetType().Name;
                return _id;
            }
        }

        public void Initialize(IReadOnlyDictionary<string, State> states)
        {
            _states = states;
            _next = this;
            _stage = Stage.Enter;

            _isInitialized = true;
        }

        public async UniTask<Result> UpdateAsync(CancellationToken token)
        {
            if (_stage == Stage.Enter) // �J�ڌ�A�ŏ���1�񂾂����s�����B
            {
                // ���������ς܂��Ă��邩�`�F�b�N�B
                if (!_isInitialized)
                {
                    Debug.LogWarning("���������Ă��Ȃ���ԂŎ��s�B");
                }

                string action = await EnterAsync(token);
                _stage = Stage.Stay;

                return new Result(this, action);
            }
            else if (_stage == Stage.Exit) // �J�ڂ��钼�O�A1�񂾂����s�����B
            {
                string action = await ExitAsync(token);
                _stage = Stage.Enter;

                // �J�ڐ悪�ݒ肳��Ă��邩�`�F�b�N�B
                if (_next == null)
                {
                    Debug.LogWarning("�J�ڐ悪���݂��Ȃ��B");
                }

                return new Result(_next, action);
            }
            else
            {
                string action = await StayAsync(token);
                return new Result(this, action);
            }
        }

        protected virtual async UniTask<string> EnterAsync(CancellationToken token)
        {
            await UniTask.Yield(cancellationToken: token);
            return string.Empty;
        }

        protected virtual async UniTask<string> StayAsync(CancellationToken token)
        {
            await UniTask.Yield(cancellationToken: token);
            return string.Empty;
        }

        protected virtual async UniTask<string> ExitAsync(CancellationToken token)
        {
            await UniTask.Yield(cancellationToken: token);
            return string.Empty;
        }

        protected void Transition(string next)
        {
            if (_stage == Stage.Enter)
            {
                Debug.LogWarning($"Enter���Ă΂��O�ɃX�e�[�g��J�ڂ��邱�Ƃ͕s�\�B�J�ڐ�:{next}");
            }
            else if (_stage == Stage.Exit)
            {
                Debug.LogWarning($"���ɕʂ̃X�e�[�g�ɑJ�ڂ��鏈�����Ă΂�Ă���B�J�ڐ�:{next}");
            }

            if (_states.ContainsKey(next))
            {
                _stage = Stage.Exit;
                _next = _states[next];
            }
            else
            {
                Debug.LogWarning("�J�ڐ悪�o�^����Ă��Ȃ��B");
            }
        }
    }
}
