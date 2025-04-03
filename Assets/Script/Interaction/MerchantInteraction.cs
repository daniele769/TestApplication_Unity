using UnityEngine;

public class MerchantInteraction : AbstractInteractableObject
{
    public override void Interact()
    {
        print("Interacted with " + transform.name);
    }
}
