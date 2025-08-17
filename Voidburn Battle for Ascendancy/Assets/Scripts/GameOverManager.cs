using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private string gameOverScene = "Game Over";

    public void TriggerGameOver()
    {
        SceneManager.LoadScene(gameOverScene);
    }

    public void OnRetry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
