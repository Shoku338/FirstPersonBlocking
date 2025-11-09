using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MoveingPlatform : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField] Transform[] wayPoints;
    [SerializeField] float speed = 2f;

    [Header("Wait Settings")]
    [SerializeField] float minWaitTime = 1f;
    [SerializeField] float maxWaitTime = 3f;

    float offsetDis = 0.1f;
    Vector3 previousPosition;
    Vector3 platformVelocity;
    Transform targetPos;
    int currentIndex = 0;

    bool isWaiting = false;
    float waitTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // ensure it's kinematic for MovePosition
        rb.interpolation = RigidbodyInterpolation.Interpolate; // smooth motion

        targetPos = wayPoints[0];
        previousPosition = rb.position;
    }

    void FixedUpdate()
    {
        Vector3 currentPosition = rb.position;

        // Default to zero velocity each frame before calculating
        platformVelocity = Vector3.zero;

        if (isWaiting)
        {
            UpdateWaitTimer();
        }
        else
        {
            // Move toward target
            Vector3 newPosition = Vector3.MoveTowards(currentPosition, targetPos.position, speed * Time.fixedDeltaTime);
            rb.MovePosition(newPosition);

            // After moving, recalc velocity based on actual movement
            platformVelocity = (newPosition - currentPosition) / Time.fixedDeltaTime;

            // If we reached the target, start waiting
            if (Vector3.Distance(newPosition, targetPos.position) <= offsetDis)
            {
                StartWait();
            }
        }

        previousPosition = rb.position;
    }

    void StartWait()
    {
        isWaiting = true;
        waitTimer = Random.Range(minWaitTime, maxWaitTime);

        // Reset velocity immediately to prevent "launching" player
        platformVelocity = Vector3.zero;
    }

    void UpdateWaitTimer()
    {
        waitTimer -= Time.fixedDeltaTime;
        if (waitTimer <= 0f)
        {
            isWaiting = false;
            targetPos = NextPoint();

            // Precompute "first-frame" velocity so it's ready immediately
            Vector3 dir = (targetPos.position - rb.position).normalized;
            platformVelocity = dir * speed;
        }
    }

    Transform NextPoint()
    {
        currentIndex++;
        if (currentIndex >= wayPoints.Length)
            currentIndex = 0;
        return wayPoints[currentIndex];
    }

    // For player controller reference
    public Vector3 GetPlatformVelocity()
    {
        return platformVelocity;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (wayPoints == null || wayPoints.Length < 2)
            return;

        Gizmos.color = Color.yellow;
        float totalDistance = 0f;

        for (int i = 0; i < wayPoints.Length - 1; i++)
        {
            if (wayPoints[i] == null || wayPoints[i + 1] == null)
                continue;

            Vector3 a = wayPoints[i].position;
            Vector3 b = wayPoints[i + 1].position;

            // Line between waypoints
            Gizmos.DrawLine(a, b);

            // Small spheres at each waypoint
            Gizmos.DrawWireSphere(a, 0.15f);
            Gizmos.DrawWireSphere(b, 0.15f);

            // Draw arrow to show direction
            Vector3 dir = (b - a).normalized;
            Vector3 mid = Vector3.Lerp(a, b, 0.5f);
            Handles.ArrowHandleCap(0, mid, Quaternion.LookRotation(dir), 0.5f, EventType.Repaint);

            // Accumulate distance
            totalDistance += Vector3.Distance(a, b);
        }

        // Optionally loop back to the first point
        if (wayPoints.Length > 2)
        {
            Vector3 first = wayPoints[0].position;
            Vector3 last = wayPoints[wayPoints.Length - 1].position;
            Gizmos.DrawWireSphere(first, 0.2f);
            Gizmos.DrawWireSphere(last, 0.2f);
        }

        // Show travel info text
        float travelTime = totalDistance / Mathf.Max(speed, 0.01f);
        float avgWait = (minWaitTime + maxWaitTime) * 0.5f;
        float estCycleTime = travelTime + wayPoints.Length * avgWait;

        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.red;
        style.fontStyle = FontStyle.Bold;
        style.fontSize = 11;

        Handles.Label(wayPoints[0].position + Vector3.up * 0.5f,
            $"Waypoints: {wayPoints.Length}\nDist: {totalDistance:F1}m\nTravel: {travelTime:F1}s\nCycle~: {estCycleTime:F1}s",
            style);
    }
#endif
}

