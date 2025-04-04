using UnityEngine;
using UnityEngine.InputSystem;

public class InputDeviceManager : MonoBehaviour
{
    void Start()
    {
        
    }
    
    void Update()
    {
        ManageFocusEnable();
        
        if (PlayerInputState.CurrentActionMap.Equals("UI") && PlayerInputState.CurrentControlScheme != "Gamepad" && Time.timeScale == 0)
        {
            Cursor.visible = true;
            return;
        }
        Cursor.visible = false;
    }

    private void ManageFocusEnable()
    {
        if (PlayerInputState.CurrentControlScheme == "Keyboard&Mouse")
        {
            if (InputSystem.actions.FindAction("Look").triggered)
            {
                if (PlayerInputState.FocusEnabled)
                {
                    print("Disable focus");
                    PlayerInputState.FocusEnabled = false;
                }
            }
            else if (InputSystem.actions.FindAction("Navigate").triggered)
            {
                print("Gamepad pressed");
                if (!PlayerInputState.FocusEnabled)
                {
                    print("Enable focus");
                    PlayerInputState.FocusEnabled = true;
                }
            }
        }
        else if (!PlayerInputState.FocusEnabled)
        {
            print("Enable focus");
            PlayerInputState.FocusEnabled = true;
        }
    }
}

