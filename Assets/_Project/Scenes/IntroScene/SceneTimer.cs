using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTimer : MonoBehaviour
{
    public string nextSceneName;
    public float delay = 50f;

    private void Start()
    {
        Invoke(nameof(LoadNextScene), delay);
    }

    private void LoadNextScene()
    {
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogWarning("Next scene name is empty");
            return;
        }
        SceneManager.LoadScene(nextSceneName);
    }
}
