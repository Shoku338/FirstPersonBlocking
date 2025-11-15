using UnityEngine;
using System.Collections;

public class IllusionEffect : MonoBehaviour
{
    [Header("Illusion Settings")]
    [SerializeField] bool vanishOnInteract = true;
    [SerializeField] bool vanishOnApproach = false;
    [SerializeField] float vanishDistance = 2f;
    [SerializeField] float vanishDuration = 0.5f;

    [Header("Optional FX")]
    [SerializeField] ParticleSystem vanishEffect;
    [SerializeField] AudioClip vanishSound;

    private Renderer[] renderers;
    private Collider col;
    private AudioSource audioSource;

    private bool isIllusion = true;
    private bool isVanishing = false;

    public bool IsIllusionActive => isIllusion && !isVanishing;

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        col = GetComponent<Collider>();

        // VITAL CHANGE: Find the IInteractable component and disable it.
        var realInteract = GetComponent<IInteractable>() as MonoBehaviour;
        if (realInteract != null)
            realInteract.enabled = false;

        var mineComponent = GetComponent<OxygenMine>() as MonoBehaviour;
        if (mineComponent != null)
            mineComponent.enabled = false;

        if (col != null)
        {
            // IMPORTANT: Ensure the main collider is a Trigger
            col.isTrigger = true;

            // If it's a MeshCollider, you generally want to disable it entirely 
            // because its shape might be too complex or block the player accidentally.
            if (col is MeshCollider meshCollider)
            {
                meshCollider.convex = true;
                meshCollider.isTrigger = true;
                //col.enabled = false;
                // Optional: You could add a simple BoxCollider for a reliable trigger range.
                // gameObject.AddComponent<BoxCollider>().isTrigger = true; 
            }
        }

        // ... rest of the code remains the same
        if (vanishSound)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }
    void Update()
    {
        if (!IsIllusionActive) return;

        if (vanishOnApproach && PlayerInRange())
            StartCoroutine(VanishRoutine());
    }

    private bool PlayerInRange()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (!player) return false;

        return Vector3.Distance(transform.position, player.transform.position) <= vanishDistance;
    }

    public void TriggerInteractIllusion()
    {
        if (vanishOnInteract && IsIllusionActive)
            StartCoroutine(VanishRoutine());
    }

    private IEnumerator VanishRoutine()
    {
        isVanishing = true;

        if (vanishEffect) vanishEffect.Play();
        if (audioSource && vanishSound) audioSource.PlayOneShot(vanishSound);

        float t = 0;
        while (t < vanishDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / vanishDuration);

            foreach (var r in renderers)
            {
                if (r.material.HasProperty("_Color"))
                {
                    Color c = r.material.color;
                    c.a = alpha;
                    r.material.color = c;
                }
            }
            yield return null;
        }

        // Remove object fully
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(VanishRoutine());
    }
}
