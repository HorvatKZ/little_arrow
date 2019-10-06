using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform player;
    public float Distnace;
    
    void FixedUpdate()
    {
        if (player != null)
        {
            transform.position = player.position + new Vector3(0, 0, -Distnace);
        }
    }
}
