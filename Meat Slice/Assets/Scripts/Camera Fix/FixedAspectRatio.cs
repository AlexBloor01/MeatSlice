using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedAspectRatio : MonoBehaviour
{
    public float targetWidth = 640f;
    public float targetHeight = 960f;
    public float targetFOV = 65f;


    void Start()
    {
        FixAspectRatios();
    }

    //When the mobile changes the resizes the window.
    private void OnRectTransformDimensionsChange()
    {
        FixAspectRatios();
    }

    void FixAspectRatios()
    {
        float targetAspect = targetHeight / targetWidth;
        float currentAspect = (float)Screen.height / Screen.width;

        float differenceInAspect = Mathf.Abs(targetAspect - currentAspect);
        differenceInAspect += (differenceInAspect / 2);

        float fov = targetFOV + (differenceInAspect * 10);

        Camera.main.fieldOfView = fov;
    }
}
