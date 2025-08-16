using UnityEngine;

public enum GameMode { PlayerVsCPU, PlayerVsPlayer }

[CreateAssetMenu(fileName = "MatchConfig", menuName = "Game/MatchConfig")]
public class MatchConfig : ScriptableObject
{
    [SerializeField] private CharacterDefinition playerChar;
    [SerializeField] private CharacterDefinition cpuChar;
    [SerializeField] private GameMode gameMode = GameMode.PlayerVsCPU;

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

    public GameMode Mode
    {
        get => gameMode;
        set => gameMode = value;
    }
}
