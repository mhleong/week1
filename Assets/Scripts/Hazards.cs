/*
 * Author: Ming Hui
 * Date: 11/06/2025
 * Description: Script for hazards and player taking dmg
 */


using UnityEngine;

public class Hazard : MonoBehaviour
{
    [SerializeField] int damageAmount = 10;
    [SerializeField] float damageInterval = 1f; // Time between damage ticks
    [SerializeField] bool isInstant = false;
    [SerializeField] bool slowsPlayer = false;
    [SerializeField] float slowMultiplier = 0.5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerBehaviour player = other.GetComponent<PlayerBehaviour>();

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
        }
    }

    System.Collections.IEnumerator DamageOverTime(PlayerBehaviour player)
    {
        while (player != null && GetComponent<Collider>().bounds.Contains(player.transform.position))
        {
            player.TakeDamage(damageAmount);
            yield return new WaitForSeconds(damageInterval);
        }
    }
}

