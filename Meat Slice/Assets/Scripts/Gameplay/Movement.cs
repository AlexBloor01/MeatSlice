using System;
using System.Collections;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Coroutine moveCoroutine; //Allows stopping and starting of movement for chopping function in food controller.

    public Transform centerPoint; //Controls the center of where the block will intersect.
    public Transform spawnPointA; //Possible food spawn position.
    public Transform spawnPointB; //Possible food spawn position.
    public Transform startMovementPoint; //Current position the food will spawn from.

    public Vector3 centerPointCenter;

    private float speed = 1;

    private const float lowerPercentage = 0.4f;
    private const float higherPercentage = 0.6f;

    private void Start()
    {
        SetupVariables();
    }

    void SetupVariables()
    {
        centerPointCenter = transform.position;
    }

    //Stops movement, play this from other scripts.
    public void StopMovement()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
        if (GameManager.slowMode)
        {
            Time.timeScale = 1f;
        }
    }

    //Starts movement, play this from other scripts as it will allow stopping later.
    public void StartMovement(Vector3 currentPosition)
    {
        StopMovement();
        SetCenterPoint(currentPosition);
        moveCoroutine = StartCoroutine(PingPongMovement());
    }

    public void SetCenterPoint(Vector3 currentPosition)
    {
        centerPoint.position = new Vector3(currentPosition.x, 0f, currentPosition.z);
    }

    //Move the holder back and forth.
    private IEnumerator PingPongMovement()
    {
        Vector3 startPosition = SwitchStartPosition();
        Vector3 target = TargetPosition(startPosition);

        float elapsedTime = 0f;

        //Makes sure that the previous requests have an extra frame to complete.
        yield return null;

        //Continue until StopMovement is played.
        while (true)
        {
            elapsedTime += Time.deltaTime * speed;
            float pingPong = Mathf.PingPong(elapsedTime, 1f);
            transform.position = Vector3.Lerp(startPosition, target, pingPong);

            if (GameManager.slowMode)
            {
                if (pingPong > lowerPercentage && pingPong < higherPercentage)
                {
                    Time.timeScale = GameManager.slowSpeed;
                }
                else
                {
                    Time.timeScale = 1f;
                }
            }

            yield return null;
        }

    }

    //Get the start position based on the opposite SpawnPoint.
    Vector3 SwitchStartPosition()
    {
        startMovementPoint = (startMovementPoint == spawnPointA) ? spawnPointB : spawnPointA;
        transform.position = startMovementPoint.position;
        return startMovementPoint.transform.position;
    }

    //Get the opposite position to the startPosition but not inverting the incorrect axis.
    Vector3 TargetPosition(Vector3 startPosition)
    {
        if (startMovementPoint == spawnPointA)
        {
            return new Vector3(startPosition.x, 0f, -startPosition.z);
        }
        else
        {
            return new Vector3(-startPosition.x, 0f, startPosition.z);
        }
    }

}
