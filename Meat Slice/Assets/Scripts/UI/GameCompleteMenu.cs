using TMPro;
using UnityEngine;

public class GameCompleteMenu : MonoBehaviour
{


    //TextMesh text references under Game Complete Menu.
    public TextMeshProUGUI meatTMPro;
    public TextMeshProUGUI cheeseTMPro;
    public TextMeshProUGUI vegetableTMPro;
    public TextMeshProUGUI tofuTMPro;

    //Text for each each TextMesh text reference.
    const string meatText = "Meat Counter: ";
    const string cheeseText = "Cheese Counter: ";
    const string vegetableText = "Vegetable Counter: ";
    const string tofuText = "Tofu Counter: ";

    public SettingsMenu settingsMenu; //Reference to the scenes UI settings button for hiding it. 


    public GameObject burgerBunPrefab; //Reference to Burgerbun prefab to yeet at the end of the level.
    public Quaternion burgerBunSpawnRotation; //Burger bun spawn rotation, make sure this is upside down.
    public Transform burgerSpawnPosition; //Position to spawn the burger bun on top from.
    const int burgerBunLimit = 20; //Maximim number of burger buns available to fire.
    const float maxBunSpeed = -80; //Maximum bun firing speed. Max should be a minus number due to using rb.velocity.
    const int minBunSpeed = 20; //Minimum bun firing speed.
    int bunsSpawned = 0; //Current number of burger buns spawned.


    private void Start()
    {
        SetupVariables();
    }

    void SetupVariables()
    {
        CloseGameCompleteMenu();
    }

    public void OpenGameCompleteMenu()
    {
        UpdateCounters();
        settingsMenu.HideSettingsButton();
        transform.localScale = Vector3.one;
    }

    public void CloseGameCompleteMenu()
    {
        settingsMenu.UnhideSettingsButton();
        transform.localScale = Vector3.zero;
    }

    void UpdateCounters()
    {
        meatTMPro.text = meatText + Score.iScore.meatCounter.ToString();
        cheeseTMPro.text = cheeseText + Score.iScore.cheeseCounter.ToString();
        vegetableTMPro.text = vegetableText + Score.iScore.vegetableCounter.ToString();
        tofuTMPro.text = tofuText + Score.iScore.tofuCounter.ToString();
    }

    public void InstantiateBurger()
    {
        if (bunsSpawned > burgerBunLimit) return;

        GameObject bun = Instantiate(burgerBunPrefab);
        bun.transform.position = burgerSpawnPosition.position;
        bun.transform.rotation = burgerBunSpawnRotation;
        Rigidbody rb = bun.AddComponent<Rigidbody>();
        rb.velocity = new Vector3(0, Random.Range(minBunSpeed, maxBunSpeed), 0);

        bunsSpawned++;
    }

}
