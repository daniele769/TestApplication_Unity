using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public abstract class AbstractUIControl : MonoBehaviour
{
    public bool isMenuVisible;
    public VisualElement rootElement;
    public VisualElement defaultFocus;
    public VisualElement lastFocus;

    private Dictionary<VisualElement, VisualElement[]> navDirections = new Dictionary<VisualElement, VisualElement[]>();
    
    [HideInInspector]
    public UIDocument uiDocument;

    protected virtual void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
        //print("UiDocument setted in " + this.name);
        rootElement = uiDocument.rootVisualElement;
    }

    protected virtual void Update()
    {
        ManageFocus();
    }

    private void ManageFocus()
    {
        if(rootElement.style.display == DisplayStyle.None)
            return;
        
        if (PlayerInputState.FocusEnabled)
        {
            if (lastFocus != null)
            {
                lastFocus.Focus();
                return;
            }
            defaultFocus?.Focus();
        }
        else
        {
            if (lastFocus != null)
            {
                lastFocus.Blur();
                return;
            }
            uiDocument.rootVisualElement.pickingMode = PickingMode.Ignore;
            defaultFocus?.Blur();
        }
    }

    protected void SetupNavPath(VisualElement currentFocused, VisualElement upElement = null, VisualElement downElement = null, VisualElement leftElement = null, VisualElement rightElement = null)
    {
        navDirections.Add(currentFocused, new []{upElement, downElement, leftElement, rightElement});
        
        currentFocused.RegisterCallback<NavigationMoveEvent>(e =>
        {
            switch (e.direction)
            {
                case NavigationMoveEvent.Direction.Up: FocusElement(upElement, e); break;
                case NavigationMoveEvent.Direction.Down: FocusElement(downElement, e); break;
                case NavigationMoveEvent.Direction.Left: FocusElement(leftElement, e); break;
                case NavigationMoveEvent.Direction.Right: FocusElement(rightElement, e); break;
            }
            currentFocused.focusController.IgnoreEvent(e);
        });
    }

    private void FocusElement(VisualElement element, NavigationMoveEvent e)
    {
       if(element == null || !PlayerInputState.FocusEnabled)
           return;

       if (!element.enabledInHierarchy)
       {
           if (!navDirections.ContainsKey(element))
               return;
           
           VisualElement[] navElements = navDirections[element];
           
           switch (e.direction)
           {
               case NavigationMoveEvent.Direction.Up: FocusElement(navElements[0], e); break;
               case NavigationMoveEvent.Direction.Down: FocusElement(navElements[1], e); break;
               case NavigationMoveEvent.Direction.Left: FocusElement(navElements[2], e); break;
               case NavigationMoveEvent.Direction.Right: FocusElement(navElements[3], e); break;
           }
       }
       else
       {
           element.Focus();
           lastFocus = element;
       }
    }

    protected abstract void InitializeElements();
    protected abstract void InitializeNavPath();
}
