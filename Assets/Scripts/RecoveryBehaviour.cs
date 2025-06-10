using UnityEngine;

public class RecoveryBehaviour : MonoBehaviour
{
    // Amount of health to recover
    [SerializeField]
    int healAmount = 5;

    // Method to recover health
    // This method will be called when the player interacts with the recovery object
    // It takes a PlayerBehaviour object as a parameter
    // This allows the recovery object to modify the player's health
    // The method is public so it can be accessed from other scripts
    public void RecoverHealth(PlayerBehaviour player)
    {
        // Calls the ModifyHealth method on the player object
        // The healAmount is passed as an argument to the method
        // This allows the player to gain health as long as they touch the recovery object
        player.ModifyHealth(healAmount);
    }
}
