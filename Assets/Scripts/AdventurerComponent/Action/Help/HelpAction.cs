using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using VTNConnect;

namespace Game
{
    public class HelpAction : SurroundingAction
    {
        Adventurer _adventurer;

        void Awake()
        {
            _adventurer = GetComponent<Adventurer>();
        }

        public async UniTask<ActionResult> PlayAsync(CancellationToken token)
        {
            _adventurer.Reboot();

            GameLog.Add(
                "�V�X�e��",
                $"�󋵂𐮗������B",
                LogColor.White,
                _adventurer.Sheet.DisplayID
            );

            // �G�s�\�[�h�𑗐M�B
            GameEpisode episode = new GameEpisode(
                EpisodeCode.VCMainItem,
                _adventurer.Sheet.UserId
            );
            episode.SetEpisode("���𐮗�����");
            VantanConnect.SendEpisode(episode);

            // �A�j���[�V�������̉��o��҂����B
            await UniTask.Yield(cancellationToken: token);

            return new ActionResult(
                "Help",
                "Success",
                "Help me!",
                _adventurer.Coords,
                _adventurer.Direction
            );
        }
    }
}
