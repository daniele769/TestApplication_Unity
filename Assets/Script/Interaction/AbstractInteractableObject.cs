using System;
using UnityEngine;

public abstract class AbstractInteractableObject : MonoBehaviour
{
    public PopupInteraction popupInteraction;

    public abstract void Interact(Transform player);

}
