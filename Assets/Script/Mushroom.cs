using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom: MonoBehaviour, IInteractable
{
    [SerializeField] GameObject thisObject;
    public void OnInteractStart() { }
    public void OnInteractHold()
    {
        Debug.Log("Get Mushroom!");
        thisObject.SetActive(false);
        Inventory.Instance.AddMushroom();
    }
    public void OnInteractEnd()
    {
       
    }
    private void Start()
    {

    }
    public string GetPrompt() => "Press [E] to get Mushroom";
}
