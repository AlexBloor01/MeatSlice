using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelSetup : MonoBehaviour
{
    [SerializeField] private FoodController foodController;
    [SerializeField] private GameObject[] meatObjects; //Meat Objects to use as stacking block.
    [SerializeField] private GameObject[] cheeseObjects; //Cheese Objects to use as stacking block.
    [SerializeField] private GameObject[] vegetableObjects; //Vegetable Objects to use as stacking block.
    [SerializeField] private GameObject[] veganProteinObjects; //Vegan Protien Objects to use as stacking block. (Tofu and beans)
    private GameObject[][] foodSetup; //This is an ordered array that will contain meatObjects,cheeseObjects, etc in order so that they can be removed or added dependant on user choices.

    private void Awake()
    {
        SetupVariables();
    }

    void SetupVariables()
    {
        //The foodSetup size is set based on number of FoodType options.
        foodSetup = new GameObject[FoodType.GetNames(typeof(FoodType)).Length][];

        if (foodController == null)
        {
            foodController = FindObjectOfType<FoodController>();
        }
    }

    //This sets up the level for a specific food type depending on player preferences.
    //Assign this to the toggles on the main menu.
    public void ToggleMeatType(FoodType foodType, bool isOn)
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
                    foodSetup[(int)foodType] = veganProteinObjects;
                    break;
            }

        }
        else
        {
            foodSetup[(int)foodType] = null;
        }

        Debug.Log($"MeatType: {foodType} isOn: {isOn}");
    }

    //Play this at the beginning of the game from the main menu start button, it will convert the player choices to foodToInstantiate at the beginning of the level.
    private void FoodToInstantiateSetup()
    {
        foodController.foodToInstantiate = null;
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
            foodController.foodToInstantiate = foodToAdd.ToArray();
        }
        else
        {
            foodController.foodToInstantiate = meatObjects;
        }
    }

    public void SetupLevel()
    {
        ResetHolder();
        ResetChoppingboard();
        FoodToInstantiateSetup();
    }

    public void StartLevel()
    {
        foodController.InstantiateNewFood();
    }

    public void ResetHolder()
    {
        foodController.Reset();
        RemoveChildGameObjects(foodController.transform, null);
    }

    public void ResetChoppingboard()
    {
        GameObject[] objectsToAvoid = { foodController.choppingBoard.choppingBoardObject, foodController.tempPivot.gameObject };
        RemoveChildGameObjects(foodController.choppingBoard.transform, objectsToAvoid);
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