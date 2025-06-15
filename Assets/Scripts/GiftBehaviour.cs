/*
 * Author: Ming Hui
 * Date: 11/06/2025
 * Description: Script for collecting rare gifts which only appears at certain areas of the level
 */

using UnityEngine;

public class GiftBehaviour : MonoBehaviour
{
    [SerializeField] int giftValue = 5;
    [SerializeField] AudioClip collectSound; 

    public void Collect(PlayerBehaviour player)
    {
        AudioSource.PlayClipAtPoint(collectSound, transform.position);
        Debug.Log("Gift collected!");
        player.ModifyScore(giftValue);
        player.CollectGift();
        Destroy(gameObject);
    }

    MeshRenderer myMeshRenderer;
    [SerializeField] Material highlightMaterial; // Material to highlight the gift
    [SerializeField] Material originalMaterial; // Default material for the gift

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myMeshRenderer = GetComponent<MeshRenderer>();

        originalMaterial = myMeshRenderer.material; // Store the original material
    }
    public void Highlight()
    {
        // Logic to highlight the gift (e.g., change color, add visual effects)
        myMeshRenderer.material = highlightMaterial; // Change the material to highlight
    }
    public void Unhighlight()
    {
        // Logic to remove the highlight from the gift
        myMeshRenderer.material = originalMaterial; // Change back to the original material

    }
}



