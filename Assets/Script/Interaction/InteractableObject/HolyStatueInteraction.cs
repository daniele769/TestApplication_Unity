using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class HolyStatueInteraction : AbstractInteractableObject
{
    [SerializeField] 
    private DialogManager dialogManager;
    
    [SerializeField] 
    private UIDocument gameCompleteView;
    
    [SerializeField] 
    private List<string> dialogs;
    
    [SerializeField] 
    private float fadeDuration;

    private PlayerInput _playerInput;

    private VisualElement _rootElementDialogManager;

    private bool _isDialogStarted;
    private bool _canGoBackToMenu;
    
    void Start()
    {
        _rootElementDialogManager = dialogManager.GetComponent<UIDocument>().rootVisualElement;
    }
    
    void Update()
    {
        if (_isDialogStarted)
        {
            if (_rootElementDialogManager.style.display == DisplayStyle.None)
            {
                Time.timeScale = 0;
                _playerInput.SwitchCurrentActionMap(Constants.ActionMapUI);
                _playerInput.actions.FindAction(Constants.ActionCloseMenu).Disable();
                _playerInput.actions.FindAction(Constants.ActionOpenMenu).Disable();
                gameCompleteView.enabled = true;
                _isDialogStarted = false;
                StartCoroutine(FadeToGameCompleteView());
            }
        }

        if (_canGoBackToMenu)
        {
            if (Input.anyKeyDown)
            {
                SceneManager.LoadScene(Constants.MainMenuScene);
            }
        }
    }

    private IEnumerator FadeToGameCompleteView()
    {
        VisualElement background = gameCompleteView.rootVisualElement.Q<VisualElement>("GameCompleteBackground");
        Color color = background.style.backgroundColor.value;
        float t = 0;
        
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(color.a, 1f, t / fadeDuration);
            background.style.backgroundColor = new StyleColor(color);
            yield return null;
        }

        color.a = 1f;
        background.style.backgroundColor = new StyleColor(color);
        gameCompleteView.rootVisualElement.Q<GroupBox>("GameCompleteGroup").style.display = DisplayStyle.Flex;
        _canGoBackToMenu = true;
    }

    public override void Interact(Transform player)
    {
        _playerInput = player.GetComponent<PlayerInput>();
        _isDialogStarted = true;
        dialogManager.DisplayTextDialog(dialogs);
    }
}
