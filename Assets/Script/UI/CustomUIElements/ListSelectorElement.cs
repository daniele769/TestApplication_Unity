using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UIElements;

public class ListSelectorElement : VisualElement
{
    private VisualElement _visualContainer;
    private Label _nameLabel;
    private Button _leftButton;
    private Button _rightButton;
    private Label _selectedValue;

    private List<string> _valuesList;
    private int _currentIndex = -1;

    public ListSelectorElement(VisualElement selectorElement)
    {
        _visualContainer = selectorElement;
        _nameLabel = selectorElement.Q<Label>("Name");
        _selectedValue = selectorElement.Q<Label>("ValueLabel");
        _leftButton = selectorElement.Q<Button>("PrevButton");
        _rightButton = selectorElement.Q<Button>("NextButton");
        
        _leftButton.clicked += PrevAction;
        _rightButton.clicked += NextAction;
    }

    public void InitializeListSelector(List<string> list)
    {
        _valuesList = list;
        if (_valuesList.Count > 0)
        {
            _currentIndex = 0;
        }
        else
        {
            _currentIndex = -1;
        }
        SetupFocusActions();
    }

    public void SetValue(string value)
    {
        int pos = 0;
        foreach (string val in _valuesList)
        {
            if (val.Equals(value))
            {
                _currentIndex = pos;
                _selectedValue.text = val;
                return;
            }
            pos++;
        }
        Debug.Log("Value not found in ListSelector");
    }

    private void NextAction()
    {
        _currentIndex++;
        CheckIndex();
    }
    
    private void PrevAction()
    {
        _currentIndex--;
        CheckIndex();
    }

    private void CheckIndex()
    {
        if (_currentIndex < 0)
            _currentIndex = _valuesList.Count - 1;
        
        else if (_currentIndex > _valuesList.Count - 1)
            _currentIndex = 0;

        _selectedValue.text = _valuesList[_currentIndex];
    }

    public string SelectedValue()
    {
        return _selectedValue.text;
    }

    public VisualElement RootElement()
    {
        return _visualContainer.Q<VisualElement>("ListSelector");
    }
    
    protected void SetupFocusActions()
    {
        _visualContainer.RegisterCallback<NavigationMoveEvent>(e =>
        {
            switch (e.direction)
            {
                case NavigationMoveEvent.Direction.Up: break;
                case NavigationMoveEvent.Direction.Down: break;
                case NavigationMoveEvent.Direction.Left: PrevAction(); break;
                case NavigationMoveEvent.Direction.Right: NextAction(); break;
            }
            _visualContainer.focusController.IgnoreEvent(e);
        });
    }
    
    
}