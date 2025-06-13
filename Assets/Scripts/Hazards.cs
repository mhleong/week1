/*
 * Author: Ming Hui
 * Date: 11/06/2025
 * Description: Script for hazards and player taking dmg
 */


using UnityEngine;
using System.Collections.Generic;

public class Hazard : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] int damageAmount = 10;
    [SerializeField] float damageInterval = 1f; // Time between damage ticks (DOT)
    [SerializeField] bool isInstant = false; //istant dmg 
    [SerializeField] bool slowsPlayer = false; //for frozen pond
    [SerializeField] float slowMultiplier = 0.5f; //frozen pond
    [SerializeField] AudioClip hitSFX;


    [Header("Falling Hazard Settings")]
    [SerializeField] bool isFallingHazard = false; // Enabled for falling icicles
    [SerializeField] float triggerRadius = 50f; // How close player must be to trigger fall
    [SerializeField] float fallDelayMin = 0.1f;
    [SerializeField] float fallDelayMax = 1f;
    [SerializeField] float embedOffsetY = 0f; // How deep into the ground the icicle embeds


    Rigidbody rb;
    bool hasFallen = false;


    private Dictionary<GameObject, Coroutine> activeDOTs = new Dictionary<GameObject, Coroutine>();

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (isFallingHazard && rb != null)
        {
            rb.isKinematic = true; // Don't fall until triggered
        }
    }


    void Update()
    {
        if (isFallingHazard && !hasFallen)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null && Vector3.Distance(transform.position, player.transform.position) <= triggerRadius)
            {
                hasFallen = true;
                float delay = Random.Range(fallDelayMin, fallDelayMax);
                Invoke(nameof(ActivateFall), delay);
            }
        }
    }


    void ActivateFall()
    {
        if (rb != null)
        {
            rb.isKinematic = false;
            Debug.Log("Icicles falling, beware!");
        }
    }

    // Detect collision for physical hazards like falling icicles
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HandleHazardEffects(collision.gameObject);
            if (isFallingHazard)
            {
                rb.isKinematic = true; // Freeze in place
                Vector3 pos = transform.position;
                pos.y = pos.y + embedOffsetY;
                transform.position = pos;
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!isFallingHazard && other.CompareTag("Player"))
        {
            PlayerBehaviour player = other.GetComponent<PlayerBehaviour>();

            if (player != null)
            {
                if (isInstant)
                {
                    player.TakeDamage(damageAmount);
                }
                else if (!activeDOTs.ContainsKey(other.gameObject))
                {
                    Coroutine dotCoroutine = StartCoroutine(DamageOverTime(player));
                    activeDOTs[other.gameObject] = dotCoroutine;
                }

                if (slowsPlayer)
                {
                    player.SetMoveSpeed(slowMultiplier);
                }

                if (hitSFX)
                {
                    AudioSource.PlayClipAtPoint(hitSFX, transform.position);
                }

                if (player.CurrentHealth <= 0)
                {
                    player.Respawn();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerBehaviour player = other.GetComponent<PlayerBehaviour>();

            if (player != null && slowsPlayer)
            {
                player.ResetMoveSpeed();
            }

            if (activeDOTs.ContainsKey(other.gameObject))
            {
                StopCoroutine(activeDOTs[other.gameObject]);
                activeDOTs.Remove(other.gameObject);
            }
        }
    }

    // Main effect logic
    void HandleHazardEffects(GameObject playerObj)
    {
        PlayerBehaviour player = playerObj.GetComponent<PlayerBehaviour>();

        if (player != null)
        {
            if (isInstant)
            {
                player.TakeDamage(damageAmount);
            }
            else
            {
                StartCoroutine(DamageOverTime(player));
            }

            if (slowsPlayer)
            {
                player.SetMoveSpeed(slowMultiplier);
            }

            if (hitSFX)
            {
                AudioSource.PlayClipAtPoint(hitSFX, transform.position);
            }

            if (player.CurrentHealth <= 0)
            {
                player.Respawn(); // Custom respawn function from your PlayerBehaviour
            }
        }
    }

    System.Collections.IEnumerator DamageOverTime(PlayerBehaviour player)
    {
        while (player != null && player.CurrentHealth > 0)
        {
            player.TakeDamage(damageAmount);
            yield return new WaitForSeconds(damageInterval);
        }
    }
}
