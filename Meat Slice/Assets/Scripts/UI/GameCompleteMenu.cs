using TMPro;
using UnityEngine;

public class GameCompleteMenu : MonoBehaviour
{
    //TextMesh text references under Game Complete Menu.
    public TextMeshProUGUI meatTMPro;
    public TextMeshProUGUI cheeseTMPro;
    public TextMeshProUGUI vegetableTMPro;
    public TextMeshProUGUI tofuTMPro;
    public TextMeshProUGUI breadTMPro;

    //Text for each each TextMesh text reference.
    const string meatText = "Meats ";
    const string cheeseText = "Cheeses ";
    const string vegetableText = "Vegetables ";
    const string tofuText = "Tofu ";
    const string breadText = "Bread ";

    public SettingsMenu settingsMenu; //Reference to the scenes UI settings button for hiding it. 


    public GameObject burgerBunPrefab; //Reference to Burgerbun prefab to yeet at the end of the level.
    public Quaternion burgerBunSpawnRotation; //Burger bun spawn rotation, make sure this is upside down.
    public Transform burgerSpawnPosition; //Position to spawn the burger bun on top from.
    private int burgerBunLimit = 20; //Maximim number of burger buns available to fire.
    private const float maxBunSpeed = -80; //Maximum bun firing speed. Max should be a minus number due to using rb.velocity.
    private const int minBunSpeed = 20; //Minimum bun firing speed.
    private int bunsSpawned = 0; //Current number of burger buns spawned.


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
        settingsMenu.HideSettingsButtonAnim();
        transform.localScale = Vector3.one;
    }

    public void CloseGameCompleteMenu()
    {
        settingsMenu.UnhideSettingsButtonAnim();
        transform.localScale = Vector3.zero;
    }

    void UpdateCounters()
    {
        burgerBunLimit = Score.iScore.score + 1;

        meatTMPro.text = meatText + Score.iScore.meatCounter.ToString();
        cheeseTMPro.text = cheeseText + Score.iScore.cheeseCounter.ToString();
        vegetableTMPro.text = vegetableText + Score.iScore.vegetableCounter.ToString();
        tofuTMPro.text = tofuText + Score.iScore.tofuCounter.ToString();
        breadTMPro.text = breadText + Score.iScore.breadCounter.ToString();
    }

    public void InstantiateBurger()
    {
        if (bunsSpawned > burgerBunLimit) return;

        GameObject bun = Instantiate(burgerBunPrefab);
        bun.transform.position = burgerSpawnPosition.position;
        bun.transform.rotation = burgerBunSpawnRotation;
        Rigidbody rb = bun.AddComponent<Rigidbody>();
        rb.velocity = new Vector3(0, Random.Range(minBunSpeed, maxBunSpeed), 0);
        //JellyMesh jm = bun.AddComponent<JellyMesh>();

        bunsSpawned++;
    }

    public void TraverseTower()
    {

    }

}
