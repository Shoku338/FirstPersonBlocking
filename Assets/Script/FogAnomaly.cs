using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogAnomaly : MonoBehaviour
{
    [Header("Movement")]
    public Transform[] waypoints;
    public float moveSpeed = 2f;
    public float waitTime = 2f;

    [Header("Effect Settings")]
    public float oxygenDrainMultiplier = 3f;
    public float fogDensityIncrease = 0.05f;
    public float effectFadeTime = 1f;

    private int currentIndex = 0;
    private bool isWaiting = false;
    private float waitTimer = 0f;
    private bool playerInside = false;

    private void Update()
    {
        MoveFog();
    }

    void MoveFog()
    {
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f) isWaiting = false;
            return;
        }

        if (waypoints.Length == 0) return;

        Transform target = waypoints[currentIndex];
        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.5f)
        {
            currentIndex = (currentIndex + 1) % waypoints.Length;
            isWaiting = true;
            waitTimer = waitTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            OxygenPlayer.Instance.SetDrainMultiplier(oxygenDrainMultiplier);
            StartCoroutine(FogVisualEffect(true));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            OxygenPlayer.Instance.SetDrainMultiplier(1f);
            StartCoroutine(FogVisualEffect(false));
        }
    }

    private IEnumerator FogVisualEffect(bool entering)
    {
        float start = RenderSettings.fogDensity;
        float target = entering ? start + fogDensityIncrease : start - fogDensityIncrease;
        float t = 0f;

        while (t < effectFadeTime)
        {
            RenderSettings.fogDensity = Mathf.Lerp(start, target, t / effectFadeTime);
            t += Time.deltaTime;
            yield return null;
        }

        RenderSettings.fogDensity = target;
    }
}