/*
* Author: Ming Hui
* Date: 10/6/25
* Description: Player behaviour script with collection of envelopes
*/

using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    // Player's maximum health
    int maxHealth = 100;
    // Player's current health
    int currentHealth = 100;
    // Player's current score
    int currentScore = 0;
    // Flag to check if the player can interact with objects
    bool canInteract = false;
    // Stores the current object the player has detected
    EnvelopeBehaviour currentEnvelope = null;
    DoorBehaviour currentDoor = null;


    [SerializeField]
    GameObject projectile;

    [SerializeField]
    Transform spawnPoint;

    [SerializeField]
    float fireForceStrength;

    [SerializeField] float interactionDistance = 50f;



void Update()
    {
        RaycastHit hitInfo;
        Debug.DrawRay(spawnPoint.position, spawnPoint.forward * interactionDistance, Color.blue); // Visualize the raycast in the scene view
        if (Physics.Raycast(spawnPoint.position, spawnPoint.forward, out hitInfo, interactionDistance))
            if (hitInfo.collider.CompareTag("Collectible"))
            {
                if (currentEnvelope != null)
                {
                    currentEnvelope.Unhighlight(); // Remove highlight from the previous envelope 
                }

                currentEnvelope = hitInfo.collider.gameObject.GetComponent<EnvelopeBehaviour>(); // Store the envelope 
                if (currentEnvelope != null)
                {
                    currentEnvelope.Highlight();
                }

            }
        else
        {
            if (currentEnvelope != null)
            {
                currentEnvelope.Unhighlight(); // Remove highlight if no envelope is hit
                currentEnvelope = null; // Clear the reference to the highlighted envelope
            }
        }

    }

    // The Interact callback for the Interact Input Action
    // This method is called when the player presses the interact button
    void OnInteract()
    {
        // Check if the player can interact with objects
        if (canInteract)
        {
            // Check if the player has detected a coin or a door
            if (currentEnvelope != null)
            {
                Debug.Log("Interacting with Envelope");
                // Call the Collect method on the object
                // Pass the player object as an argument
                currentEnvelope.Collect(this);
            }

            else if (currentDoor != null)
            {
                Debug.Log("Interacting with door");
                currentDoor.Interact();
            }

            else
            {
                RaycastHit hitInfo;
                if (Physics.Raycast(spawnPoint.position, spawnPoint.forward, out hitInfo, interactionDistance))
                {
                    if (hitInfo.collider.CompareTag("Collectible"))
                    {
                        KeyBehaviour key = hitInfo.collider.GetComponent<KeyBehaviour>();
                        if (key != null)
                        {
                            key.Collect(this);
                        }
                    }
                }
            }

        }
    }



    // Method to modify the player's score
    // This method takes an integer amount as a parameter
    // It adds the amount to the player's current score
    // The method is public so it can be accessed from other scripts
    public void ModifyScore(int amt)
    {
        // Increase currentScore by the amount passed as an argument
        currentScore += amt;
    }

    // Method to modify the player's health
    // This method takes an integer amount as a parameter
    // It adds the amount to the player's current health
    // The method is public so it can be accessed from other scripts
    public void ModifyHealth(int amount)
    {
        // Check if the current health is less than the maximum health
        // If it is, increase the current health by the amount passed as an argument
        if (currentHealth < maxHealth)
        {
            currentHealth += amount;
            // Check if the current health exceeds the maximum health
            // If it does, set the current health to the maximum health
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
        }
    }


    // Trigger Callback for when the player enters a trigger collider
    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        // Check if the player detects a trigger collider tagged as "Collectible" or "Door"
        if (other.CompareTag("Collectible"))
        {
            // Set the canInteract flag to true
            // Get the CoinBehaviour component from the detected object
            canInteract = true;
            currentEnvelope = other.GetComponent<EnvelopeBehaviour>();
        }

        else if (other.CompareTag("Door"))
        {
            canInteract = true;
            currentDoor = other.GetComponent<DoorBehaviour>();
        }
    }

    // Trigger Callback for when the player exits a trigger collider
    // Trigger Callback for when the player exits a trigger collider
    void OnTriggerExit(Collider other)
    {
        if (currentEnvelope != null && other.gameObject == currentEnvelope.gameObject)
        {
            canInteract = false;
            currentEnvelope = null;
        }
        else if (currentDoor != null && other.gameObject == currentDoor.gameObject)
        {
            canInteract = false;
            currentDoor = null;
        }
    }


    void OnFire()
    {
        // Instantiate a new projectile at the spawn point's position and rotation
        //Store the spawned projectile to the 'newProjectile' variable
        GameObject newProjectile = Instantiate(projectile, spawnPoint.position, spawnPoint.rotation);

        // Create a new Vector3 variable 'fireForce'
        // Set it to the forward direction of the spawn point multiplied by the fire strength
        // This will determine the direction and speed of the projectile
        Vector3 fireForce = spawnPoint.forward * fireForceStrength;

        // Get the Rigidbody component of the new projectile
        // Add a force to the projectile defined by the fireforce variable
        newProjectile.GetComponent<Rigidbody>().AddForce(fireForce);
    }

}

