using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPCInteract : MonoBehaviour, IInteractable
{
    [SerializeField] TMP_Text dialogueText; // UI text to show messages
    [SerializeField] float dialogueDuration = 2f;
    private bool hasTraded = false; // Prevent multiple trades

    string originalText;

    private void Start()
    {
        originalText = dialogueText.text;
    }

    public void OnInteractStart() { }
    public void OnInteractHold() 
    {
        if (hasTraded)
        {
            ShowDialogue("You already traded with me!");
            return;
        }

        // Check inventory
        if (Inventory.Instance != null && Inventory.Instance.HasMushrooms(5))
        {
            // Do trade
            Inventory.Instance.RemoveMushrooms(5);
            Inventory.Instance.AddGearCount();
            hasTraded = true;
            ShowDialogue("Thanks! Here's a gear!");
        }
        else
        {
            ShowDialogue("You need 5 mushrooms!");
        }
    }

    public void OnInteractEnd()
    {
        
    }

    public string GetPrompt() => "Press [E] to Trade";

    private void ShowDialogue(string message)
    {
        StopAllCoroutines(); // In case previous message still running
        StartCoroutine(DialogueRoutine(message));
    }

    private IEnumerator DialogueRoutine(string message)
    {
        dialogueText.text = message;
        dialogueText.transform.parent.gameObject.SetActive(true);
        yield return new WaitForSeconds(dialogueDuration);
        if (!hasTraded)
            dialogueText.text = originalText;

        else
            dialogueText.text = "MY MUSHROOM NOW!";
        yield return new WaitForSeconds(dialogueDuration);
        dialogueText.transform.parent.gameObject.SetActive(false);
    }
}
