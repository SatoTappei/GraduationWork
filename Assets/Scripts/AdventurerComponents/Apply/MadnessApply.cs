using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class MadnessApply : MonoBehaviour
    {
        static BilingualString Text = new BilingualString(
            "©•ªˆÈŠO‚Ì–`Œ¯Ò‚ğŠFE‚µ‚É‚µ‚æ‚¤B", 
            "All adventurers other than yourself are enemies, so it is advisable to attack them aggressively."
        );

        InformationStock _informationStock;
        AvailableActions _availableActions;
        Blackboard _blackBoard;

        WaitForSeconds _waitInterval;
        bool _isEnabled;

        void Awake()
        {
            _informationStock = GetComponent<InformationStock>();
            _availableActions = GetComponent<AvailableActions>();
            _blackBoard = GetComponent<Blackboard>();
        }

        public void Apply()
        {
            if (_isEnabled) return;

            StartCoroutine(ApplyAsync());
        }

        IEnumerator ApplyAsync()
        {
            _isEnabled = true;
            _blackBoard.AddStatusEffect("“ª‚¨‚©‚µ‚¢B");

            // ˆê’èŠÔŠu‚ÅAI‚É–`Œ¯Ò‚ğUŒ‚‚·‚é‚æ‚¤‘£‚·î•ñ‚ğ’Ç‰ÁB‰ñ”‚ÆŠÔŠu‚Ì’l‚Í“K“–‚Éİ’èB
            for (int i = 0; i < 10; i++)
            {
                _informationStock.AddPending(Text, "Myself", isShared: false);
                //_availableActions.Remove("Scavenge Surrounding");
                //_availableActions.Remove("Talk Surrounding");
                // _availableActions.Add("Attack Surrounding Adventurer");

                yield return _waitInterval ??= new WaitForSeconds(3.0f);
            }

            //_availableActions.Add("Scavenge Surrounding");
            //_availableActions.Add("Talk Surrounding");
            //_availableActions.Remove("Attack Surrounding Adventurer");

            _blackBoard.RemoveStatusEffect("“ª‚¨‚©‚µ‚¢B");
            _isEnabled = false;
        }
    }
}
