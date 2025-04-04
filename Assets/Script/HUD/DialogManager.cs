using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class DialogManager : AbstractUIControl
{
    private UIDocument _uiDocument;
    private Label _label;
    private GroupBox _buttonBox;
    private List<string> _textToDisplay;
    private int _pos;
    private bool _isTextPlaying;
    private Coroutine _displayText = null;
    private AudioSource _audioSource;
    private GameObject _interactor;
    private bool _choiceAtEnd;
    private string _choiceSummary;
    private string _choiceOneText;
    private string _choicetwoText;
    private List<string> _choiceOneReply;
    private List<string> _choiceTwoReply;
    private bool _choicePlaying;
    private Action _actionYesButton;
    private Action _actionNoButton;
    private bool _isFocusInitialized;
    
    [HideInInspector]
    public Button yesButton;
    
    [HideInInspector]
    public Button noButton;
    
    [SerializeField]
    private float textSpeed = 0.05f;
    
    [SerializeField]
    private PlayerInput playerInput;
    
    void OnEnable()
    {
        InitializeElements();
        //defaultFocus = noButton;
    }

    
    void Update()
    {
        if (InputDeviceManager.Instance.currentActionMap.Equals(Constants.ActionMapUI) && _uiDocument.rootVisualElement.style.display == DisplayStyle.Flex)
        {
            if (InputSystem.actions.FindAction("Submit").triggered)
            {
                if (_isTextPlaying)
                {
                    StopCoroutine(_displayText);
                    _label.text = _textToDisplay[_pos];
                    _isTextPlaying = false;
                    if (_choicePlaying)
                    {
                        InitializeChoices();
                    }
                    return;
                }
                
                if(_choicePlaying)
                    return;
                
                _pos++;
                if (_pos >= _textToDisplay.Count)
                {
                    if (_choiceAtEnd)
                    {
                        MakeChoice();
                        return;
                    }
                    
                    _uiDocument.rootVisualElement.style.display = DisplayStyle.None;
                    playerInput.SwitchCurrentActionMap(Constants.ActionMapPlayer);
                    StopCoroutine(_displayText);
                    _displayText = null;
                    _textToDisplay = null;
                    _pos = 0;
                    _label.text = "";
                    yesButton.clicked -= _actionYesButton;
                    noButton.clicked -= _actionNoButton;
                    _isFocusInitialized = false;
                    return;
                }
                _displayText = StartCoroutine(DisplayText());
            }

            if (!_isFocusInitialized && _buttonBox.style.display == DisplayStyle.Flex)
            {
                noButton.Focus();
                _isFocusInitialized = true;
            }
        }
    }

    protected override void InitializeElements()
    {
        _uiDocument = GetComponent<UIDocument>();
        _label = _uiDocument.rootVisualElement.Q<Label>("Text");
        yesButton = _uiDocument.rootVisualElement.Q<Button>("YesButton");
        noButton = _uiDocument.rootVisualElement.Q<Button>("NoButton");
        _buttonBox = _uiDocument.rootVisualElement.Q<GroupBox>("ButtonBox");
        _audioSource = GetComponent<AudioSource>();
        
        _uiDocument.rootVisualElement.style.display = DisplayStyle.None;
        _buttonBox.style.display = DisplayStyle.None;
        _pos = 0;
    }

    protected override void InitializeNavPath()
    {
        SetupNavPath(yesButton, null, noButton);
        SetupNavPath(noButton, yesButton, noButton);
    }

    private void MakeChoice()
    {
        _choicePlaying = true;
        _pos = 0;
        _textToDisplay.Clear();
        _textToDisplay.Add(_choiceSummary);
        _displayText = StartCoroutine(DisplayText(true));
    }

    public void DisplayTextDialog(List<string> textList, bool isChoice = false, string summary = "", string choiceOne = "", string choiceTwo = "")
    {
        _textToDisplay = new List<string>(textList);
        _choiceAtEnd = isChoice;
        if (isChoice)
        {
            _choiceSummary = summary;
            _choiceOneText = choiceOne;
            _choicetwoText = choiceTwo;
        }
        _pos = 0;
        playerInput.SwitchCurrentActionMap(Constants.ActionMapUI);
        _uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
        _displayText = StartCoroutine(DisplayText());
        
    }

    private IEnumerator DisplayText(bool startChoice = false)
    {
        _isTextPlaying = true;
        string text;
        for (int i = 0; i < _textToDisplay[_pos].Length; i++)
        { 
            _label.text = _textToDisplay[_pos].Substring(0, i); 
            _audioSource.Play();
            yield return new WaitForSeconds(textSpeed);
        }
        _isTextPlaying = false;
        if (startChoice)
        {
            StopCoroutine(_displayText);
            InitializeChoices();
        }
    }

    private void InitializeChoices()
    {
        yesButton.text = _choiceOneText;
        noButton.text = _choicetwoText;
        _buttonBox.style.display = DisplayStyle.Flex;
        Time.timeScale = 0;
    }

    public void RegisterButtonCallback(Action yesAction, Action noAction)
    {
        _actionYesButton = yesAction;
        _actionNoButton = noAction;
        
        yesButton.clicked += _actionYesButton;
        noButton.clicked += _actionNoButton;
    }

    public void ShowReply(string reply)
    {
        _textToDisplay.Clear();
        _pos = 0;
        _textToDisplay.Add(reply);
        Time.timeScale = 1;
        _choicePlaying = false;
        _choiceAtEnd = false;
        StopCoroutine(_displayText);
        _displayText = StartCoroutine(DisplayText());
        _buttonBox.style.display = DisplayStyle.None;
    }
    
    
}
