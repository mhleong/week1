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
    [SerializeField] public bool isIcyHazard = false;
    [SerializeField] float slowMultiplier = 0.5f; //frozen pond
    [SerializeField] AudioClip hitSFX;


    [Header("Falling Hazard Settings")]
    [SerializeField] bool isFallingHazard = false; // Enabled for falling icicles
    [SerializeField] float triggerRadius = 15f; // How close player must be to trigger fall
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
                Debug.Log("Beware of falling icicles");
                Invoke(nameof(ActivateFall), delay);
            }
        }
    }


    void ActivateFall()
    {
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.AddForce(Vector3.down * 200f, ForceMode.Impulse); //make icicle fall faster
        }
    }

    // Detect collision for physical hazards like falling icicles
    private void OnCollisionEnter(Collision collision)
    {
        if (isFallingHazard && rb != null && !rb.isKinematic)
        {
            // Always embed once it touches ground or player
            rb.isKinematic = true;

            Vector3 pos = transform.position;
            pos.y += embedOffsetY;
            transform.position = pos;
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            HandleHazardEffects(collision.gameObject);
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

                // Trigger frost overlay if this hazard is icy
                if (isIcyHazard)
                {
                    player.TriggerFrostOverlay();
                }

                if (hitSFX)
                {
                    AudioSource.PlayClipAtPoint(hitSFX, transform.position);
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
            else if (!activeDOTs.ContainsKey(playerObj))
            {
                Coroutine dotCoroutine = StartCoroutine(DamageOverTime(player));
                activeDOTs[playerObj] = dotCoroutine;
            }

            if (slowsPlayer)
            {
                player.SetMoveSpeed(slowMultiplier);
            }

            if (hitSFX)
            {
                AudioSource.PlayClipAtPoint(hitSFX, transform.position);
            }
        }
    }

    System.Collections.IEnumerator DamageOverTime(PlayerBehaviour player)
    {
        GameObject playerObj = player.gameObject;

        while (player != null && player.CurrentHealth > 0)
        {
            player.TakeDamage(damageAmount);
            yield return new WaitForSeconds(damageInterval);
        }

        if (activeDOTs.ContainsKey(playerObj))
        {
            activeDOTs.Remove(playerObj);
        }
    }
    

    // Called by PlayerBehaviour to clean up DOT when respawning
    public static void StopAllDOTForPlayer(GameObject player)
    {
        foreach (Hazard hazard in FindObjectsByType<Hazard>(FindObjectsSortMode.None))
        {
            if (hazard.activeDOTs.ContainsKey(player))
            {
                hazard.StopCoroutine(hazard.activeDOTs[player]);
                hazard.activeDOTs.Remove(player);
            }
        }
    }
}
