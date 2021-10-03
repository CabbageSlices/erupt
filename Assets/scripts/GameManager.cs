using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{

    public int numRegionsForRotation = 8;

    public float initialGravity = 8;

    public float currentGravity = 8;

    public float rotationRegionSizeInDegrees
    {
        get
        {
            return 360.0f / numRegionsForRotation;
        }
    }

    [SerializeField]
    public List<GameObject> spawnPositions = new List<GameObject>();

    int spawnPositionIndexToUseNext = 0;

    // Start is called before the first frame update
    void Start()
    {
        GameObject spawnPositionParent = GameObject.FindWithTag("spawnPositions");

        spawnPositions.Clear();
        foreach (Transform child in spawnPositionParent.transform)
        {
            spawnPositions.Add(child.gameObject);
        }
    }

    void Awake()
    {
        currentGravity = initialGravity;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void onPlayerJoin(PlayerInput input)
    {

        setSpawnPosition(input.gameObject);
        // Destroy(input.gameObject);
    }


    public void setSpawnPosition(GameObject player)
    {
        if (spawnPositions.Count == 0)
        {
            return;
        }

        player.transform.position = spawnPositions[spawnPositionIndexToUseNext].transform.position;
        spawnPositionIndexToUseNext = (spawnPositionIndexToUseNext + 1) % spawnPositions.Count;
    }

    public void onPlayerLeft(PlayerInput input)
    {
        // Destroy(input.gameObject);
    }
}
