using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LoadingScreen : MonoBehaviour
{
    private UIDocument _uiDocument;
    private Label _labelContinue;
    private ProgressBar _loadingBar;

    private void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();
        _labelContinue = _uiDocument.rootVisualElement.Q<Label>("labelCompletLoading");
        _loadingBar = _uiDocument.rootVisualElement.Q<ProgressBar>("LoadingBar");
    }

    void Start()
    {
        Time.timeScale = 1;
        StartCoroutine(LoadSceneCoroutine(StaticSceneLoader.sceneToLoad));
    }

    private IEnumerator LoadSceneCoroutine(string sceneToLoad)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);

        if (operation == null)
        {
            Debug.LogError("Scene to load not founded");
            yield break;
        }
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            _loadingBar.value = (operation.progress * 100) + 10;
            _loadingBar.title = _loadingBar.value + "%";

            if (operation.progress >= 0.9f)
            {
                _labelContinue.style.display = DisplayStyle.Flex;

                if (Input.anyKeyDown)
                {
                    operation.allowSceneActivation = true;
                }
            }
            yield return null;
        }
    }
}
