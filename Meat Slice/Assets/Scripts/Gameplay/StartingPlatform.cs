using System.Collections;
using UnityEngine;

public class StartingPlatform : MonoBehaviour
{
    [SerializeField] private PlayerController playerController; //Reference to StartingPlatform script in scene.
    public GameObject startChildObject; //The physical platform Child under this Game Object.
    private float downOnceDuration = 1f; //Duration of lerp. This can be used for different Speed modes if the player wants.
    private Vector3 startPosition; //This is the starting position of the StartingPlatform when game is first started.
    Coroutine moveCoroutine; //Coroutine used for movement, allows stopping and starting.
    Vector3 lastFoodPosition; //Position of the last food that was placed.
    public AnimationCurve traverseAnimCurve; //Traverse lerp animation curve 
    readonly float speedDivider = 2;
    // readonly float maxTraverseDuration = 0.5f; //Maximum duration for traverse lerp to take.


    private void Start()
    {
        SetupVariables();
    }

    //Sets up script variables at start.
    void SetupVariables()
    {
        startPosition = transform.position;

        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }
    }

    //Activate a new moveCoroutine from outside this script.
    public void MoveToNext(bool newFood, float yMovement)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine = StartCoroutine(MoveToNextYPosition(newFood, yMovement, downOnceDuration));
    }

    //Moves To the yMovement position along the y axis then continues to next generation of the game loop.
    //isNewFood is true if we want to instantiate a new food object once moved down.
    //yMovement is the amount of space to move down on the y axis, this will the distance between half of two objects y scale.
    private IEnumerator MoveToNextYPosition(bool isNewFood, float yMovement, float duration)
    {
        float percentage = 0f;
        float timer = 0f;
        float newYPos = yMovement;

        if (isNewFood == false)
        {
            newYPos = (yMovement / 2) - Mathf.Abs(transform.position.y);
        }

        Vector3 newPosition = new Vector3(
            transform.position.x,
            transform.position.y - newYPos,
            transform.position.z);

        while (percentage <= 1f)
        {
            timer += Time.deltaTime;
            percentage = timer / duration;
            transform.position = Vector3.Lerp(transform.position, newPosition, percentage);
            yield return null;
        }

        if (isNewFood)
        {
            playerController.InstantiateNewFood();
            lastFoodPosition = transform.position;
        }

        yield return null;
    }

    //This is played when game is reset.
    public void ReturnToOrigin()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        transform.position = startPosition;
    }

    public void TraverseTower()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        if (lastFoodPosition == null || lastFoodPosition == Vector3.zero)
        {
            return;
        }

        Vector3 target = (Vector3.Distance(transform.position, lastFoodPosition) > Vector3.Distance(transform.position, startPosition)) ? lastFoodPosition : startPosition;


        float speed = Vector3.Distance(transform.position, target) / speedDivider;

        // speed = Mathf.Clamp(speed, minTraverseSpeed, float.MaxValue);

        moveCoroutine = StartCoroutine(MovementLibrary.AnimationCurveLerp(gameObject, speed, transform.position, target, false, MovementLibrary.ObjectLerpType.Position, traverseAnimCurve, null));
    }

}
