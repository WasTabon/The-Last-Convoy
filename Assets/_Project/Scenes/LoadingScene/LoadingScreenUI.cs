using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingScreenUI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string _sceneToLoad;
    [SerializeField] private bool _loadOnStart = true;

    [Header("UI")]
    [SerializeField] private Slider _progressBar;
    [SerializeField] private Text _progressText;

    private AsyncOperation _loadOperation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1f;

        if (_loadOnStart)
        {
            StartLoading();
        }
    }

    public void StartLoading()
    {
        if (string.IsNullOrEmpty(_sceneToLoad))
        {
            Debug.LogError("[LoadingScreenUI] Scene To Load is not set!");
            return;
        }

        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        _loadOperation = SceneManager.LoadSceneAsync(_sceneToLoad);
        _loadOperation.allowSceneActivation = false;

        while (_loadOperation.progress < 0.9f)
        {
            UpdateProgress(_loadOperation.progress / 0.9f);
            yield return null;
        }

        UpdateProgress(1f);

        yield return new WaitForSeconds(0.5f);

        _loadOperation.allowSceneActivation = true;
    }

    private void UpdateProgress(float progress)
    {
        if (_progressBar != null)
        {
            _progressBar.value = progress;
        }

        if (_progressText != null)
        {
            _progressText.text = $"{(progress * 100f):F0}%";
        }
    }
}
