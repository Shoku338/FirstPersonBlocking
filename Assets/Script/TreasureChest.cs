using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : MonoBehaviour, IInteractable
{
    [SerializeField] GameObject thisObject;
    public void OnInteractStart(){ }
    public void OnInteractHold(){ }
    public void OnInteractEnd(){
        Debug.Log("Get Treasure!");
        Inventory.Instance.AddGearCount();
        thisObject.SetActive(false);
        
    }

    public string GetPrompt() => "Press [E] to get treasure";
}

