using System.Collections;
using UnityEngine;
using System;

public class CloudGenerator : MonoBehaviour
{
    public GameObject[] cloud; //Reference to cloud Prefabs wanted in no particular order.
    public Transform target; //target position for clouds to lerp to.
    public int maxClouds = 12; //Max number of clouds allowed alive at one time.
    public Vector2 durationTime = new Vector2(15f, 20f); //Random duration time between start and end lerp position.
    public Vector2 waitTime = new Vector2(4f, 7f); //Random wait time before spawning another cloud.
    public Vector2 cloudScale = new Vector2(2f, 4f); //Random scale value to equal on all cloud scale axes.
    public Quaternion lookRotation; //Every cloud will be on this rotation, use this to face the camera.
    public bool cloudFlip = false; //Do you want to randomly flip clouds scale on the x axis? Better for far away clouds.
    public bool introSpawn = false; //Do you want clouds to spawn right as the game starts?
    public int introSpawnAmount = 3; //How many at the very beginning of the game do you want to spawn?


    public float lowestPoint = -40; //Lowest position on y axis to spawn clouds.
    public float highestPoint = 20; //Highest position on y axis to spawn clouds.
    private const float spawnPointVariation = 3; //Variation in height between cloud spawns.
    private int clouds = 0; //Current clouds in play.


    void Start()
    {
        SetupVariables();
    }

    void SetupVariables()
    {
        if (target == null) target = transform.GetChild(0).transform;
        IntroSpawning();
        StartCoroutine(GenerateClouds());
    }

    void IntroSpawning()
    {
        if (introSpawn)
        {
            for (int i = 0; i < introSpawnAmount; i++)
            {
                InstantiateCloud(UnityEngine.Random.Range(durationTime.x, durationTime.y) / i + 1);
            }
        }
    }

    IEnumerator GenerateClouds()
    {
        while (true)
        {
            float confirmedWaitTime = UnityEngine.Random.Range(waitTime.x, waitTime.y);
            yield return new WaitForSeconds(confirmedWaitTime);
            InstantiateCloud(UnityEngine.Random.Range(durationTime.x, durationTime.y));
            yield return new WaitUntil(() => clouds < maxClouds);
        }
    }

    void InstantiateCloud(float lerpDuration)
    {
        float y = UnityEngine.Random.Range(lowestPoint, highestPoint);
        while (true)
        {
            if (y < transform.position.y + spawnPointVariation && y > transform.position.y - spawnPointVariation)
            {
                y = UnityEngine.Random.Range(lowestPoint, highestPoint);
            }
            else
            {
                break;
            }
        }
        transform.position = new Vector3(transform.position.x, y, transform.position.z);

        GameObject cloudObj = Instantiate(cloud[UnityEngine.Random.Range(0, cloud.Length)]);


        cloudObj.transform.localScale = VectorLibrary.RandomVector3Whole(cloudScale.x, cloudScale.y);

        if (cloudFlip)
        {
            Vector3 flippedCloud = cloudObj.transform.localScale;

            if (UnityEngine.Random.Range(0, 3) != 0)
            {
                flippedCloud = new Vector3(-flippedCloud.x, flippedCloud.y, flippedCloud.z);
            }
            cloudObj.transform.localScale = flippedCloud;
        }

        cloudObj.transform.rotation = lookRotation;
        clouds++;

        StartCoroutine(MovementLibrary.LerpOnce(cloudObj, lerpDuration, transform.position, target.position, true, MovementLibrary.ObjectLerpType.Position, RemoveIndex));

        void RemoveIndex()
        {
            clouds--;
            Destroy(cloudObj);
        }

    }




}
