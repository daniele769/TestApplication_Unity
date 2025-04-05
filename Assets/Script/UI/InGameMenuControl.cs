using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class InGameMenuControl : AbstractUIControl
{
    private Button _continueButton;
    private Button _optionsButton;
    private Button _keyBindingButton;
    private Button _backToMenuButton;
    private Button _closeGameButton;

    [SerializeField] 
    private UIControlRoot controlUiRoot;

    [SerializeField] 
    private AbstractUIControl options;

    [SerializeField] 
    private AbstractUIControl keybinding;
    
    void Start()
    {
        InitializeMenu();
        InitializeElements();
        InitializeNavPath();
        
        defaultFocus = _continueButton;
    }

    private void InitializeMenu()
    {
        rootElement.style.display = DisplayStyle.None;
        options.uiDocument.rootVisualElement.style.display = DisplayStyle.None;
        keybinding.uiDocument.rootVisualElement.style.display = DisplayStyle.None;
    }

    protected override void InitializeElements()
    {
        _continueButton = rootElement.Q<Button>("Continue");
        _optionsButton = rootElement.Q<Button>("Options");
        _keyBindingButton = rootElement.Q<Button>("Keybinding");
        _backToMenuButton = rootElement.Q<Button>("BackToMainMenu");
        _closeGameButton = rootElement.Q<Button>("CloseGame");

        _continueButton.clicked += ContinueButtonOnClicked;
        _optionsButton.clicked += OpenOptions;
        _keyBindingButton.clicked += OpenKeybinding;
        _backToMenuButton.clicked += () => { SceneManager.LoadScene(Constants.MainMenuScene); };
        _closeGameButton.clicked += () => { Application.Quit(); };
    }

    private void ContinueButtonOnClicked()
    {
        controlUiRoot.CloseAllView();
    }

    private void OpenOptions()
    {
        lastFocus = _optionsButton;
        controlUiRoot.OpenView(options);
    }
    
    private void OpenKeybinding()
    {
        lastFocus = _keyBindingButton;
        controlUiRoot.OpenView(keybinding);
    }

    protected override void InitializeNavPath()
    {
        SetupNavPath(_continueButton, null, _optionsButton);
        SetupNavPath(_optionsButton, _continueButton, _keyBindingButton);
        SetupNavPath(_keyBindingButton, _optionsButton, _backToMenuButton);
        SetupNavPath(_backToMenuButton, _keyBindingButton, _closeGameButton);
        SetupNavPath(_closeGameButton, _backToMenuButton);
    }

    
}
