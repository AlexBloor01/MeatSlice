
using UnityEngine;
using UnityEngine.UI;

public class SlowMoToggle : MonoBehaviour
{
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

        // Add a click listener to the button.
        toggle.onValueChanged.AddListener(OnToggleClick);

        bool toggleSetting = toggle.isOn;

        //Grab saved choice if played before.
        if (GameManager.playedBefore)
        {
            toggleSetting = PlayerPrefs.GetInt("SlowMotionToggle", 0) == 0;
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
        GameManager.slowMode = isOn;
        Debug.Log($"Slow Motion Mode isOn: {isOn}");
        PlayerPrefs.SetInt("SlowMotionToggle", isOn ? 0 : 1);
    }
}
