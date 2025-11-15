using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipInteractable : MonoBehaviour, IInteractable
{
   
    public void OnInteractStart() { }
    public void OnInteractHold()
    {

    }
    public void OnInteractEnd()
    {

    }

    public string GetPrompt() => "Press [E] to try and fix";
}
