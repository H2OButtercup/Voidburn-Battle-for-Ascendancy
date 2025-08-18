using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CharacterSelectController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private MatchConfig match;
    [SerializeField] private CharacterDefinition[] available;

    [Header("UI")]
    [SerializeField] private Transform playerGrid;
    [SerializeField] private Transform cpuGrid;
    [SerializeField] private Button startButton;
    [SerializeField] private Button characterButtonPrefab;

    private CharacterDefinition _playerPick;
    private CharacterDefinition _cpuPick;

    private void Start()
    {
        if (available == null || available.Length == 0)
            available = Resources.LoadAll<CharacterDefinition>("Characters");

        BuildGrid(playerGrid, true);
        BuildGrid(cpuGrid, false);

        startButton.onClick.AddListener(OnStartMatch);
    }

    private void BuildGrid(Transform grid, bool isPlayer)
    {
        foreach (Transform c in grid) Destroy(c.gameObject);

        foreach (var def in available)
        {
            var btn = Instantiate(characterButtonPrefab, grid);

            
            var label = btn.GetComponentInChildren<TMP_Text>();
            if (label != null) label.text = def.DisplayName;

            var img = btn.GetComponentInChildren<Image>();
            if (img != null) img.sprite = def.Portrait;

            var picked = def;
            btn.onClick.AddListener(() =>
            {
                if (isPlayer) _playerPick = picked;
                else _cpuPick = picked;
            });
        }
    }

    private void OnStartMatch()
    {
        match.PlayerChar = _playerPick ?? available.FirstOrDefault();
        match.CPUChar = _cpuPick ?? available.Skip(1).FirstOrDefault() ?? available.FirstOrDefault();

        SceneManager.LoadScene("Arena");
    }
}


