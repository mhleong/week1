/*
* Author: Ming Hui
* Date: 10/6/25
* Description: Player behaviour script with collection of envelopes 
* Handles player movement, interaction with game objects (collectibles, doors, keys),
* health management, damage from hazards, UI updates (score, health, messages),
* and triggers special effects like the frost overlay when taking damage from icy hazards
*/

using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

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

    // Stores the current objects the player has detected
    private EnvelopeBehaviour currentEnvelope;
    private GiftBehaviour currentGift;
    private KeyBehaviour currentKey;
    private DoorBehaviour currentDoor;
    //player default n current speed
    float defaultSpeed = 5f;
    float moveSpeed;


    [SerializeField]
    GameObject projectile; //just in case for future usage

    [SerializeField]
    float fireForceStrength; 

    [SerializeField]
    Transform spawnPoint; //spawn for raycasting

    [SerializeField] float interactionDistance = 10f;

    [SerializeField] Transform respawnPoint; //for player respawn

    [SerializeField] TextMeshProUGUI scoreText; //score ui
    [SerializeField] TextMeshProUGUI healthText; //health ui
    [SerializeField] TextMeshProUGUI envelopeCounterText; //env ui
    [SerializeField] TextMeshProUGUI giftCounterText; //gift ui
    [SerializeField] GameObject congratsMessagePanel; // only show congrats UI when everything is collected
    [SerializeField] AudioClip congratsSound;
    [SerializeField] int totalEnvelopes = 20; //number of envelopes + gifts
    [SerializeField] int totalGifts = 10;


    [Header("Frost Overlay")] //effect when players hit by icicle/frozen pond/ice puddle
    public Image overlay;
    public float duration =2f; //how long it stays
    public float fadeSpeed = 1f; //how fast it fades in and out

    private float durationTimer = 0f; //timer to check against the duration


    //for no. of collectibles left and congrats UI
    int collectedEnvelopes = 0;
    int collectedGifts = 0;


    /// UI panel shown when door is locked and key is needed
    public GameObject doorMessageUI;


    /// Time to show the door locked msg
    public float doorMessageDuration = 2.5f;

    private Coroutine doorMessageCoroutine;

    // Call this when player tries a locked door without key
    public void ShowDoorLockedMessage()
    {
        if (doorMessageCoroutine != null)
            StopCoroutine(doorMessageCoroutine);

        doorMessageCoroutine = StartCoroutine(DisplayDoorMessage());
    }

    private IEnumerator DisplayDoorMessage()
    {
        doorMessageUI.SetActive(true);
        yield return new WaitForSeconds(doorMessageDuration);
        doorMessageUI.SetActive(false);
    }


    void Start()
    {
        moveSpeed = defaultSpeed;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0f);
        scoreText.text = "SCORE:" + currentScore.ToString();
        healthText.text = "HEALTH: " + currentHealth;

        // Auto-detect collectibles
        totalEnvelopes = FindObjectsByType<EnvelopeBehaviour>(FindObjectsSortMode.None).Length;
        totalGifts = FindObjectsByType<GiftBehaviour>(FindObjectsSortMode.None).Length;
        envelopeCounterText.text = "0/" + totalEnvelopes;
        giftCounterText.text = "0/" + totalGifts;

        if (doorMessageUI != null)
            doorMessageUI.SetActive(false);

        if (congratsMessagePanel != null)
        {
            Debug.Log("Hiding Congrats!");
            congratsMessagePanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Congrats image not assigned!");
        }
    
    }

    public void SetMoveSpeed(float multiplier)
    {
        moveSpeed = defaultSpeed * multiplier;
    }

    public void ResetMoveSpeed()
    {
        moveSpeed = defaultSpeed;
    }

    public int CurrentHealth => currentHealth; //for hazards.cs

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Respawn();
        }
        healthText.text = "HEALTH: " + currentHealth;


    }


    /// Triggers the frost overlay effect (used only for icy hazards).
    public void TriggerFrostOverlay()
    {
        durationTimer = 0f; //reset timer
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 1f);
    }

    public void Respawn()
    {
        Hazard.StopAllDOTForPlayer(gameObject); // Stop all ongoing DOT
        transform.position = respawnPoint.position;
        currentHealth = maxHealth;
        healthText.text = "HEALTH: " + currentHealth;
    }


    void Update()
    {
        RaycastHit hitInfo;
        Debug.DrawRay(spawnPoint.position, spawnPoint.forward * interactionDistance, Color.red);

        if (Physics.Raycast(spawnPoint.position, spawnPoint.forward, out hitInfo, interactionDistance))
        {
            GameObject hitObject = hitInfo.collider.gameObject;

            // Check for envelope
            EnvelopeBehaviour envelope = hitObject.GetComponent<EnvelopeBehaviour>();
            if (envelope != null)
            {
                if (currentEnvelope != null && currentEnvelope != envelope)
                    currentEnvelope.Unhighlight();
                if (currentGift != null)
                    currentGift.Unhighlight();

                currentEnvelope = envelope;
                currentGift = null;
                envelope.Highlight();
                return;
            }

            // Check for gift
            GiftBehaviour gift = hitObject.GetComponent<GiftBehaviour>();
            if (gift != null)
            {
                if (currentGift != null && currentGift != gift)
                    currentGift.Unhighlight();
                if (currentEnvelope != null)
                    currentEnvelope.Unhighlight();

                currentGift = gift;
                currentEnvelope = null;
                gift.Highlight();
                return;
            }

            // Check for key
            KeyBehaviour key = hitObject.GetComponent<KeyBehaviour>();
            if (key != null)
            {
                if (currentKey != null && currentKey != key)
                    currentKey.Unhighlight();
                if (currentEnvelope != null)
                    currentEnvelope.Unhighlight();
                if (currentGift != null)
                    currentGift.Unhighlight();

                currentKey = key;
                currentEnvelope = null;
                currentGift = null;
                key.Highlight();
                return;
            }


            // Check for door
            DoorBehaviour door = hitObject.GetComponent<DoorBehaviour>();
            if (door != null)
            {
                currentDoor = door;
            }
            else
            {
                currentDoor = null;
            }
        }
        else
        {
            // Clear highlights if ray hits nothing
            if (currentEnvelope != null)
            {
                currentEnvelope.Unhighlight();
                currentEnvelope = null;
            }
            if (currentGift != null)
            {
                currentGift.Unhighlight();
                currentGift = null;
            }

            if (currentKey != null)
            {
                currentKey.Unhighlight();
                currentKey = null;
            }

            currentDoor = null;
        }

        // Frost overlay fade logic for icy hazards
        if (overlay.color.a > 0f)
        {
            durationTimer += Time.deltaTime;

            float alpha = overlay.color.a;

            if (durationTimer <= duration)
            {
                // Wait before fading
                return;
            }

            alpha -= Time.deltaTime * fadeSpeed;
            overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, Mathf.Max(0f, alpha));
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

            else if (currentGift != null)
            {
                Debug.Log("Interacting with Gift");
                currentGift.Collect(this);
            }
            
            else if (currentKey != null)
            {
                Debug.Log("Interacting with Key");
                currentKey.Collect(this);
            }

            else if (currentDoor != null)
            {
                Debug.Log("Interacting with door");
                currentDoor.Interact(this);
            }

            else
            {
                RaycastHit hitInfo;
                if (Physics.Raycast(spawnPoint.position, spawnPoint.forward, out hitInfo, interactionDistance))
                {
                    GameObject obj = hitInfo.collider.gameObject;

                    if (obj.TryGetComponent<KeyBehaviour>(out KeyBehaviour key))
                    {
                        key.Collect(this);
                    }
                    else if (obj.TryGetComponent<GiftBehaviour>(out GiftBehaviour gift))
                    {
                        gift.Collect(this);
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
        scoreText.text = "SCORE:" + currentScore.ToString();
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
        healthText.text = "HEALTH: " + currentHealth;
    }

    //UI trackingfor envelope + gifts
    public void CollectEnvelope()
    {
        collectedEnvelopes++;
        envelopeCounterText.text = collectedEnvelopes + "/" + totalEnvelopes;
        CheckAllCollected();
    }

    public void CollectGift()
    {
        collectedGifts++;
        giftCounterText.text = collectedGifts + "/" + totalGifts;
        CheckAllCollected();
    }

    void CheckAllCollected()
    {
        if (collectedEnvelopes >= totalEnvelopes && collectedGifts >= totalGifts)
        {
            if (congratsMessagePanel != null)
            {
                congratsMessagePanel.SetActive(true);
                AudioSource.PlayClipAtPoint(congratsSound, transform.position);
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
            // Get the EnvelopeBehaviour component from the detected object
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



