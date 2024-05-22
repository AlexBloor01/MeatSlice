using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelSetup : MonoBehaviour
{
    [SerializeField] private StartingPlatform startingPlatform; //Reference to Scene StartingPlatform Script.
    [SerializeField] private Movement movement; //Reference to Scene Movement Script.
    [SerializeField] private PlayerController playerController; //Reference to Scene PlayerController Script.
    [SerializeField] private GameObject[] meatObjects; //Meat Objects to use as stacking block.
    [SerializeField] private GameObject[] cheeseObjects; //Cheese Objects to use as stacking block.
    [SerializeField] private GameObject[] vegetableObjects; //Vegetable Objects to use as stacking block.
    [SerializeField] private GameObject[] veganObjects; //Vegan Protien Objects to use as stacking block. (Tofu and beans)
    [SerializeField] private GameObject[] breadObjects; //Bread Objects to use as stacking block.
    private GameObject[][] foodSetup; //This is an ordered array that will contain meatObjects,cheeseObjects, etc in order so that they can be removed or added dependant on user choices.

    private void Awake()
    {
        SetupVariables();
    }

    void SetupVariables()
    {
        //The foodSetup size is set based on number of FoodType options.
        foodSetup = new GameObject[FoodType.GetNames(typeof(FoodType)).Length][];

        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }

        if (startingPlatform == null)
        {
            startingPlatform = FindObjectOfType<StartingPlatform>();
        }

        if (movement == null)
        {
            movement = FindObjectOfType<Movement>();
        }
    }

    //This sets up the level for a specific food type depending on player preferences.
    //Assign this to the toggles on the main menu.
    public void ToggleFoodType(FoodType foodType, bool isOn)
    {
        if (isOn)
        {
            switch (foodType)
            {
                case FoodType.Meat:
                    foodSetup[(int)foodType] = meatObjects;
                    break;

                case FoodType.Cheese:
                    foodSetup[(int)foodType] = cheeseObjects;
                    break;

                case FoodType.Vegetables:
                    foodSetup[(int)foodType] = vegetableObjects;
                    break;

                case FoodType.VeganProtein:
                    foodSetup[(int)foodType] = veganObjects;
                    break;

                case FoodType.Bread:
                    foodSetup[(int)foodType] = breadObjects;
                    break;
            }

        }
        else
        {
            foodSetup[(int)foodType] = null;
        }

        // Debug.Log($"MeatType: {foodType} isOn: {isOn}");
    }

    //Play this at the beginning of the game from the main menu start button, it will convert the player choices to foodToInstantiate at the beginning of the level.
    private void FoodToInstantiateImport()
    {
        playerController.foodToInstantiate = null;
        List<GameObject> foodToAdd = new List<GameObject>();

        for (int i = 0; i < foodSetup.Length; i++)
        {
            if (foodSetup[i] != null)
            {
                foreach (GameObject obj in foodSetup[i])
                {
                    foodToAdd.Add(obj);
                }
            }
        }

        if (foodToAdd.Count > 0)
        {
            playerController.foodToInstantiate = foodToAdd.ToArray();
        }
        else
        {
            playerController.foodToInstantiate = meatObjects;
        }
    }

    public void SetupLevel()
    {
        Menus.UnHideMenu(Score.iScore.gameObject);
        ResetPlayerController();
        ResetMovement();
        Score.iScore.ResetScore();
        ResetStartPlatform();
        FoodToInstantiateImport();
    }

    public void StartLevel()
    {
        playerController.InstantiateNewFood();
    }

    //Resets the Player controller.
    //Activates player controllers reset and removes any child objects not required.
    public void ResetPlayerController()
    {
        playerController.ResetPlayerController();
        RemoveChildGameObjects(playerController.transform, null);
    }

    //Resets the Movement script for the player by stopping any current movement and returning to center point.
    void ResetMovement()
    {
        movement.StopMovement();
        movement.SetCenterPoint(movement.centerPointCenter);
    }

    //Resets the Starting Platform by removes any child objects not required.
    void ResetStartPlatform()
    {
        startingPlatform.ReturnToOrigin();
        GameObject[] objectsToAvoid = { startingPlatform.startChildObject, playerController.scalePivot.gameObject };
        RemoveChildGameObjects(playerController.startingPlatform.transform, objectsToAvoid);
    }

    void RemoveChildGameObjects(Transform parent, GameObject[] avoid)
    {
        List<GameObject> children = new List<GameObject>();

        foreach (Transform child in parent)
        {
            if (avoid == null || avoid.All(obj => child.gameObject != obj))
            {
                children.Add(child.gameObject);
            }
        }

        for (int i = children.Count - 1; i >= 0; i--)
        {
            Destroy(children[i]);
        }
    }
}

#region Food Types
//Different Foods to spawn.
public enum FoodType
{
    Meat,
    Cheese,
    Vegetables,
    VeganProtein,
    Bread,
    None //Put this last.
}
#endregion