using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloudTransition : MonoBehaviour
{
    public static CloudTransition iMenuTransition; //Reference to self.
    public RawImage UIImage; //Reference to UI element that renders this transition.

    public Transform[] clouds; //Select all of the clouds that are required to  be 
    public float transitionDuration = 3f; //Amount of time for transition to last.
    public Vector2 randomScale = new Vector2(1.3f, 2f); //Random scale to choose between, we want to cover the screen.
    public AnimationCurve transitionCurve; //Animation curve that controls the lerp position.

    void Start()
    {
        SetupVariables();
    }

    //Sets up script.
    void SetupVariables()
    {
        iMenuTransition = this;
        UIImage.gameObject.SetActive(false);
    }

    //Transition by scaling the clouds array transforms that are already in the scene.
    //middleTransitionAction activates right after majority clouds have reached their largest scale.
    //endTransitionAction activates after transition has happened and after ui image has been disabled.
    //alterTransitionDuration when above 0 replaces transitionDuration.
    public IEnumerator MenuTransition(Action startAction, Action middleAction, Action endAction, float alterTransitionDuration)
    {
        bool isStartAction = false;
        float duration = transitionDuration;

        if (alterTransitionDuration > 0)
        {
            duration = alterTransitionDuration;
        }

        //Turn on visual and stop button presses for happening below the ui sort position.
        UIImage.gameObject.SetActive(true);
        UIImage.raycastTarget = true;

        for (int cloudIndex = 1; cloudIndex < clouds.Length; cloudIndex++)
        {
            //Pick new Scale for cloud.
            Vector3 newScale = VectorLibrary.RandomVector3Whole(randomScale.x, randomScale.y);

            //Animation Lerp between 0 and newScale.
            StartCoroutine(MovementLibrary.AnimationCurveLerp(clouds[cloudIndex].gameObject, duration, Vector3.zero, newScale, false, MovementLibrary.ObjectLerpType.LocalScale, transitionCurve, null));

            //ADD POP AND OR POOF SOUND

            if (cloudIndex >= clouds.Length / 2 && isStartAction == false)
            {
                isStartAction = true;
                startAction?.Invoke();
            }

            //Wait a period of time to activate next cloud for smoother effect.
            yield return new WaitForSecondsRealtime((duration / cloudIndex) / clouds.Length);
        }

        middleAction?.Invoke();
        yield return new WaitForSecondsRealtime(Numbers.GetPercent(duration, 70));
        UIImage.raycastTarget = false;
        yield return new WaitForSecondsRealtime(Numbers.GetPercent(duration, 30));
        UIImage.gameObject.SetActive(false);
        endAction?.Invoke();
        yield return null;
    }

}
