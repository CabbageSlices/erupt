using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{

    public Text startText;
    public GameObject startBar;
    public RectTransform transformOfStartBar;

    public GameObject[] players
    {
        get
        {
            return GameObject.FindGameObjectsWithTag("player");
        }
    }

    public float barFilledRightValue = 15.306f;
    public float barEmptyRightValue = 347f;

    public float timeBarStartedFilling = 0;

    public float timeToFillBar = 3;

    public bool isBarFilling = false;

    int numStartPressed = 0;

    public AudioClip onProgressBarFill;

    public AudioSource audioPlayer;

    // Start is called before the first frame update
    void Awake()
    {
        audioPlayer = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!startText.enabled && players.Length > 1)
        {
            displayStartText();
        }
        else if (players.Length < 2 && startText.enabled)
        {
            hideStartText();
        }

        if (isBarFilling)
        {
            float elapsedtime = Time.time - timeBarStartedFilling;

            float progress = elapsedtime / timeToFillBar;
            transformOfStartBar.anchorMax = new Vector2(progress, 1);

            if (progress >= 1)
            {
                startGame();
            }
        }
    }

    void hideStartText()
    {
        startText.enabled = false;
        startBar.SetActive(false);
    }


    void displayStartText()
    {
        startText.enabled = true;
        transformOfStartBar.anchorMax = new Vector2(0, 1);
        startBar.SetActive(true);
    }

    public void startFillingUpBar()
    {
        audioPlayer.PlayOneShot(onProgressBarFill);
        timeBarStartedFilling = Time.time;
        isBarFilling = true;
        transformOfStartBar.anchorMax = new Vector2(0, 1);
    }

    public void onStartPressed()
    {
        if (numStartPressed < 0)
        {
            numStartPressed = 0;
        }

        if (numStartPressed == 0 || !isBarFilling)
        {
            startFillingUpBar();
        }
        numStartPressed++;
    }

    void cancelFillingUpBar()
    {
        audioPlayer.Stop();
        isBarFilling = false;
        transformOfStartBar.anchorMax = new Vector2(0, 1);
    }

    public void startGame()
    {
        SceneManager.LoadScene(1);
    }

    public void onStartReleased()
    {
        numStartPressed--;

        if (numStartPressed <= 0)
        {
            numStartPressed = 0;
            cancelFillingUpBar();
        }
    }
}
