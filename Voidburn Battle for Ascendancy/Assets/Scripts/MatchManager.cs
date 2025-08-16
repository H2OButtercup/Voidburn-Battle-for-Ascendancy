using UnityEngine;
using UnityEngine.Rendering;

public class MatchManager : MonoBehaviour
{
    [SerializeField] private MatchConfig match;
    [SerializeField] private Transform p1Spawn;
    [SerializeField] private Transform cpuOrP2Spawn;

    [Header("Optional input for Player 2")]
    [SerializeField] private KeyCode p2Left = KeyCode.LeftArrow;
    [SerializeField] private KeyCode p2Right = KeyCode.RightArrow;
    [SerializeField] private KeyCode p2Jump = KeyCode.RightShift;
    [SerializeField] private KeyCode p2Light = KeyCode.Keypad1;
    [SerializeField] private KeyCode p2Heavy = KeyCode.Keypad2;

    private void Start()
    {
        if (!match || !match.PlayerChar || !match.CPUChar)
        {
            Debug.LogWarning("MatchConfig not set. Did you come from CharacterSelect?");
            return;
        }

        var p1 = Instantiate(match.PlayerChar.Prefab, p1Spawn.position, Quaternion.identity);
        var p2 = Instantiate(match.CPUChar.Prefab, cpuOrP2Spawn.position, Quaternion.identity);

        var p1Stats = p1.GetComponent<CharacterStats>();
        var p2Stats = p2.GetComponent<CharacterStats>();
        if (p1Stats) p1Stats.Apply(match.PlayerChar);
        if (p2Stats) p2Stats.Apply(match.CPUChar);

        var p1Ctrl = p1.GetComponent<FighterController>();
        var p2Ctrl = p2.GetComponent<FighterController>();

        if (match.Mode == GameMode.PlayerVsCPU)
        {
            if (p2Ctrl) p2Ctrl.SetCPU(true);
        }
        else
        {
            if (p2Ctrl)
            {
                p2Ctrl.SetCPU(false);

                p2Ctrl.SetKeys(p2Left, p2Right, p2Jump, p2Light, p2Heavy);
            }
        }
    }
}