using UnityEngine;

public class SwordStatueControl : AbstractInteractableObject
{
    [SerializeField]
    private HolyBarrierPuzzleControl puzzleControl;

    private AudioSource _audioSource;
    
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    
    void Update()
    {
        
    }

    public override void Interact(Transform player)
    {
        if (CollectableItemManager.Instance.HaveSword())
        {
            puzzleControl.AddPuzzleCompleted();
            transform.Find(Constants.SwordStatue).gameObject.SetActive(true);
            popupInteraction.canvas.enabled = false;
            CollectableItemManager.Instance.RemoveSword(); 
            _audioSource.Play();
            this.enabled = false;
        }
    }
}
