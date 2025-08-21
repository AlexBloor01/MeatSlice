using System;
using System.Collections;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Coroutine moveCoroutine; //Allows stopping and starting of movement for chopping function in food controller.

    public Transform centerPoint; //Controls the center of where the block will intersect.
    public Transform spawnPointA; //Possible food spawn position.
    public Transform spawnPointB; //Possible food spawn position.
    public Transform currentMovementPoint; //Current position the food will spawn from.

    public Vector3 centerPointCenter; //Returns to this point when game restarts.

    public float speed = 1; //Speed of PingPong movement. This could increase difficulty
    public const float maxSpeed = 3;
    public const float minSpeed = 0.1f;

    private const float lowerSlowMoPercentage = 0.4f; //Lowest required position in lerp to start slow motion.
    private const float higherSlowMoPercentage = 0.6f; //highest required position in lerp to start slow motion.

    private void Start()
    {
        SetupVariables();
    }

    void SetupVariables()
    {
        centerPointCenter = transform.position;

        speed = PlayerPrefs.GetFloat("Game_Speed", 1);

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
        SwitchStartPosition();
        Vector3 startPosition = currentMovementPoint.transform.position;
        Vector3 targetPosition = TargetPosition();

        float elapsedTime = 0f;

        //Makes sure that the previous requests have an extra frame to complete.
        yield return null;

        //Continue until StopMovement is played.
        while (GameManager.gameOver == false)
        {
            elapsedTime += Time.deltaTime * speed;
            float pingPong = Mathf.PingPong(elapsedTime, 1f);
            transform.position = Vector3.Lerp(startPosition, targetPosition, pingPong);

            if (GameManager.slowMode)
            {
                if (pingPong > lowerSlowMoPercentage && pingPong < higherSlowMoPercentage)
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
    void SwitchStartPosition()
    {
        currentMovementPoint = (currentMovementPoint == spawnPointA) ? spawnPointB : spawnPointA;
        transform.position = currentMovementPoint.transform.position;
    }

    //Get the opposite position to the startPosition but not inverting the incorrect axis.
    Vector3 TargetPosition()
    {
        if (currentMovementPoint == spawnPointA)
        {
            return new Vector3(currentMovementPoint.transform.position.x, 0f, -currentMovementPoint.transform.position.z);
        }
        else
        {
            return new Vector3(-currentMovementPoint.transform.position.x, 0f, currentMovementPoint.transform.position.z);
        }
    }

    public bool IsSpawnPointA()
    {
        return currentMovementPoint == spawnPointA;
    }
}
