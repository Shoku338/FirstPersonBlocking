using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TensionManager : MonoBehaviour
{
    [SerializeField] OxygenPlayer oxygen;
    [SerializeField] Light flashlight;
    [SerializeField] AudioSource ambientAudio;
    [SerializeField] TMP_Text uiText;

    private float nextEventTime;

    void Update()
    {
        if (Time.time > nextEventTime)
        {
            float o2 = oxygen.currentOxygen / oxygen.maxOxygen;
            if (o2 < 0.5f)
                TriggerMinorHallucination(o2);
            nextEventTime = Time.time + Random.Range(10f, 25f);
        }
    }
    void TriggerMinorHallucination(float hallucinationValue)
    {

    }
}
