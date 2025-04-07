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
            popupInteraction.canvas.enabled = false;
            if (_rootElementDialogManager.style.display == DisplayStyle.None)
            {
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
            popupInteraction.canvas.enabled = false;
            if (Input.anyKeyDown)
            {
                SceneManager.LoadScene(Constants.MainMenuScene);
            }
        }
    }

    private IEnumerator FadeToGameCompleteView()
    {
        popupInteraction.canvas.enabled = false;
        VisualElement background = gameCompleteView.rootVisualElement.Q<VisualElement>("GameCompleteBackground");
        Color color = background.style.backgroundColor.value;
        float initialAlpha = color.a;
        float t = 0;
        
        while (t < fadeDuration)
        {
            popupInteraction.canvas.enabled = false;
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(initialAlpha, 1f, t / fadeDuration);
            color.a = alpha;
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
