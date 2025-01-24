using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class NameTag : MonoBehaviour
    {
        [SerializeField] NameTagUI[] _nameTagUI;

        public static NameTag Find()
        {
            return GameObject.FindGameObjectWithTag("UiManager").GetComponent<NameTag>();
        }

        public void Register(Adventurer adventurer)
        {
            Debug.Log(adventurer.AdventurerSheet.Number);

            foreach (NameTagUI ui in _nameTagUI)
            {
                ui.Add(adventurer);
            }
        }

        public void Delete(Adventurer adventurer)
        {
            foreach (NameTagUI ui in _nameTagUI)
            {
                ui.Remove(adventurer);
            }
        }
    }
}
