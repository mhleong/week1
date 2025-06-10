using UnityEngine;

public class EnvelopeBehaviour : MonoBehaviour
{
    // Coin value that will be added to the player's score
    [SerializeField]
    int EnvelopeValue = 1;

    // Method to collect the coin
    // This method will be called when the player interacts with the coin
    // It takes a PlayerBehaviour object as a parameter
    // This allows the coin to modify the player's score
    // The method is public so it can be accessed from other scripts
    public void Collect(PlayerBehaviour player)
    {
        // Logic for collecting the coin
        Debug.Log("Envelope collected!");

        // Add the coin value to the player's score
        // This is done by calling the ModifyScore method on the player object
        // The coinValue is passed as an argument to the method
        // This allows the player to gain points when they collect the coin
        player.ModifyScore(EnvelopeValue);

        Destroy(gameObject); // Destroy the coin object
    }
}
