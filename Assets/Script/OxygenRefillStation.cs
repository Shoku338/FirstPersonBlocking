using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OxygenRefillStation : MonoBehaviour, IInteractable
{
    [Header("Refill Settings")]
    public float oxygenGainPerCycle = 10f;
    [SerializeField] float cycleTime = 3f;
    public float totalOxygen = 50;
    public bool requireHold = true; // Toggle this to switch between hold vs press
    [SerializeField] GameObject oxygenTankVisual;

    private float currentOxygen;

    private bool isRefilling = false;
    
    private float time = 0;

    private void Start()
    {
        currentOxygen = totalOxygen;
        time = 0;
    }

    public void OnInteractStart()
    {
        if (!requireHold)
        {
            // Tap once: instantly refill a chunk
            OxygenPlayer.Instance.Refill(oxygenGainPerCycle);
            Debug.Log("Oxygen refilled by tapping!");
        }
        else
        {
            // Begin holding
            isRefilling = true;
            OxygenPlayer.Instance.SetIsreducing(!isRefilling);
            OxygenPlayer.Instance.ShowCycleBar(isRefilling);
            Debug.Log("Started refilling...");
        }
    }

    public void OnInteractHold()
    {
        if (requireHold && isRefilling)
        {
            time += Time.deltaTime;
            OxygenPlayer.Instance.UpdateCycleBar(time / cycleTime);
            if (time >= cycleTime && currentOxygen > 0)
            {
                OxygenPlayer.Instance.Refill(oxygenGainPerCycle);
                currentOxygen -= oxygenGainPerCycle;
                float fillPercent = Mathf.Clamp01(currentOxygen / totalOxygen); // assuming 50 is max
                Vector3 scale = oxygenTankVisual.transform.localScale;
                scale.x = fillPercent; // only scale in Y axis
                oxygenTankVisual.transform.localScale = scale;
                time = 0;
            }
            else if (currentOxygen < 0)
            {
                OnInteractEnd();
            }

        }
    }

    public void OnInteractEnd()
    {
        if (requireHold)
        {
            isRefilling = false;
            OxygenPlayer.Instance.SetIsreducing(!isRefilling);
            OxygenPlayer.Instance.ShowCycleBar(isRefilling);
            time = 0;
            Debug.Log("Stopped refilling.");
        }
    }

    public string GetPrompt() => requireHold ? "Hold [E] to refill oxygen" : "Press [E] to refill oxygen";
}
