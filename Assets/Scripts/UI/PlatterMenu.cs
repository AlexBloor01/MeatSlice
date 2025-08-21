using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlatterMenu : MonoBehaviour
{
    public Movement movement;
    public Slider speedSlider;
    public TextMeshProUGUI speedTMPro;
    string speedText = "Game Speed: ";
    int decimalPlaceDivider = 10;


    // Start is called before the first frame update
    void Start()
    {
        SetupVariables();
    }

    void SetupVariables()
    {
        if (movement == null)
        {
            movement = FindObjectOfType<Movement>();
        }

        //Setup game speed slider.
        SetupSpeedSlider();

        //HIde this menu on start.
        Menus.HideMenu(gameObject);
    }

    //Setup the game speed slider.
    void SetupSpeedSlider()
    {
        float currentSpeed = PlayerPrefs.GetFloat("Game_Speed", 1);
        speedSlider.minValue = Movement.minSpeed;
        speedSlider.maxValue = Movement.maxSpeed;
        currentSpeed = Mathf.Round(currentSpeed * 10f) / 10f; //Round to nearest 0.1.
        speedSlider.value = currentSpeed;
        speedTMPro.text = speedText + currentSpeed.ToString("F2");
    }

    //Link this to game speed slider and movement to make the game faster or slower.
    public void SpeedControl(float _speed)
    {
        //This will force the speedcontrol to round to the nearest 0.1
        float newSpeed = Mathf.Floor(_speed) + ((Mathf.Round(-((Mathf.Floor(_speed) - _speed)) * decimalPlaceDivider)) / decimalPlaceDivider);

        movement.speed = newSpeed;
        speedTMPro.text = speedText + movement.speed.ToString("F2");
        PlayerPrefs.SetFloat("Game_Speed", movement.speed);
    }
}
