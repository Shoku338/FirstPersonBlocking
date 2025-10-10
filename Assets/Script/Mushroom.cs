using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour, IInteractable
{
    [SerializeField] GameObject thisObject;
    public void OnInteractStart() { }
    public void OnInteractHold() { }
    public void OnInteractEnd()
    {
        Debug.Log("Get Mushroom!");
        thisObject.SetActive(false);
        Inventory.Instance.AddMushroom();
    }

    public string GetPrompt() => "Press [E] to get Mushroom";
}
