using UnityEngine;

public class MatchManager : MonoBehaviour
{
    [SerializeField] private MatchConfig match;
    [SerializeField] private Transform p1Spawn;
    [SerializeField] private Transform cpuSpawn;

    private void Start()
    {
        if (!match || !match.PlayerChar || !match.CPUChar)
        {
            Debug.LogWarning("MatchConfig not set. Did you come from CharacterSelect?");
            return;
        }

        var p1 = Instantiate(match.PlayerChar.Prefab, p1Spawn.position, Quaternion.identity);
        var cpu = Instantiate(match.CPUChar.Prefab, cpuSpawn.position, Quaternion.identity);

        var p1Stats = p1.GetComponent<CharacterStats>();
        var cpuStats = cpu.GetComponent<CharacterStats>();
        if (p1Stats) p1Stats.Apply(match.PlayerChar);
        if (cpuStats) cpuStats.Apply(match.CPUChar);

        var cpuCtrl = cpu.GetComponent<FighterController>();
        if (cpuCtrl) cpuCtrl.SetCPU(true);
    }
}