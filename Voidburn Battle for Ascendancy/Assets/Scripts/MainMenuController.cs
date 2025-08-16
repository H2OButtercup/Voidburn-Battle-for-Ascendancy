using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private MatchConfig match;

    [SerializeField] private string characterSelectionScene = "CharacterSelect";
    [SerializeField] private string optionsScene = "Options";

    public void OnPlayerVsCPU()
    {
        match.Mode = GameMode.PlayerVsCPU;
        SceneManager.LoadScene(characterSelectionScene);
    }

    public void OnPlayerVsPlayer()
    {
        match.Mode = GameMode.PlayerVsPlayer;
        SceneManager.LoadScene(characterSelectionScene);
    }

    public void OnOptions()
    {
        SceneManager.LoadScene(optionsScene);
    }

    public void OnQuit()
    {
        Application.Quit();
    }
}
