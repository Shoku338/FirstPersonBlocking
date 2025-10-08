using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("Get Treasure!");
        // Your refill logic here
    }

    public string GetPrompt() => "Press [E] to get treasure";
}

