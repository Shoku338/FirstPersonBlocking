using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OxygenPlayer : MonoBehaviour
{
    // Singleton instance
    public static OxygenPlayer Instance { get; private set; }

    [SerializeField] Slider oxygenBar;

    [Header("Oxygen Settings")]
    public float maxOxygen = 150f;
    public float currentOxygen;
    [SerializeField]float timeToLoseOxygen = 1;
    [SerializeField]float oxygenLostperTime = 1;
    [SerializeField] Slider cycleSlider;

    float time;

    bool reduceOxygen = true;

    void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        currentOxygen = maxOxygen;
        cycleSlider.gameObject.SetActive(false);
        oxygenBar.value = 1f; // start full
        time = 0;
    }

    private void Update()
    {
        if(reduceOxygen){
            time += Time.deltaTime;
            if (time >= timeToLoseOxygen)
            {
                currentOxygen -= oxygenLostperTime;
                time = 0;
            }
        }
        oxygenBar.value = currentOxygen / maxOxygen;
    }

    public void Drain(float amount)
    {
        currentOxygen -= amount;
        currentOxygen = Mathf.Max(0, currentOxygen);
        Debug.Log("Oxygen drained! Remaining: " + currentOxygen);
    }

    public void Refill(float amount)
    {
        currentOxygen += amount;
        currentOxygen = Mathf.Min(currentOxygen, maxOxygen); // clamp to max
        Debug.Log("Oxygen refilled! Current: " + currentOxygen);
    }

    public bool IsDepleted()
    {
        return currentOxygen <= 0;
    }
    
    public void UpdateCycleBar(float normalizeValue){
        cycleSlider.value = normalizeValue;
    }

    public void ShowCycleBar(bool show)
    {
        cycleSlider.gameObject.SetActive(show);
    }

    public void SetIsreducing(bool state){
        reduceOxygen = state;
    }


}
