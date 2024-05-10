using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FoodController : MonoBehaviour
{
    [SerializeField] private Movement movement;
    public ChoppingBoard choppingBoard;
    // [SerializeField] private JellyMesh jellyMesh;

    public GameObject[] foodToInstantiate; //Food chosen to use during game.

    private GameObject nextFood; //Stores next food to instantiante in the game loop.
    private GameObject currentFood; //Stores the current food to access in the game loop.
    private Transform previousFood; //Stores previous food to instantiante in the game loop.
    public Transform tempPivot; //Scales the block to fit the next stage of the game loop.

    private const int scaleFactor = 10; //Scale of each base object on the x and y axis.
    public const int difficultyVarient = 4; //UPDATE Positioning so it can do 2 with accuracy.
    private const int minimumScale = 2; //Minumum scaling of the foodYScale, if an object is below or equal to this number it will be too small to alter.


    public bool isFoodSliced = true; //This stops spamming the slice button while loading next food or in menus.
    private float foodYScale = 1f; //Scale of the current foods Y scale.

    private const float pushForce = 3.5f; //Force of an object assigned rigid body and the applied force/speed of that push. 
    const float slicePositionIncrease = 5f; //This is the position away from the original slice the object will instantiate from. This must be above 1.
    private const float pushForceVariance = 0.4f; //This will take standardPushForce and plus of minus to give the lowest possible push and the highest.
    private const float standardPushForce = 1f; //This will be the median times force of pushForce.


    private void Awake()
    {
        SetupVariables();
    }

    void SetupVariables()
    {
        if (movement == null)
        {
            movement = FindObjectOfType<Movement>();
        }

        if (choppingBoard == null)
        {
            choppingBoard = FindObjectOfType<ChoppingBoard>();
        }
    }

    public void Reset()
    {
        nextFood = null;
        currentFood = null;
        NewPreviousFood(choppingBoard.choppingBoardObject);
        isFoodSliced = true;
        tempPivot.localScale = Vector3.one;
        choppingBoard.ReturnToOrigin();
        movement.SetCenterPoint(movement.centerPointCenter);
    }

    public void NewPreviousFood(GameObject obj)
    {
        previousFood = obj.transform;
    }

    void LoadNextFood()
    {
        nextFood = foodToInstantiate[UnityEngine.Random.Range(0, foodToInstantiate.Length)];

        int yScale = (int)nextFood.transform.localScale.y;

        if (yScale > minimumScale)
        {
            int lowerRnage = yScale - (int)minimumScale / 2;
            int upperRange = yScale + (int)minimumScale / 2;
            yScale = UnityEngine.Random.Range(lowerRnage, upperRange);
        }
        foodYScale = yScale;
    }

    //Pick a random food to spawn inside the foodToInstantiate array.
    public void InstantiateNewFood()
    {
        if (nextFood == null)
        {
            LoadNextFood();
        }
        GameObject newFood = Instantiate(nextFood);
        newFood.transform.SetParent(transform);
        newFood.transform.localPosition = Vector3.zero;

        if (currentFood != null)
        {
            NewPreviousFood(currentFood);
            newFood.transform.localScale = ReplaceY(previousFood.transform.localScale, foodYScale);
            currentFood = newFood;
            movement.StartMovement(previousFood.transform.position);
        }
        else
        {
            newFood.transform.localScale = ReplaceY(newFood.transform.localScale, foodYScale);
            currentFood = newFood;
            StartCoroutine(choppingBoard.MoveToNextYPosition(false, foodYScale));
            movement.StartMovement(movement.centerPointCenter);
        }
        isFoodSliced = false;
    }

    Vector3 ReplaceY(Vector3 original, float y)
    {
        return new Vector3(original.x, y, original.z);
    }

    //This action is played from a button on the ui.
    public void SliceFood()
    {
        if (isFoodSliced)
        {
            return;
        }

        isFoodSliced = true;
        //Stop movement.
        movement.StopMovement();

        //Scale.
        SetNewScale();

        //Set parent to chopping board.
        currentFood.transform.SetParent(choppingBoard.transform);

        NextGenerationStep();
    }

    void NextGenerationStep()
    {
        //Continue to next round if the game is not over.
        if (GameManager.gameOver == false)
        {
            LoadNextFood();
            float difference = foodYScale;
            if (nextFood.transform.localScale.y != currentFood.transform.localScale.y)
            {
                difference = (currentFood.transform.localScale.y * 0.5f) + (foodYScale * 0.5f);
            }
            StartCoroutine(choppingBoard.MoveToNextYPosition(true, difference));
        }
    }


    //This will change the blocks size if it misses the mark else nothing.
    //Check if the block is off kilter from the previous box or not, if so remove.
    private void SetNewScale()
    {
        Vector3 currentFoodPos = currentFood.transform.position;
        currentFoodPos = new Vector3(RoundToPointFive(currentFoodPos.x, difficultyVarient), 0, RoundToPointFive(currentFoodPos.z, difficultyVarient));
        currentFood.transform.position = currentFoodPos;

        Vector3 prevFoodPos = new Vector3(RoundToPointFive(previousFood.position.x, difficultyVarient), 0, RoundToPointFive(previousFood.position.z, difficultyVarient));

        float distance = Vector3.Distance(currentFoodPos, prevFoodPos);
        if (distance == 0) return;

        bool isSpawnPointA = movement.startMovementPoint == movement.spawnPointA;

        Vector3 boxPosition = GetBoxPosition(isSpawnPointA, currentFoodPos, prevFoodPos, distance, out float xDiff, out float zDiff);

        tempPivot.position = boxPosition;

        //Set temp pivot as currentFood parent.
        currentFood.transform.SetParent(tempPivot);

        Vector3 newScale = CalculateNewScale(tempPivot.localScale, xDiff, zDiff);

        if (newScale == tempPivot.localScale) return;

        if (newScale.x <= 0 || newScale.z <= 0)
        {
            GameOver();
            return;
        }
        //Set new scale.
        tempPivot.localScale = newScale;

        //Unparent currentFood from the temp pivot.
        currentFood.transform.SetParent(choppingBoard.transform);

        CreateSlice(newScale, xDiff, zDiff, isSpawnPointA, currentFoodPos, prevFoodPos, distance);
    }

    private void CreateSlice(Vector3 newScale, float xDiff, float zDiff, bool isSpawnPointA, Vector3 currentFoodPos, Vector3 prevFoodPos, float distance)
    {
        Vector3 newSliceScale = ReplaceY(previousFood.transform.localScale, currentFood.transform.localScale.y);

        //Set the opposite box position on the Temp Pivot for slicing.
        Vector3 oppositeBoxPosition = GetBoxPosition(isSpawnPointA, prevFoodPos, currentFoodPos, distance, out _, out _);
        Vector3 slicePosition = oppositeBoxPosition;

        float increaseX = 1;
        float increaseZ = 1;

        //Scale Slice based on which of x or z difference is higher.
        if (!isSpawnPointA)
        {
            newSliceScale = new Vector3(xDiff * scaleFactor, newSliceScale.y, newSliceScale.z);
            increaseX = (currentFood.transform.localScale.x * newScale.x) / 2;
            if (slicePosition.x < 0) increaseX = -increaseX;
        }
        else
        {
            newSliceScale = new Vector3(newSliceScale.x, newSliceScale.y, zDiff * scaleFactor);
            increaseZ = (currentFood.transform.localScale.z * newScale.z) / 2;
            if (slicePosition.z < 0) increaseZ = -increaseZ;
        }

        GameObject newSlice = Instantiate(currentFood);
        newSlice.transform.localScale = newSliceScale;

        slicePosition = new Vector3(slicePosition.x + increaseX, slicePosition.y, slicePosition.z + increaseZ);

        newSlice.transform.position = slicePosition;

        AssignPhysics(newSlice);
    }


    void AssignPhysics(GameObject newSlice)
    {
        if (newSlice.transform.GetChild(0))
        {
            GameObject parent = newSlice;
            newSlice = newSlice.transform.GetChild(0).gameObject;
            newSlice.transform.SetParent(null);
            Destroy(parent);
            newSlice.AddComponent<BoxCollider>();
        }

        Rigidbody rb = newSlice.AddComponent<Rigidbody>();
        rb.useGravity = true;

        Vector3 targetDirection = newSlice.transform.position - tempPivot.position;

        //Rotate Towards direction.
        Quaternion lookDirection = Quaternion.LookRotation(targetDirection);
        transform.rotation = lookDirection;

        //Push in direction.
        float _pushForce = pushForce * UnityEngine.Random.Range(standardPushForce - pushForceVariance, standardPushForce + pushForceVariance);
        rb.velocity = targetDirection * _pushForce;

        //Add Rotation based on direction.
        rb.AddTorque((-targetDirection * _pushForce) * _pushForce);

        newSlice.AddComponent<JellyMesh>();
    }

    // if you want to round to 0.33 varient == 3.
    float RoundToPointFive(float value, int varient)
    {
        if (value == 0) return value;

        //Check if negative, if so make positive.
        bool isNegativeNumber = value < 0;
        if (isNegativeNumber)
        {
            value = -value;
        }

        //Get whole and decimal numbers separated.
        int wholeNumberPart = (int)value;

        //Round all numbers below first decimal place.
        float decimalNumberPart = (value - wholeNumberPart) * 10;
        decimalNumberPart = Mathf.Round(decimalNumberPart) / 10;

        //Round to variant.
        //This will split 1 into parts, if you want to round to 0.33 varient == 3.
        decimalNumberPart = (float)Math.Round(decimalNumberPart * varient, MidpointRounding.ToEven) / varient;

        //Replace value with new rounded number.
        value = wholeNumberPart + decimalNumberPart;

        //if negative return to negative.
        if (isNegativeNumber)
        {
            value = -value;
        }
        return value;
    }

    private Vector3 GetBoxPosition(bool isSpawnPointA, Vector3 currentPos, Vector3 prevPos, float distance, out float xDiff, out float zDiff)
    {
        BoxCollider boxCollider;
        if (currentFood.GetComponent<BoxCollider>())
        {
            boxCollider = currentFood.GetComponent<BoxCollider>();
        }
        else
        {
            boxCollider = currentFood.AddComponent<BoxCollider>();
        }

        Vector3 boxCenter = boxCollider.center;
        Vector3 halfSize = boxCollider.size / 2;
        Vector3 boxPosition = Vector3.one;

        if (isSpawnPointA)
        {
            bool isPositive = currentPos.z < prevPos.z;
            boxPosition = isPositive ?
             currentFood.transform.TransformPoint(boxCenter + new Vector3(0, 0, halfSize.z)) :
             currentFood.transform.TransformPoint(boxCenter - new Vector3(0, 0, halfSize.z));
            zDiff = distance / scaleFactor;
            xDiff = 0;
        }
        else
        {
            bool isPositive = currentPos.x < prevPos.x;
            boxPosition = isPositive ?
            currentFood.transform.TransformPoint(boxCenter + new Vector3(halfSize.x, 0, 0)) :
            currentFood.transform.TransformPoint(boxCenter - new Vector3(halfSize.x, 0, 0));
            xDiff = distance / scaleFactor;
            zDiff = 0;
        }

        return boxPosition;
    }

    private Vector3 CalculateNewScale(Vector3 currentScale, float xDiff, float zDiff)
    {
        Vector3 newScale = new Vector3(
            Mathf.Max(currentScale.x - xDiff, 0),
            currentScale.y,
            Mathf.Max(currentScale.z - zDiff, 0)
        );
        return newScale;
    }



    private void GameOver()
    {
        AssignPhysics(currentFood);
        GameManager.GameOver();
    }

}



//Different Foods to spawn.
public enum FoodType
{
    Meat,
    Cheese,
    Vegetables,
    VeganProtein
}