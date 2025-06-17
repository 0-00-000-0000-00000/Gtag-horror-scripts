using System.Collections;
using UnityEngine;

public class Jumpscare : MonoBehaviour
{
    [Header("Jumpscare + Config - Made by IDK83")]
    [SerializeField] private GameObject objectToEnable; // Object to show during jumpscare
    [SerializeField] private GameObject[] objectsToDisable; // Objects to disable during jumpscare
    [SerializeField] private string targetTag = "GorillaPlayer"; // Tag for triggering jumpscare
    [SerializeField] private float jumpscareDuration = 2f; // Duration of jumpscare
    [SerializeField] private AudioSource audioSource; // Audio to play during jumpscare
    [SerializeField] private Transform jumpscarePosition; // Position to teleport player to

    private GameObject player;
    private Rigidbody playerRigidbody;
    private bool isJumpscaring;
    private Collider triggerCollider;

    private void Awake()
    {
        // Cache trigger collider
        triggerCollider = GetComponent<Collider>();
        if (triggerCollider == null || !triggerCollider.isTrigger)
        {
            Debug.LogError("Jumpscare script requires a trigger Collider on " + gameObject.name);
            enabled = false;
            return;
        }

        // Cache player and rigidbody
        player = GameObject.FindGameObjectWithTag(targetTag);
        if (player != null)
        {
            playerRigidbody = player.GetComponent<Rigidbody>();
            if (playerRigidbody == null)
            {
                Debug.LogError("Player with tag 'GorillaPlayer' has no Rigidbody component");
                enabled = false;
            }
        }
        else
        {
            Debug.LogWarning("No player with tag 'GorillaPlayer' found in the scene");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isJumpscaring || player == null) return;

        if (other.CompareTag(targetTag))
        {
            StartCoroutine(ExecuteJumpscare());
        }
    }

    private IEnumerator ExecuteJumpscare()
    {
        isJumpscaring = true;

        // Activate jumpscare object
        if (objectToEnable != null)
            objectToEnable.SetActive(true);

        // Disable specified objects
        foreach (GameObject obj in objectsToDisable)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        // Play audio
        if (audioSource != null)
            audioSource.Play();

        // Teleport player and freeze movement
        if (player != null && playerRigidbody != null && jumpscarePosition != null)
        {
            player.transform.position = jumpscarePosition.position;
            playerRigidbody.isKinematic = true;
        }

        // Wait for jumpscare duration
        yield return new WaitForSeconds(jumpscareDuration);

        // Revert changes
        if (objectToEnable != null)
            objectToEnable.SetActive(false);

        foreach (GameObject obj in objectsToDisable)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();

        if (playerRigidbody != null)
            playerRigidbody.isKinematic = false;

        isJumpscaring = false;
    }

    // Public method to trigger jumpscare manually (e.g., from another script)
    public void TriggerJumpscare()
    {
        if (!isJumpscaring)
            StartCoroutine(ExecuteJumpscare());
    }

    private void OnValidate()
    {
        // Ensure the collider is set as a trigger in the editor
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning("Collider on " + gameObject.name + " should be set as a trigger for jumpscare functionality");
        }
    }
}