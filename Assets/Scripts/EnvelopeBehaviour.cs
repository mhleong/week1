/*
* Author: Ming Hui
* Date: 10/6/25
* Description: Envelope Behaviour script, allows collecting of 
*/


using UnityEngine;

public class EnvelopeBehaviour : MonoBehaviour
{
    // Coin value that will be added to the player's score
    [SerializeField]
    int EnvelopeValue = 1;

    [SerializeField]
    AudioClip collectSound;


    // Method to collect the envelope
    // This method will be called when the player interacts with the envelope
    // It takes a PlayerBehaviour object as a parameter
    // This allows the coin to modify the player's score
    // The method is public so it can be accessed from other scripts
    public void Collect(PlayerBehaviour player)
    {
        AudioSource.PlayClipAtPoint(collectSound, transform.position);
        // Logic for collecting the coin
        Debug.Log("Envelope collected!");

        // Add the envelope value to the player's score
        // This is done by calling the ModifyScore method on the player object
        // The coinValue is passed as an argument to the method
        // This allows the player to gain points when they collect the envelope
        player.ModifyScore(EnvelopeValue);

        Destroy(gameObject); // Destroy the envelope object

    }

    MeshRenderer myMeshRenderer;
    [SerializeField] Material highlightMaterial; // Material to highlight the envelope
    [SerializeField] Material originalMaterial; // Default material for the envelope

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myMeshRenderer = GetComponent<MeshRenderer>();

        originalMaterial = myMeshRenderer.material; // Store the original material
    }
    public void Highlight()
    {
        // Logic to highlight the envelope (e.g., change color, add visual effects)
        myMeshRenderer.material = highlightMaterial; // Change the material to highlight
    }
    public void Unhighlight()
    {
        // Logic to remove the highlight from the envelope
        myMeshRenderer.material = originalMaterial; // Change back to the original material

    }
}
