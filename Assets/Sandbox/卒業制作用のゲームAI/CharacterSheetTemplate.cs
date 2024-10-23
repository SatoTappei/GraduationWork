using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSheetTemplate_", menuName = "CharacterSheetTemplate")]
public class CharacterSheetTemplate : ScriptableObject
{
    [SerializeField] CharacterSheet _value;

    public CharacterSheet Value => _value;
}