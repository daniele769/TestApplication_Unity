using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class UIControlRoot : MonoBehaviour
{
    private List<AbstractUIControl> viewQueque = new List<AbstractUIControl>();
    private int currentPos = -1;

    [SerializeField] 
    private AbstractUIControl inGameMenu;

    [SerializeField] 
    private PlayerInput playerInput;
    
    public void Update()
    {
        if (InputDeviceManager.Instance.currentActionMap.Equals("Player"))
        {
            if (InputSystem.actions.FindAction("OpenMenu").triggered)
            {
                print("OpenMenu");
                OpenRoot();
                return;
            }
        }
        
        if (InputDeviceManager.Instance.currentActionMap.Equals("UI"))
        {
            if (InputSystem.actions.FindAction("Cancel").triggered)
            {
                print("BackActionPressed");
                GoToPrevious();
                return;
            }
            
            if (InputSystem.actions.FindAction("CloseMenu").triggered)
            {
                print("CloseMenu");
                CloseAllView();
                return;
            }
        }
    }

    public void OpenRoot()
    {
        playerInput.SwitchCurrentActionMap("UI");
        OpenView(inGameMenu);
        Time.timeScale = 0;
    }

    public void OpenView(AbstractUIControl view)
    {
        if (viewQueque.Count > 0)
        {
            viewQueque[currentPos].uiDocument.rootVisualElement.style.display = DisplayStyle.None;
            viewQueque[currentPos].isMenuVisible = false;
        }
        viewQueque.Add(view);
        view.uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
        view.defaultFocus?.Focus();
        view.isMenuVisible = true;
        currentPos++;
    }

    public void GoToPrevious()
    {
        if (viewQueque.Count > 0)
        {
            viewQueque[currentPos].uiDocument.rootVisualElement.style.display = DisplayStyle.None;
            viewQueque[currentPos].isMenuVisible = false;
            viewQueque[currentPos].lastFocus = null;
            viewQueque.RemoveAt(currentPos);
            currentPos--;
            if (viewQueque.Count > 0)
            {
                viewQueque[currentPos].uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
                viewQueque[currentPos].lastFocus.Focus();
                viewQueque[currentPos].isMenuVisible = true;
            }
            else
            {
                playerInput.SwitchCurrentActionMap("Player");
                Time.timeScale = 1;
            }
            
        }
    }

    public void CloseAllView()
    {
        print("CloseMenu");
        for (int i = viewQueque.Count - 1; i >= 0; i--)
        {
            viewQueque[i].uiDocument.rootVisualElement.style.display = DisplayStyle.None;
            viewQueque[i].lastFocus = null;
            viewQueque[i].isMenuVisible = false;
            viewQueque.RemoveAt(i);
            currentPos--;
        }
        
        playerInput.SwitchCurrentActionMap("Player");
        Time.timeScale = 1;
    }
}