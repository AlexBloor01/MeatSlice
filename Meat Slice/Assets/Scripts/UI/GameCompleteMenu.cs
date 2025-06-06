using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameCompleteMenu : MonoBehaviour
{
    [Header("Game Scoreboard")]
    //This menu appears at the end of a game when the player has no positions left to stack on and presents the score of the game.

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

    [Header("References")]
    public SettingsMenu settingsMenu; //Reference to the scenes UI settings button for hiding it. 


    [Header("Burger Bun Spawning")]
    public GameObject burgerBunPrefab; //Reference to Burgerbun prefab to yeet at the end of the level.
    public Quaternion burgerBunSpawnRotation; //Burger bun spawn rotation, make sure this is upside down.
    public Transform burgerSpawnPosition; //Position to spawn the burger bun on top from.
    private Vector3 burgerSpawnStartingPosition;
    public Transform choppingBoard; //Chopping board transform.
    private int burgerBunLimit = 200; //Maximim number of burger buns available to fire.
    private const float maxBunSpeed = -80; //Maximum bun firing speed. Max should be a minus number due to using rb.velocity.
    private const int minBunSpeed = 20; //Minimum bun firing speed.
    int minBunSpawnsAvailable = 15;
    private List<GameObject> bunsSpawned = new List<GameObject>(); //Current burger buns spawned.
    float killBunTimer = 1.75f;

    private void Start()
    {
        SetupVariables();
    }

    void SetupVariables()
    {
        burgerSpawnStartingPosition = burgerSpawnPosition.position;
        CloseGameCompleteMenu();
    }

    public void OpenGameCompleteMenu()
    {
        SetupBurgerSpawner();
        UpdateCounters();
        bunsSpawned = new List<GameObject>();
        settingsMenu.HideSettingsButtonAnim();
        transform.localScale = Vector3.one;
    }

    void SetupBurgerSpawner()
    {
        burgerSpawnPosition.position = burgerSpawnStartingPosition;
        burgerSpawnPosition.parent = choppingBoard;
    }


    public void CloseGameCompleteMenu()
    {
        RemoveBuergerBunSpawner();
        settingsMenu.UnhideSettingsButtonAnim();
        transform.localScale = Vector3.zero;
        StartCoroutine(KillTheBuns());
    }

    void RemoveBuergerBunSpawner()
    {
        burgerSpawnPosition.parent = null;
    }

    IEnumerator KillTheBuns()
    {
        yield return new WaitForSeconds(killBunTimer);
        foreach (GameObject bun in bunsSpawned)
        {
            if (bun != null)
            {
                Destroy(bun);
            }
        }
        bunsSpawned.Clear();
        yield return null;
    }

    void UpdateCounters()
    {
        burgerBunLimit = Score.iScore.score + minBunSpawnsAvailable;

        meatTMPro.text = meatText + Score.iScore.meatCounter.ToString();
        cheeseTMPro.text = cheeseText + Score.iScore.cheeseCounter.ToString();
        vegetableTMPro.text = vegetableText + Score.iScore.vegetableCounter.ToString();
        tofuTMPro.text = tofuText + Score.iScore.tofuCounter.ToString();
        breadTMPro.text = breadText + Score.iScore.breadCounter.ToString();
    }

    public void InstantiateBurger()
    {
        if (bunsSpawned.Count > burgerBunLimit) return;

        GameObject bun = Instantiate(burgerBunPrefab);
        bun.transform.position = burgerSpawnPosition.position;
        bun.transform.rotation = burgerBunSpawnRotation;
        Rigidbody rb = bun.AddComponent<Rigidbody>();
        rb.velocity = new Vector3(0, Random.Range(minBunSpeed, maxBunSpeed), 0);
        //JellyMesh jm = bun.AddComponent<JellyMesh>();

        bunsSpawned.Add(bun);
    }

    public void TraverseTower()
    {

    }

}
