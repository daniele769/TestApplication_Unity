using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MerchantInteraction : AbstractInteractableObject
{
    private float _rotSpeed = 7f;
    private Coroutine _lookAtPlayer = null;
    private bool _alreadyTalked;
    private bool _interactionStarted;
    private AudioSource _audioSource;
    
    [HideInInspector]
    public bool dialogFinished;

    [SerializeField] 
    private CoinsHUDManager coinsHUD;
    
    [SerializeField] 
    private int potionCost = 5;
    
    [SerializeField] 
    private PotionsHUDManager potionHUD;
    
    [SerializeField] 
    private DialogManager dialogManager;
    
    [SerializeField] 
    private List<string> firstOnlyDialogs;
    
    [SerializeField] 
    private List<string> defaultDialogs;
    
    [SerializeField] 
    private bool choiceAtEnd;
    
    [SerializeField] 
    private string choiceSummary;
    
    [SerializeField] 
    private string choiceOneText;
    
    [SerializeField] 
    private string choicetwoText;
    
    [SerializeField] 
    private List<string> choiceOneReply;
    
    [SerializeField] 
    private List<string> choiceTwoReply;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    
    public override void Interact(Transform player)
    {
        print("Interacted with " + transform.name);
        LookAtPlayer(player);
        
        dialogManager.RegisterButtonCallback(YesAction, NoAction);
        
        if (!_alreadyTalked)
        {
            dialogManager.DisplayTextDialog(firstOnlyDialogs, choiceAtEnd, choiceSummary, choiceOneText, choicetwoText);
            _alreadyTalked = true;
            return;
        }
        dialogManager.DisplayTextDialog(defaultDialogs, choiceAtEnd, choiceSummary, choiceOneText, choicetwoText);
        
    }

    private void LookAtPlayer(Transform player)
    {
        if (_lookAtPlayer != null)
        {
            StopCoroutine(_lookAtPlayer);
            _lookAtPlayer = null;
        }
        _lookAtPlayer = StartCoroutine(RotateToPlayer(player));
    }

    private IEnumerator RotateToPlayer(Transform player)
    {
        Quaternion targetRotation = Quaternion.LookRotation(player.position - transform.position);
        
        while (true)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotSpeed * Time.deltaTime);
            float angle = Quaternion.Angle(transform.rotation, targetRotation);
            if(angle < 0.1f)
            {
                _lookAtPlayer = null;
                yield break;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private void YesAction()
    {
        if (coinsHUD.coinsCount >= potionCost)
        {
            coinsHUD.ConsumeCoin(potionCost);
            potionHUD.AddPotions();
            dialogManager.ShowReply(choiceOneReply[0]);
            _audioSource.Play();
            return;
        }
        dialogManager.ShowReply(choiceOneReply[1]);
    }

    private void NoAction()
    {
        dialogManager.ShowReply(choiceTwoReply[0]);
    }
    
}
