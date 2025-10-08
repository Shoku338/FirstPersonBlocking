using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenRefillStation : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("Oxygen refilled!");
        // Your refill logic here
    }

    public string GetPrompt() => "Press [E] to refill oxygen";
}
