/*
 * Author: Ming Hui
 * Date: 11/06/2025
 * Description: Script for collecting keys to unlock doors
 */

using UnityEngine;

public class KeyBehaviour : MonoBehaviour
{
    [SerializeField]
    AudioClip collectSound;

    // Called when player collects the key
    public void Collect(PlayerBehaviour player)
    {
        AudioSource.PlayClipAtPoint(collectSound, transform.position);
        player.GetComponent<PlayerInventory>().HasKey = true;
        Debug.Log("Key collected!");
        Destroy(gameObject);
    }
}