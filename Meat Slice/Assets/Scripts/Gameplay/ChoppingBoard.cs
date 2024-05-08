using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoppingBoard : MonoBehaviour
{
    [SerializeField] private FoodController foodController;
    public GameObject choppingBoardObject; //The physical chopping board Child under this Game Object.
    public float duration = 1f; //Duration of lerp. This can be used for different Speed modes if the player wants.

    Vector3 startingPosition;

    private void Start()
    {
        SetupVariables();
    }

    void SetupVariables()
    {
        startingPosition = transform.position;
    }

    public IEnumerator MoveToNextYPosition(bool newFood)
    {
        if (GameManager.gameOver == false)
        {
            float percentage = 0f;
            float timer = 0f;
            float newYPos = newFood ? foodController.foodYSize : (foodController.foodYSize / 2);
            if (newFood)
            {
                newYPos = foodController.foodYSize;

            }
            else
            {
                if (foodController.foodYSize > 1)
                {
                    newYPos = foodController.foodYSize / 2;
                    // newYPos -= 0.5f;
                }

                // if ( && foodController.foodYSize > 1)
                // {
                //     newYPos -= 0.5f;
                // }

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

    public void ReturnToOrigin()
    {
        transform.position = startingPosition;
    }

}
