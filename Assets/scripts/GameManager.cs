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

    public GameObject[] players
    {
        get
        {
            return GameObject.FindGameObjectsWithTag("player");
        }
    }

    [SerializeField]
    public List<GameObject> spawnPositions = new List<GameObject>();

    [SerializeField]
    public Texture2D playerSpriteTexture;

    public Sprite[] playerSprites;

    int spawnPositionIndexToUseNext = 0;
    int playerSpriteToUseNext = 0;

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
        playerSprites = Resources.LoadAll<Sprite>(playerSpriteTexture.name);
        Debug.Log(playerSprites.Length);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void onPlayerJoin(PlayerInput input)
    {
        int newNumber = players.Length;
        var player = input.gameObject.GetComponent<Player>();
        player.setPlayerNumber(newNumber);
        setSpawnPosition(input.gameObject);
        setPlayerSprite(input.gameObject);
    }

    public void onSpawnPlayer(GameObject player)
    {
        int newNumber = players.Length;
        player.GetComponent<Player>().setPlayerNumber(newNumber);
        // setPlayerSprite(player);
    }

    public void setPlayerSprite(GameObject player)
    {
        if (playerSprites.Length == 0)
        {
            return;
        }

        int spriteIndex = Random.Range(0, playerSprites.Length - 1);

        player.GetComponent<SpriteRenderer>().sprite = playerSprites[spriteIndex];
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
