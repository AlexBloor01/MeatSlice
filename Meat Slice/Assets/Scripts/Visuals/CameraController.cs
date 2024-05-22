using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Vector3 startingPosition;

    private void Awake()
    {
        startingPosition = transform.position;
    }

}
