/*
* Author: Ming Hui
* Date: 10/6/25
* Description: Door Behaviour script, allows opening of door
*/


using UnityEngine;

public class DoorBehaviour : MonoBehaviour
{
    public void Interact()
    {
        Vector3 doorRotation = transform.eulerAngles;
        if (doorRotation.y == 0f)
        {
            doorRotation.y += 90f;
            transform.eulerAngles = doorRotation;
        }
        else if (doorRotation.y == 90f)
        {
            doorRotation.y = 0f;
            transform.eulerAngles = doorRotation;
        }
    }

}