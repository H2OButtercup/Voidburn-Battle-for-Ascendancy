using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string retryScene = "Arena";
    [SerializeField] private string mainMenuScene = "Main Menu";

    public void SetRetryScene(string sceneName) => retryScene = sceneName;

    public void OnRetry()
    {
        if (string.IsNullOrEmpty(retryScene))
            retryScene = SceneManager.GetActiveScene().name;

        SceneManager.LoadScene(retryScene);
    }

    public void OnMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}
