
using UnityEngine;
using UnityEngine.UI;

public class SlowMoToggle : MonoBehaviour
{
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

        // Add a click listener to the button.
        toggle.onValueChanged.AddListener(OnToggleClick);

        //Set Toggle on startup.
        OnToggleClick(toggle.isOn);
    }

    private void OnToggleClick(bool isOn)
    {
        GameManager.slowMode = isOn;
        Debug.Log($"Slow mode is {GameManager.slowMode}");
    }
}
