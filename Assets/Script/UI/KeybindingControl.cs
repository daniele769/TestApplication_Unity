using UnityEngine;

public class KeybindingControl : AbstractUIControl
{
    protected override void Start()
    {
        base.Start();
        
        InitializeElements();
        InitializeNavPath();
    }

    protected override void InitializeElements()
    {
        print("Implement InitializeElements into KeybindingControl");
    }

    protected override void InitializeNavPath()
    {
        print("Implement InitializeNavPath into KeybindingControl");
    }
}