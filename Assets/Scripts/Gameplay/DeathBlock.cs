using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBlock : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<MarkedForDeath>() == null)
        {
            other.gameObject.AddComponent<MarkedForDeath>();

        }
    }

}