using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest: MonoBehaviour, IInteractable
{
    public GameObject thisObject;

    public void OnInteractStart(){ }
    public void OnInteractHold(){
        Debug.Log("Get Treasure!");
        Inventory.Instance.AddGearCount();
        thisObject.SetActive(false);
    }
    public void OnInteractEnd(){
        
        
    }
    private void Start()
    {
        
    }
    public string GetPrompt() => "Press [E] to get treasure";
}

