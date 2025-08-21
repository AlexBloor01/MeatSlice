
using System.Runtime.Serialization.Formatters;
using UnityEngine;
using UnityEngine.UI;

public class FoodToggle : MonoBehaviour
{
    [SerializeField] private LevelSetup levelSetup; //Reference to LevelSetup in scene.
    public FoodType foodType; //What type of food are we changing in the level setup script?
    public Toggle toggle; //Reference to the toggle attatched to this gameObject.


    private void Start()
    {
        SetupToggle();
    }

    //Setup the toggle when the game starts.
    void SetupToggle()
    {
        if (toggle == null)
        {
            toggle = GetComponent<Toggle>();
        }

        if (levelSetup == null)
        {
            levelSetup = FindObjectOfType<LevelSetup>();
        }

        bool toggleSetting = toggle.isOn;

        //Grab saved choice if played before.
        if (GameManager.playedBefore)
        {
            toggleSetting = PlayerPrefs.GetInt($"{foodType}_FoodToggleSetting", 0) == 0;
        }

        // Add a click listener to the button and set the toggle isOn visual to the correct setting.
        toggle.onValueChanged.AddListener(OnToggleClick);
        toggle.isOn = toggleSetting;

        //Set Toggle on startup.
        OnToggleClick(toggleSetting);
    }

    //On Click Toggle action.
    private void OnToggleClick(bool isOn)
    {
        levelSetup.ToggleFoodType(foodType, isOn);
        PlayerPrefs.SetInt($"{foodType}_FoodToggleSetting", isOn ? 0 : 1);
        Debug.Log($"MeatType: {foodType} isOn: {isOn}");
    }
}
