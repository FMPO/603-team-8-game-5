using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void GoToGameplayTest()
    {
        SceneManager.LoadScene("gameplayTesting");
    }

    public void GoToGameplay()
    {
        SceneManager.LoadScene("LevelOne");
    }
}
