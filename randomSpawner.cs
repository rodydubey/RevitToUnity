using UnityEngine;
public class randomSpawner : MonoBehaviour
{

    //Spawn this object
    public GameObject spawnObject;
    public GameObject spawnObject1;
    public float maxTime = 4;
    public float minTime = 2;

    //current time
    private float time;

    //The time to spawn the object
    private float spawnTime;
    public static int numberOfAgent = 0;
    public int agent = 0;
    
    void Start()
    {
        SetRandomTime();
        time = minTime;
    }

    void FixedUpdate()
    {

        //Counts up
        time += 15*Time.deltaTime;
        agent = numberOfAgent;
        //Check if its the right time to spawn the object
        if (time >= spawnTime)
        {
            //SpawnObject();
            SetRandomTime();
        }
    }


    //Spawns the object and resets the time
    void addNumberOfAgent()
    {
        numberOfAgent++;
    }
    public static void subtractNumberOfAgent()
    {
        numberOfAgent--;
    }
    void SpawnObject()
    {
        time = 2;
        int randomIndex = Random.Range(1, 9);
        int spawnPointX;
        int spawnPointY;
        if (randomIndex == 1)
        {
            spawnPointX = Random.Range(70, 77);
            spawnPointY = Random.Range(28, 33);
            Vector3 spawnPosition = new Vector3(spawnPointX, 0, spawnPointY);
            Instantiate(spawnObject, spawnPosition, spawnObject.transform.rotation);
        }
        if (randomIndex == 2)
        {
            spawnPointX = Random.Range(51, 55);
            spawnPointY = Random.Range(-26, -31);
            Vector3 spawnPosition = new Vector3(spawnPointX, 0, spawnPointY);
            Instantiate(spawnObject1, spawnPosition, spawnObject1.transform.rotation);
        }
        if (randomIndex == 3)
        {
            spawnPointX = Random.Range(-69, -74);
            spawnPointY = Random.Range(24, 27);
            Vector3 spawnPosition = new Vector3(spawnPointX, 0, spawnPointY);
            Instantiate(spawnObject, spawnPosition, spawnObject.transform.rotation);
        }
        if (randomIndex == 4)
        {
            spawnPointX = Random.Range(8, 13);
            spawnPointY = Random.Range(48, 54);
            Vector3 spawnPosition = new Vector3(spawnPointX, 0, spawnPointY);
            Instantiate(spawnObject1, spawnPosition, spawnObject1.transform.rotation);
        }
        if (randomIndex == 5)
        {
            spawnPointX = Random.Range(-54, -58);
            spawnPointY = Random.Range(20, 21);
            Vector3 spawnPosition = new Vector3(spawnPointX, 0, spawnPointY);
            Instantiate(spawnObject, spawnPosition, spawnObject.transform.rotation);
        }
        if (randomIndex == 6)
        {
            spawnPointX = Random.Range(-34, -46);
            spawnPointY = Random.Range(45, 49);
            Vector3 spawnPosition = new Vector3(spawnPointX, 0, spawnPointY);
            Instantiate(spawnObject1, spawnPosition, spawnObject1.transform.rotation);
        }
        if (randomIndex == 7)
        {
            spawnPointX = Random.Range(-48, -49);
            spawnPointY = Random.Range(-20, -22);
            Vector3 spawnPosition = new Vector3(spawnPointX, 0, spawnPointY);
            Instantiate(spawnObject, spawnPosition, spawnObject.transform.rotation);
        }
        if (randomIndex == 8)
        {
            spawnPointX = Random.Range(7, 7);
            spawnPointY = Random.Range(8, 11);
            Vector3 spawnPosition = new Vector3(spawnPointX, 0, spawnPointY);
            Instantiate(spawnObject1, spawnPosition, spawnObject1.transform.rotation);
        }
        addNumberOfAgent();
    }

    //Sets the random time between minTime and maxTime
    void SetRandomTime()
    {
        spawnTime = Random.Range(minTime, maxTime);
    }

}