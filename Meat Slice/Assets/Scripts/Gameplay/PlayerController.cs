using System;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Movement movement; //Reference to Movement script in scene.
    public StartingPlatform startingPlatform; //Reference to StartingPlatform script in scene.

    public GameObject[] foodToInstantiate; //Food chosen to use during game.

    public GameObject nextFood; //Stores next food to instantiante in the game loop.
    public GameObject currentFood; //Stores the current food to access in the game loop.
    private Transform previousFood; //Stores previous food to instantiante in the game loop.
    public Transform scalePivot; //Scales the block to fit the next stage of the game loop.

    public const int difficultyVarient = 4; //The dividing factor that controls the Rounding Factor in RoundToPoint.


    public bool isFoodSliced = true; //This stops spamming the slice button while loading next food or in menus.
    private float foodYScale = 1f; //Scale of the current foods Y scale.

    private const float pushForce = 3.5f; //Force of an object assigned rigid body and the applied force/speed of that push. 
    private const float pushForceVariance = 0.4f; //This will take standardPushForce and plus of minus to give the lowest possible push and the highest.
    private const float standardPushForce = 1f; //This will be the median times force of pushForce.


    private void Awake()
    {
        SetupVariables();
    }

    //Sets up script variables.
    void SetupVariables()
    {
        if (movement == null)
        {
            movement = FindObjectOfType<Movement>();
        }

        if (startingPlatform == null)
        {
            startingPlatform = FindObjectOfType<StartingPlatform>();
        }
    }

    //Resets the script to origin.
    public void ResetPlayerController()
    {
        nextFood = null;
        currentFood = null;
        NewPreviousFood(startingPlatform.startChildObject);
        isFoodSliced = true;
        scalePivot.localScale = Vector3.one;

        startingPlatform.ReturnToOrigin();
    }

    //Set previousFood to obj.
    public void NewPreviousFood(GameObject obj)
    {
        previousFood = obj.transform;
    }

    //Sets the next food and assigns the parameters for instantiating it when ready by picking a random food to spawn inside the foodToInstantiate array.
    void LoadNextFood()
    {
        nextFood = foodToInstantiate[UnityEngine.Random.Range(0, foodToInstantiate.Length)];
        int yScale = (int)nextFood.transform.localScale.y;
        foodYScale = yScale;
    }

    //instantiates the gameObject in nextFood then moves to the correct position.
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
            newFood.transform.localScale = VectorLibrary.ReplaceVector(previousFood.transform.localScale, foodYScale, 1);
            currentFood = newFood;
            movement.StartMovement(previousFood.transform.position);
        }
        else
        {
            newFood.transform.localScale = VectorLibrary.ReplaceVector(newFood.transform.localScale, foodYScale, 1);
            currentFood = newFood;
            startingPlatform.MoveToNext(false, foodYScale);
            movement.StartMovement(movement.centerPointCenter);
        }
        isFoodSliced = false;
    }


    //This action is played from a button on the ui.
    public void SliceFood()
    {
        //Check if button reset.
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
        currentFood.transform.SetParent(startingPlatform.transform);
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
            startingPlatform.MoveToNext(true, difference);
        }
    }


    //This will change the blocks size if it misses the mark else nothing.
    //Check if the block is off kilter from the previous box or not, if so remove.
    private void SetNewScale()
    {
        //Get the rounded positions of current and previous food.
        Vector3 currentFoodPos = currentFood.transform.position;
        // currentFoodPos = new Vector3(RoundToPoint(currentFoodPos.x, difficultyVarient), 0, RoundToPoint(currentFoodPos.z, difficultyVarient));
        // currentFood.transform.position = currentFoodPos;
        Vector3 prevFoodPos = previousFood.transform.position;
        prevFoodPos = new Vector3(RoundToPoint(previousFood.position.x, difficultyVarient), 0, RoundToPoint(previousFood.position.z, difficultyVarient));

        float distance = Vector3.Distance(currentFoodPos, prevFoodPos);
        if (distance == 0) return;

        bool isSpawnPointA = movement.startMovementPoint == movement.spawnPointA;
        Vector3 boxPosition = GetBoxPosition(isSpawnPointA, currentFoodPos, prevFoodPos, distance, out float xDiff, out float zDiff);
        scalePivot.position = boxPosition;

        //Set temp pivot as currentFood parent.
        currentFood.transform.SetParent(scalePivot);
        Vector3 newScale = CalculateNewScale(scalePivot.localScale, xDiff, zDiff);

        if (newScale == scalePivot.localScale) return;

        if (newScale.x <= 0 || newScale.z <= 0)
        {
            GameOver();
            return;
        }

        FoodType foodType = FoodType.None;
        if (currentFood.GetComponent<FoodCategory>())
        {
            foodType = currentFood.GetComponent<FoodCategory>().foodType;
        }

        Score.iScore.AddScore(foodType);

        //Set new scale.
        scalePivot.localScale = newScale;
        //Unparent currentFood from the temp pivot.
        currentFood.transform.SetParent(startingPlatform.transform);

        // SliceSound();

        CreateSlice(newScale, xDiff, zDiff, isSpawnPointA, prevFoodPos, currentFoodPos);
    }

    //Sound of slicing the food.
    void SliceSound()
    {
        if (AudioManager.iAudioManager != null)
        {
            AudioClip squelch = AudioManager.iAudioManager.squelch[UnityEngine.Random.Range(0, AudioManager.iAudioManager.squelch.Length)];
            AudioManager.iAudioManager.PlayOneShot(squelch);
        }
    }

    #region CreateSlice
    //Creates a slice of the current food and updates the visuals for the current food and slice.
    private void CreateSlice(Vector3 newScale, float xDiff, float zDiff, bool isSpawnPointA, Vector3 prevFoodPos, Vector3 currentFoodPos)
    {
        //Get the opposite position of the temppivot, this is where the slice needs to appear and material change.
        Vector3 oppositePivotPosition = GetBoxPosition(isSpawnPointA, prevFoodPos, currentFoodPos, 0, out _, out _);
        Vector3 newSliceScale = VectorLibrary.ReplaceVector(previousFood.transform.localScale, currentFood.transform.localScale.y, 1);

        Vector3 slicePosition = oppositePivotPosition;
        float increaseX = 1;
        float increaseZ = 1;

        //increaseX and increaseZ are either half of the currentFoods scale or 1.
        if (isSpawnPointA)
        {
            newSliceScale = VectorLibrary.ReplaceVector(newSliceScale, zDiff * GameManager.scaleFactor, 2);
            increaseZ = previousFood.transform.localScale.z * newScale.z / 2;

            //Invert increaseZ for minus z position.
            if (slicePosition.z < 0) increaseZ = -increaseZ;
        }
        else
        {
            newSliceScale = VectorLibrary.ReplaceVector(newSliceScale, xDiff * GameManager.scaleFactor, 0);
            increaseX = previousFood.transform.localScale.x * newScale.x / 2;

            //Invert increaseX for minus x position.
            if (slicePosition.x < 0) increaseX = -increaseX;
        }
        slicePosition = new Vector3(slicePosition.x + increaseX, slicePosition.y, slicePosition.z + increaseZ);

        //Now the position and localScale are setup, instantiate new food and assign position and scale.
        GameObject newSlice = Instantiate(currentFood);
        newSlice.transform.localScale = newSliceScale;
        newSlice.transform.position = slicePosition;

        //Assign the visual based on the location of the pivot.
        //Newslice must be done before currentFood as it is a copy of currentFood and would take on its visuals.
        if (newSlice.GetComponentInChildren<FoodVisual>())
        {
            newSlice.GetComponentInChildren<FoodVisual>().SliceVisual(scalePivot.position, xDiff, zDiff);
        }

        if (currentFood.GetComponentInChildren<FoodVisual>())
        {
            currentFood.GetComponentInChildren<FoodVisual>().SliceVisual(oppositePivotPosition, xDiff, zDiff);
        }

        //Give Physics so it falls off screen and jiggles on bounce.
        AssignPhysics(newSlice);
    }
    #endregion

    #region Physics
    //Gives a gameObject gravity and wobble when it bounces off an object.
    void AssignPhysics(GameObject newSlice)
    {
        if (newSlice.transform.GetChild(0))
        {
            GameObject parent = newSlice;
            newSlice = newSlice.transform.GetChild(0).gameObject;
            newSlice.transform.SetParent(null);
            Destroy(parent);

            if (newSlice.GetComponent<BoxCollider>() == false)
            {
                if (newSlice.GetComponent<Collider>())
                {
                    Destroy(newSlice.GetComponent<Collider>());
                }
                newSlice.AddComponent<BoxCollider>();
            }
        }

        Rigidbody rb = newSlice.AddComponent<Rigidbody>();
        rb.useGravity = true;

        Vector3 targetDirection = newSlice.transform.position - scalePivot.position;

        //Rotate Towards direction.
        Quaternion lookDirection = Quaternion.LookRotation(targetDirection);
        transform.rotation = lookDirection;

        //Push in direction.
        float _pushForce = pushForce * UnityEngine.Random.Range(standardPushForce - pushForceVariance, standardPushForce + pushForceVariance);
        rb.velocity = targetDirection * _pushForce;

        //Add Rotation based on direction.
        rb.AddTorque((-targetDirection * _pushForce) * _pushForce);

        // newSlice.AddComponent<JellyMesh>();
    }
    #endregion

    // if you want to round to 0.33 varient == 3.
    float RoundToPoint(float value, int varient)
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

    //Gets the center position and distanced traveled on x and z axis of an object.
    //Gets the position of a pivot point based on the spawnPoint direction (isSpawnPointA). 
    //As well as the current and previous positions to create a direction using distance between the two to get the difference as well.
    private Vector3 GetBoxPosition(bool isSpawnPointA, Vector3 currentFoodPos, Vector3 prevFoodPos, float distance, out float xDiff, out float zDiff)
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
            bool isPositive = currentFoodPos.z < prevFoodPos.z;
            boxPosition = isPositive ?
             currentFood.transform.TransformPoint(boxCenter + new Vector3(0, 0, halfSize.z)) :
             currentFood.transform.TransformPoint(boxCenter - new Vector3(0, 0, halfSize.z));
            zDiff = distance / GameManager.scaleFactor;
            xDiff = 0;
        }
        else
        {
            bool isPositive = currentFoodPos.x < prevFoodPos.x;
            boxPosition = isPositive ?
            currentFood.transform.TransformPoint(boxCenter + new Vector3(halfSize.x, 0, 0)) :
            currentFood.transform.TransformPoint(boxCenter - new Vector3(halfSize.x, 0, 0));
            xDiff = distance / GameManager.scaleFactor;
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
        GameManager.iGameManager.GameOver();
    }
}

