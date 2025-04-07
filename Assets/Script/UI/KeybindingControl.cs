using UnityEngine;
using UnityEngine.UIElements;

public class KeybindingControl : AbstractUIControl
{
    [SerializeField] 
    private UIControlRoot uiControlRoot;
    
    private Button _backButton;
    
    void Start()
    {
        InitializeElements();
        InitializeNavPath();

        defaultFocus = _backButton;
    }

    protected override void InitializeElements()
    {
        _backButton = rootElement.Q<Button>("BackButton");
        
        _backButton.clicked += uiControlRoot.GoToPrevious;
    }

    protected override void InitializeNavPath()
    {
        SetupNavPath(_backButton);
    }
}