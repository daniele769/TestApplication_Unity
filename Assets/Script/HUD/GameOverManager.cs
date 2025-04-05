using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameOverManager : AbstractUIControl
{
    private Button _retryButton;
    private Button _mainMenuButton;
    private AudioSource _audioSource;

    [SerializeField] 
    private PlayerInput _playerInput;
    
    void Start()
    {
        InitializeElements();
        InitializeNavPath();

        _audioSource = GetComponent<AudioSource>();

        _mainMenuButton.clicked += () => { SceneManager.LoadScene(Constants.MainMenuScene); };
        _retryButton.clicked += () => { SceneManager.LoadScene(Constants.SceneLoader); };

        defaultFocus = _retryButton;

        rootElement.style.display = DisplayStyle.None;
        HealthManager.Instance.OnDeath += GameOverIsVisible;
    }

    private void GameOverIsVisible()
    {
        rootElement.style.display = DisplayStyle.Flex;
        _audioSource.Play();
    }

    protected override void InitializeElements()
    {
        _retryButton = rootElement.Q<Button>("RetryButton");
        _mainMenuButton = rootElement.Q<Button>("MainMenuButton");
    }

    protected override void InitializeNavPath()
    {
        SetupNavPath(_retryButton,  null, _mainMenuButton);
        SetupNavPath(_mainMenuButton,  _retryButton, null);
    }
}
