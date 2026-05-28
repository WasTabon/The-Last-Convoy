using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private string _gameSceneName;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1f;
    }

    public void StartGame()
    {
        if (string.IsNullOrEmpty(_gameSceneName))
        {
            Debug.LogError("[MainMenuUI] Game Scene Name is not set!");
            return;
        }

        SceneManager.LoadScene(_gameSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
