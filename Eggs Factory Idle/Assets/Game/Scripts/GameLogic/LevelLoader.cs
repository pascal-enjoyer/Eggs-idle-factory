// LevelLoader.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public static string levelSceneName = "Level";
    public static string mainMenuName = "MainMenu";

    public void LoadLevel()
    {
        if (string.IsNullOrEmpty(levelSceneName))
        {
            return;
        }

        SceneManager.LoadScene(levelSceneName);
    }
    
    public void CloseGame()
    {
        Application.Quit();
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuName);
    }
}
