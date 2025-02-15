using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using VTNConnect;

namespace Game
{
    public class AttackAction : SurroundingAction
    {
        Adventurer _adventurer;
        Animator _animator;
        LineDisplayer _line;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
            _animator = GetComponentInChildren<Animator>();
            _line = GetComponent<LineDisplayer>();
        }

        public async UniTask<ActionResult> PlayAsync<TTarget>(CancellationToken token) where TTarget : class
        {
            // �V���A���C�Y���Ă��ǂ��B
            const float RotateSpeed = 4.0f;
            const float PlayTime = 2.0f;

            // �s���J�n�̃^�C�~���O�Ŏ��S���Ă����ꍇ�B
            if (_adventurer.Status.CurrentHp <= 0)
            {
                return new ActionResult(
                    "Attack",
                    "Miss",
                    $"Died.",
                    _adventurer.Coords,
                    _adventurer.Direction
                );
            }

            // ���͂ɍU���\�ȑΏۂ����Ȃ��ꍇ�B
            if (!TryGetTarget<TTarget>(out Actor target))
            {
                return new ActionResult(
                    "Attack",
                    "Miss",
                    "There are no enemies around to attack.",
                    _adventurer.Coords,
                    _adventurer.Direction
                );
            }

            // �U������O�ɖڕW�Ɍ����B
            Vector3 targetPosition = DungeonManager.GetCell(target.Coords).Position;
            await RotateAsync(RotateSpeed, targetPosition, token);

            _animator.Play("Attack");
            _line.Show(RequestLineType.Attack);

            // �_���[�W��^����B
            string result;
            if (target != null && target.TryGetComponent(out IDamageable damage))
            {
                // Defeat(���j����)�AHit(��������������)�ACorpse(���Ɏ���ł���)�AMiss(������Ȃ�����)
                result = damage.Damage(_adventurer.Status.TotalAttack, _adventurer.Coords);
            }
            else
            {
                result = "Miss";
            }

            // �U���̃A�j���[�V�����̍Đ��I����҂B
            // �U�����Ɏ��S�����ꍇ�͒��f����邪�A�U�����̂̓q�b�g����B
            for (float i = 0; i <= PlayTime; i += Time.deltaTime)
            {
                if (_adventurer.Status.CurrentHp <= 0) break;

                await UniTask.Yield(cancellationToken: token);
            }

            // ���j�����ꍇ�A�䎌�ƃ��O��\�����A���j�J�E���g�𑝂₷�B
            if (result == "Defeat")
            {
                _line.Show(RequestLineType.DefeatEnemy);

                GameLog.Add(
                    $"�V�X�e��", 
                    $"�G��|�����B",
                    LogColor.White,
                    _adventurer.Sheet.DisplayID
                );

                _adventurer.Status.DefeatCount++;
            }

            // �U�����ʂ��Ƃ̍s�����O�ɒǉ����镶�́B
            string actionLog;
            if (result == "Defeat")
            {
                actionLog = "I attacked the enemy. I defeated the enemy.";
            }
            else if (result == "Hit")
            {
                actionLog = "I attacked the enemy. The enemy is still alive.";
            }
            else if (result == "Corpse")
            {
                actionLog = "I tried to attack it, but it was already dead.";
            }
            else if (result == "Miss")
            {
                actionLog = "I tried to attack it, but it was miss.";
            }
            else
            {
                Debug.LogWarning($"�U�����ʂɑΉ����鏈���������B�X�y���~�X�H: {result}");
                
                actionLog = "I tried to attack it.";
            }

            // ���j�����ꍇ�̓C�x���g�ƃG�s�\�[�h�𑗐M�B
            if (result == "Defeat")
            {
                // �␂����C�x���g�B
                EventData eventData = new EventData(EventDefine.DefeatBomb);
                VantanConnect.SendEvent(eventData);

                // �`���Ҍ��j�G�s�\�[�h�B
                if (target.TryGetComponent(out Adventurer targetAdventurer))
                {
                    GameEpisode episode = new GameEpisode(
                        EpisodeCode.VCMainAttack,
                        _adventurer.Sheet.UserId
                    );
                    episode.SetEpisode("�`���҂�|����");
                    episode.DataPack("�|��������", targetAdventurer.Sheet.FullName);
                    VantanConnect.SendEpisode(episode);
                }
                
                // �G���j�G�s�\�[�h�B
                if (target.TryGetComponent(out Enemy targetEnemy))
                {
                    GameEpisode episode = new GameEpisode(
                        EpisodeCode.VCMainEnemy,
                        _adventurer.Sheet.UserId
                    );

                    if (targetEnemy.ID == nameof(Golem))
                    {
                        episode.SetEpisode("�_���W�����̃{�X��|����");
                    }
                    else
                    {
                        episode.SetEpisode("�G��|����");
                    }

                    VantanConnect.SendEpisode(episode);
                }
            }

            return new ActionResult(
                "Attack",
                result,
                actionLog,
                _adventurer.Coords,
                _adventurer.Direction
            );
        }
    }
}