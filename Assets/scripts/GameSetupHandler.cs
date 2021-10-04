using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameSetupHandler : MonoBehaviour
{
    GameObject gameManager;
    public GameObject gameManagerPrefab;

    public enum GameState
    {
        PRELOAD,
        COUNTDOWN,
        PLAYING,
        ENDING
    };

    public GameState state;

    float timeCountDownStarted;
    public float numberToCountDownFrom = 5;
    public float timePlayStarted;

    public Text countdownText;

    bool isActuallyPlaying = false;

    public float timeGameEnded;
    public float victoryScreenDisplayTime = 4;

    public AudioClip onStartSFX;
    public AudioClip victorySfx;
    public AudioClip countdownBeepSfx;

    public AudioSource audioPlayer;

    public GameObject[] players
    {
        get
        {
            return GameObject.FindGameObjectsWithTag("player");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (state == GameState.COUNTDOWN)
        {
            float timeElapsed = Time.time - timeCountDownStarted;
            int countdownNumber = Mathf.CeilToInt(numberToCountDownFrom - timeElapsed);

            if (countdownNumber.ToString() != countdownText.text)
            {

                audioPlayer.PlayOneShot(countdownBeepSfx);
            }
            countdownText.text = countdownNumber.ToString();

            if (countdownNumber <= 0)
            {
                startPlaying();
            }
        }
        else if (state == GameState.PLAYING)
        {
            float timeElapsed = Time.time - timePlayStarted;

            if (timeElapsed > 2)
            {
                hideText();
            }

            var _players = players;
            if (isActuallyPlaying && _players.Length == 1)
            {
                onVictory();
            }
        }
        else if (state == GameState.ENDING)
        {
            float timeElapsed = Time.time - timeGameEnded;


            if (timeElapsed > 3)
            {
                goBackToMainMenu();
            }
        }
    }

    private void Awake()
    {
        audioPlayer = GetComponent<AudioSource>();
    }

    void Start()
    {
        countdownText.gameObject.SetActive(false);
        gameManager = GameObject.FindWithTag("gameManager");

        if (gameManager == null)
        {
            createGameManager();
        }

        if (players.Length > 0)
        {
            state = GameState.PRELOAD;
            //we just laoded a scene
            onSceneLoad();
            beginCountDown();
        }

    }

    public void onVictory()
    {
        var _players = players;

        foreach (var player in _players)
        {
            player.GetComponent<Player>().doUnsquish();
            player.GetComponent<Player>().enabled = false;
            player.GetComponent<Rigidbody2D>().isKinematic = true;
            player.GetComponent<RotatedVelocity>().enabled = false;
        }

        countdownText.text = "VICTORY";
        countdownText.gameObject.SetActive(true);
        countdownText.enabled = true;
        state = GameState.ENDING;
        timeGameEnded = Time.time;

        audioPlayer.PlayOneShot(victorySfx);
    }

    public void goBackToMainMenu()
    {
        Destroy(gameManager);

        Destroy(GameObject.FindWithTag("bgm"));

        var _players = players;

        foreach (var player in _players)
        {
            Destroy(player);
        }

        SceneManager.LoadScene(0);
    }

    public void startPlaying()
    {
        isActuallyPlaying = true;
        countdownText.text = "START";
        timePlayStarted = Time.time;

        var _players = players;

        foreach (var player in _players)
        {
            player.GetComponent<Player>().enabled = true;
            player.GetComponent<Rigidbody2D>().isKinematic = false;
            player.GetComponent<RotatedVelocity>().enabled = true;
        }

        state = GameState.PLAYING;

        audioPlayer.PlayOneShot(onStartSFX);
    }

    public void createGameManager()
    {
        GameObject.Instantiate(gameManagerPrefab);
        state = GameState.PLAYING;
    }

    public void beginCountDown()
    {
        state = GameState.COUNTDOWN;
        countdownText.gameObject.SetActive(true);
        timeCountDownStarted = Time.time;
        countdownText.text = numberToCountDownFrom.ToString();
        audioPlayer.PlayOneShot(countdownBeepSfx);
    }

    public void hideText()
    {
        countdownText.enabled = false;
    }


    public void onSceneLoad()
    {
        //get all players and disable their gameplay and stuff
        gameManager.GetComponent<PlayerInputManager>().DisableJoining();
        gameManager.GetComponent<GameManager>().resetReferences();


        var _players = players;

        foreach (var player in _players)
        {
            var playerS = player.GetComponent<Player>();
            playerS.resetReferences();
            playerS.doUnsquish();
            playerS.doUnsquish();
            playerS.enabled = false;
            player.GetComponent<Rigidbody2D>().isKinematic = true;
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            player.GetComponent<RotatedVelocity>().enabled = false;
        }

        gameManager.GetComponent<GameManager>().setPlayersToSpawnLocation();
    }
}
