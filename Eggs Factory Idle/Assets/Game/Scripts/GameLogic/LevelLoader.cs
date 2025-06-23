// LevelLoader.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private string levelSceneName = "GameLevel";

    public void LoadLevel()
    {
        if (string.IsNullOrEmpty(levelSceneName))
        {
            Debug.LogError("Level scene name is not set in the inspector.");
            return;
        }

        SceneManager.LoadScene(levelSceneName);
    }
}
