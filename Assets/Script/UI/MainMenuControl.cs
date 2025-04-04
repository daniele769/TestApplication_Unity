using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuControl : AbstractUIControl
{
    private Button _playButton;
    private Button _optionsButton;
    private Button _closeGameButton;
    
    [SerializeField] 
    private UIControlRoot controlUiRoot;
    
    [SerializeField] 
    private AbstractUIControl options;
    
    void Start()
    {
        InitializeElements();
        InitializeNavPath();

        rootElement.style.display = DisplayStyle.None;
        options.uiDocument.rootVisualElement.style.display = DisplayStyle.None;
        
        defaultFocus = _playButton;
        _playButton.Focus();
        
        controlUiRoot.OpenRoot();
    }
    
    void Update()
    {
        
    }

    protected override void InitializeElements()
    {
        _playButton = rootElement.Q<Button>("PlayButton");
        _optionsButton = rootElement.Q<Button>("OptionsButton");
        _closeGameButton = rootElement.Q<Button>("CloseGameButton");

        _playButton.clicked += PlayGame;
        _optionsButton.clicked += OpenOptions;
        _closeGameButton.clicked += CloseGame;
    }

    protected override void InitializeNavPath()
    {
        SetupNavPath(_playButton, null, _optionsButton);
        SetupNavPath(_optionsButton, _playButton, _closeGameButton);
        SetupNavPath(_closeGameButton, _optionsButton);
    }

    private void PlayGame()
    {
        StaticSceneLoader.sceneToLoad = Constants.Level_0;
        SceneManager.LoadScene(Constants.SceneLoader);
    }

    private void OpenOptions()
    {
        Color color = options.rootElement.Q<VisualElement>("Container").style.backgroundColor.value;
        options.rootElement.Q<VisualElement>("Container").style.backgroundColor = new StyleColor(new Color(color.r, color.g, color.b, 255));
        controlUiRoot.OpenView(options);
    }

    private void CloseGame()
    {
        Application.Quit();
    }
}
