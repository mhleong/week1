/*
* Author: Ming Hui
* Date: 10/6/25
* Description: Door Behaviour script, allows opening of door
*/


using UnityEngine;

public class DoorBehaviour : MonoBehaviour
{
    // Flag to determine if the door is open
    bool isOpen = false;

    // Flag to determine if the door is locked
    [SerializeField]
    bool isLocked = true;
    [SerializeField] private bool isFinalDoor = false;

    [SerializeField] AudioClip doorSound;

    // Reference to the player inventory to check if player has key
    [SerializeField]
    PlayerInventory playerInventory;

    // Distance player must move away before door auto-closes
    [SerializeField]
    float closeDistance = 3f;

    // Reference to the player transform
    private Transform playerTransform;
    private bool playerWasInside = false;


    void Start()
    {
        // Find the player by tag at start
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    void Update()
    {
        // Continuously check distance only if door is open and player exited before
        if (isOpen && playerWasInside && playerTransform != null)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            if (distance > closeDistance)
            {
                ToggleDoor();
                playerWasInside = false; // reset so it doesn't keep toggling
            }
        }
    }


    // Interact method called when player presses Interact button
    public void Interact()
    {

        if (isLocked)
        {
            if (playerInventory != null && playerInventory.HasKey)
            {
                isLocked = false; // Unlock door
                ToggleDoor(); // Open door
            }
            else
            {
                Debug.Log("Door is locked! Find the key!");
            }
        }
        else
        {
            ToggleDoor(); // Open or close
        }
    }


    void ToggleDoor()  // Opens or closes the door by rotating it
    {
        Vector3 doorRotation = transform.eulerAngles;
        if (!isOpen)
        {
            doorRotation.y += 90f;
            isOpen = true;
        }
        else
        {
            doorRotation.y -= 90f;
            isOpen = false;
            playerWasInside = true; // mark player as inside for distance check
        }

        transform.eulerAngles = doorRotation;
        AudioSource.PlayClipAtPoint(doorSound, playerTransform.position, 1.0f);
        // transform.GetChild: plays the sound at the position of the actual visible door (the first child)
        //1.0f = loudness of audioclip

    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerWasInside = true; // Enable distance check in Update
        }
    }

}