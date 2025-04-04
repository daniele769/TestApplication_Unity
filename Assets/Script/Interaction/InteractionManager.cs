using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    private int _interactableObjectAround;
    private AbstractInteractableObject _interactableObject;
    
    [SerializeField] 
    private float interactDistance = 2f;
    
    [SerializeField] 
    private float interactHeight = 0.5f;
    void Start()
    {
        _interactableObjectAround = 0;
    }
    
    void FixedUpdate()
    {
        if (_interactableObjectAround > 0)
        {
            SearchInteractableObject();
        }
    }

    public void AddInteractableObjectAround()
    {
        _interactableObjectAround++;
    }

    public void RemoveInteractableObjectAround()
    {
        _interactableObjectAround--;
        if (_interactableObjectAround == 0 && _interactableObject != null)
        {
            _interactableObject.popupInteraction.canvas.enabled = false;
            _interactableObject = null;
        }
    }
    
    private void SearchInteractableObject()
    {
        //print("Searching interactable");
        Vector3 origin = new Vector3(transform.position.x, transform.position.y + interactHeight, transform.position.z);
        Debug.DrawLine(origin , origin + transform.forward * interactDistance, Color.red, 10);
        
        if (Physics.Raycast(origin, transform.forward, out RaycastHit hit, interactDistance, 1, QueryTriggerInteraction.Ignore))
        {
            if (hit.transform.TryGetComponent<AbstractInteractableObject>(out AbstractInteractableObject obj))
            {
                //print("Interactable focused: " + hit.transform.name);
                _interactableObject = obj;
                _interactableObject.popupInteraction.canvas.enabled = true;
                return;
            }
        }

        //print("No Interactable object focused");
        if (_interactableObject)
        {
            _interactableObject.popupInteraction.canvas.enabled = false;
            _interactableObject = null;
        }
    }

    private void OnInteract()
    {
        if (_interactableObject)
        {
            _interactableObject.Interact(transform);
        }
    }
}
