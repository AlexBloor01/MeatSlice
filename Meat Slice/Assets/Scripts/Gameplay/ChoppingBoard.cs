using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoppingBoard : MonoBehaviour
{
    [SerializeField] private FoodController foodController; //Reference to FoodController script in scene.
    public GameObject choppingBoardObject; //The physical chopping board Child under this Game Object.
    public float duration = 1f; //Duration of lerp. This can be used for different Speed modes if the player wants.
    Vector3 startingPosition; //This is the starting position of the choppingboard when game is first started.

    private void Start()
    {
        SetupVariables();
    }

    //Sets up script variables at start.
    void SetupVariables()
    {
        startingPosition = transform.position;

        if (foodController == null)
        {
            foodController = FindObjectOfType<FoodController>();
        }
    }

    //Moves To the yMovement position along the y axis then continues to next generation of the game loop.
    public IEnumerator MoveToNextYPosition(bool newFood, float yMovement)
    {
        if (GameManager.gameOver == false)
        {
            float percentage = 0f;
            float timer = 0f;
            float newYPos = yMovement;

            if (newFood == false)
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

            if (newFood) foodController.InstantiateNewFood();

            yield return null;
        }
        yield return null;
    }

    //This is played when game is reset.
    public void ReturnToOrigin()
    {
        transform.position = startingPosition;
    }

}
