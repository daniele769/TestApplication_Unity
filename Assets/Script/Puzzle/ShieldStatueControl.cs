using UnityEngine;

public class ShieldStatueControl : AbstractInteractableObject
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
        if (CollectableItemManager.Instance.HaveShield())
        {
            puzzleControl.AddPuzzleCompleted();
            transform.Find(Constants.ShieldStatue).gameObject.SetActive(true);
            popupInteraction.canvas.enabled = false;
            CollectableItemManager.Instance.RemoveShield();
            _audioSource.Play();
            this.enabled = false;
        }
    }
}
