using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    string message = "Food Club chicken rice is the best!";
    int currentHealth = 100;
    float movementSpeed = 5.5f;
    bool isPlayerAlive = false;

    int attack = 50;
    int defense = 20;

    void Start()
    {
        //Task 1: (prints 1 to 10 in single line)
        string numbers = ""; 

        for (int i = 1; i <= 10; i++)
        {
            numbers += i + " ";
        }

        Debug.Log(numbers); 

        //Task 2:
        // Create 2 int variables
        //Using a while loop, increase the value of the smaller int by 1 until it is equal to
        //the value of the larger int
        //Print how many increments it took to make them equal
    
        int increments = 0;

        if (defense > attack)
        {
            int basee = defense;  //basee is a temporary variable to jus swap the values of def n atk to make sure smller num always first
            defense = attack;
            attack = basee;
        }

        while (defense < attack)
        {
            defense++;
            increments++;
        }

        Debug.Log("It took " + increments + " increments to make defense and attack equal");
    }

    void Update()
    {
        
    }
}
