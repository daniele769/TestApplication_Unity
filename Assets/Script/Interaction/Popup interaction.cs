using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class PopupInteraction : MonoBehaviour
{
    [SerializeField] 
    private TMP_Text label;
    
    [SerializeField]
    private string popupText = "Interact";

    private Camera mainCamera;
    
    [HideInInspector]
    public Canvas canvas;
    void Awake()
    {
        canvas = GetComponent<Canvas>();
        label.text = popupText;
        canvas.enabled = false;
        mainCamera = Camera.main;
    }
    
    void Update()
    {
        if(mainCamera) 
            canvas.transform.LookAt(mainCamera.transform);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InteractionManager interactionManager = other.GetComponent<InteractionManager>();
            interactionManager.AddInteractableObjectAround();
            //print("Interactable object inside interaction range: " + other.transform.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InteractionManager interactionManager = other.GetComponent<InteractionManager>();
            interactionManager.RemoveInteractableObjectAround();
            //print("Interactable outside interaction range: " + other.transform.name);
        }
    }
}
