using UnityEngine;

[CreateAssetMenu(fileName = "MatchConfig", menuName = "Scriptable Objects/MatchConfig")]
public class MatchConfig : ScriptableObject
{
    [SerializeField] private CharacterDefinition playerChar;
    [SerializeField] private CharacterDefinition cpuChar;

    public CharacterDefinition PlayerChar
    {
        get => playerChar;
        set => playerChar = value;
    }

    public CharacterDefinition CPUChar
    {
        get => cpuChar;
        set => cpuChar = value;
    }
}
