using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AdventurerSheet_", menuName = "AdventurerSheet")]
public class AdventurerSheet : ScriptableObject
{
    [SerializeField] string _name;
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

    public string Name { get => _name; }
    public string Sex { get => _sex; }
    public string Job { get => _job; }
    public int Age { get => _age; }
    public string Background { get => _background; }
    public string LineSample1 {  get => _lineSample1; }
    public string LineSample2 {  get => _lineSample2; }
    public string LineSample3 { get => _lineSample3; }
    public int Strength { get => _strength; set => _strength = value; }
    public int Inteligence { get => _inteligence; set => _inteligence = value; }
    public int Charm { get => _charm; set => _charm = value; }
    public int Dexterity { get => _dexterity; set => _dexterity = value; }
    public int Sensitivity { get => _sensitivity; set => _sensitivity = value; }
}
