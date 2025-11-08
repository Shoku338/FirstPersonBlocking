using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveingPlatform : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] Transform[] wayPoints;
    [SerializeField] float speed = 2;
    [Header("Wait Settings")]
    [SerializeField] float minWaitTime = 1f;   //  Minimum wait time
    [SerializeField] float maxWaitTime = 3f;   //  Maximum wait time

    float offsetDis = 0.1f;
    Vector3 previousPosition;
    Vector3 platformVelocity;
    Transform targetPos;
    int currentIndex = 0;

    bool isWaiting = false;
    float waitTimer = 0f;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        targetPos = wayPoints[0];
        previousPosition = rb.position;
    }

    void FixedUpdate()
    {
        Vector3 currentPosition = rb.position;
        platformVelocity = (currentPosition - previousPosition) / Time.deltaTime;
        previousPosition = currentPosition;

        if (isWaiting)
        {
            UpdateWaitTimer();
            return; // pause movement while waiting
        }

        rb.MovePosition(Vector3.MoveTowards(transform.position, targetPos.position, speed * Time.fixedDeltaTime));

        if (Vector3.Distance(transform.position, targetPos.position) < offsetDis)
        {
            StartWait();
        }
            
    }
    private void StartWait()
    {
        isWaiting = true;
        waitTimer = Random.Range(minWaitTime, maxWaitTime);
    }

    // Function: Countdown wait timer
    private void UpdateWaitTimer()
    {
        waitTimer -= Time.fixedDeltaTime;
        if (waitTimer <= 0f)
        {
            isWaiting = false;
            targetPos = NextPoint();
        }
    }
    private Transform NextPoint()
    {
        currentIndex++;
        if (currentIndex >= wayPoints.Length)
            currentIndex = 0;
        return wayPoints[currentIndex];
    }
    private void OnCollisionStay(Collision collision)
    {
        Debug.Log($"Collision Enter with {collision.gameObject.name}");
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log($"player on platform \n add player velocity {platformVelocity}");
            collision.gameObject.GetComponent<MCController>().SetVelocity(platformVelocity);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<MCController>().SetVelocity(Vector3.zero);
        }
    }

    public Vector3 GetPlatformVelocity() => platformVelocity;
}
