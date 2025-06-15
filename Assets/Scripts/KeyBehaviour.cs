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


    MeshRenderer myMeshRenderer;
    [SerializeField] Material highlightMaterial; // Material to highlight the key
    [SerializeField] Material originalMaterial; // Default material for the key

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myMeshRenderer = GetComponent<MeshRenderer>();

        originalMaterial = myMeshRenderer.material; // Store the original material
    }
    public void Highlight()
    {
        // Logic to highlight the key (e.g., change color, add visual effects)
        myMeshRenderer.material = highlightMaterial; // Change the material to highlight
    }
    public void Unhighlight()
    {
        // Logic to remove the highlight from the key
        myMeshRenderer.material = originalMaterial; // Change back to the original material

    }
    

    // Called when player collects the key
    public void Collect(PlayerBehaviour player)
    {
        AudioSource.PlayClipAtPoint(collectSound, transform.position);
        player.GetComponent<PlayerInventory>().HasKey = true;
        Debug.Log("Key collected!");
        Destroy(gameObject);
    }
}