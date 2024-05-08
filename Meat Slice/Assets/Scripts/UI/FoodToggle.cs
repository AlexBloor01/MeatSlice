
using UnityEngine;
using UnityEngine.UI;

public class FoodToggle : MonoBehaviour
{
    [SerializeField] private LevelSetup levelSetup;
    public FoodType foodType;
    public Toggle toggle;


    private void Start()
    {
        SetupToggle();
    }

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

        // Add a click listener to the button.
        toggle.onValueChanged.AddListener(OnToggleClick);

        //Set Toggle on startup.
        OnToggleClick(toggle.isOn);
    }

    private void OnToggleClick(bool isOn)
    {
        levelSetup.ToggleMeatType(foodType, isOn);
    }
}
