using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenMine : MonoBehaviour
{
    [SerializeField] float oxygenDrainAmount = 20f;
    [SerializeField] float knockbackForce = 5f;
    [SerializeField] GameObject parentObject;


    private void OnTriggerEnter(Collider other)
    {
        if (!enabled)
            return;
        if (other.CompareTag("Player"))
        {
            // Drain oxygen
            other.GetComponent<OxygenPlayer>().Drain(oxygenDrainAmount);

            // Knockback through controller
            Vector3 dir = (other.transform.position - transform.position).normalized;
            other.GetComponent<MCController>().ApplyKnockback(dir, knockbackForce);

            // TODO: Add VFX/SFX
            Destroy(parentObject);
        }
    }
}
