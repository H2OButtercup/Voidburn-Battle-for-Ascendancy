using System.Linq;
using UnityEngine;

public class ButtonFunctions : MonoBehaviour
{
    void Resume()
    {
        gameManager.instance.stateUnpause();
    }

    void Exit()
    {

    }

    void Settings()
    {
        gameManager.instance.menuScenes.Add(gameManager.instance.menuSettings); 
        gameManager.instance.menuActive = gameManager.instance.menuScenes.Last();
    }

    void Aplly()
    {
        gameManager.instance.applySettings();
    }

    void MoveList()
    {
        gameManager.instance.menuScenes.Add(gameManager.instance.menuMoveList);
        gameManager.instance.menuActive = gameManager.instance.menuScenes.Last();
    }

    void Close()
    {
        gameManager.instance.menuScenes.Remove(gameManager.instance.menuScenes.Last());
        gameManager.instance.menuActive = gameManager.instance.menuScenes.Last();
    }
}
