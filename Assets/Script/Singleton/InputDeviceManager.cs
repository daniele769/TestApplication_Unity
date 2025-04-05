using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputDeviceManager : MonoBehaviour
{
    public static InputDeviceManager Instance { get; private set; }
    private PlayerControllerRigid _playerController;
    
    [HideInInspector]
    public string currentControlScheme;
    
    [HideInInspector]
    public string currentActionMap;
    
    [HideInInspector]
    public bool focusEnabled = true;

    [SerializeField] 
    private PlayerInput playerInput;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            currentControlScheme = playerInput.currentControlScheme;
            currentActionMap = playerInput.currentActionMap.name;
            _playerController = playerInput.GetComponent<PlayerControllerRigid>();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        currentControlScheme = playerInput.currentControlScheme;
        currentActionMap = playerInput.currentActionMap.name;
        
        ManageFocusEnable();
        
        if (currentActionMap.Equals("UI") && currentControlScheme != "Gamepad" && (Time.timeScale == 0 || _playerController.IsDeath()) )
        {
            Cursor.visible = true;
            return;
        }
        Cursor.visible = false;
    }
    
    private void ManageFocusEnable()
    {
        if (currentControlScheme == "Keyboard&Mouse")
        {
            if (InputSystem.actions.FindAction("Look").triggered)
            {
                if (focusEnabled)
                {
                    print("Disable focus");
                    focusEnabled = false;
                }
            }
            else if (InputSystem.actions.FindAction("Navigate").triggered)
            {
                print("Gamepad pressed");
                if (!focusEnabled)
                {
                    print("Enable focus");
                    focusEnabled = true;
                }
            }
        }
        else if (!focusEnabled)
        {
            print("Enable focus");
            focusEnabled = true;
        }
    }
}
