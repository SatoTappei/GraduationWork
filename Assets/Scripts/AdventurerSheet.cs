using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AdventurerSheet_", menuName = "AdventurerSheet")]
public class AdventurerSheet : ScriptableObject
{
    [SerializeField] Sprite _icon;
    [SerializeField] string _fullName;
    [SerializeField] string _displayName;
    [SerializeField] string _sex;
    [SerializeField] string _job;
    [SerializeField] int _age;
    [TextArea]
    [SerializeField] string _background;
    [SerializeField] string _lineSample1;
    [SerializeField] string _lineSample2;
    [SerializeField] string _lineSample3;
    [SerializeField] int _strength;
    [SerializeField] int _inteligence;
    [SerializeField] int _charm;
    [SerializeField] int _dexterity;
    [SerializeField] int _sensitivity;

    public Sprite Icon => _icon;
    public string FullName => _fullName;
    public string DisplayName => _displayName;
    public string Sex => _sex;
    public string Job => _job;
    public int Age => _age;
    public string Background => _background;
    public string LineSample1 => _lineSample1;
    public string LineSample2 => _lineSample2;
    public string LineSample3 => _lineSample3;
    public int Strength => _strength;
    public int Inteligence => _inteligence; 
    public int Charm => _charm;
    public int Dexterity => _dexterity;
    public int Sensitivity => _sensitivity;
}
