using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    int health = 10;
    int coinScore = 4;
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("player had a collision with:" + collision.gameObject.name);
        if(collision.gameObject.CompareTag("Recovery"))
        {
            ++health;
            Debug.Log("Player health: " + health);
        }

   

    }


    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name + "entered the trigger");
        if (other.gameObject.CompareTag("Collectible"))
        {
            //if the object is a collectible, i can interact with it
            Debug.Log("Collectible object detected");

        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
