using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    // Main game state objects
    [SerializeField] GameObject startScreen;
    [SerializeField] GameObject endScreen;
    [SerializeField] GameObject startButton;
    [SerializeField] GameObject restartButton;
    [SerializeField] GameObject hexmapObj;

    // updatable objects
    [SerializeField] Text timerText;
    [SerializeField] Text scoreText;
    [SerializeField] Texture2D sprayCursor;
    [SerializeField] Texture2D wallCursor;
    [SerializeField] GameObject bone1;
    [SerializeField] GameObject bone2;
    [SerializeField] GameObject bone3;
    [SerializeField] Text healthText;


    float gameTimer = 0;
    int scoreTracker = 99;
    int healthTracker = 0;
    GameObject activeIconElement = null;
    bool isActiveIcon = false;
    private Hexmap hexmap;


    //Engine Functions
    private void Start()
    {
        Time.timeScale = 0;
        StartGameState(true);
        EndGameState(false);
        hexmap = hexmapObj.GetComponentInChildren<Hexmap>();
    }

    private void Update()
    {
        gameTimer += Time.deltaTime;
        timerText.text = gameTimer.ToString("00.0");

    }

    // Public Functions game functions for buttons and interactions

    public void UpdateScore(int value) // pass an incremental value to this to up score
    {
        scoreTracker += value;
        scoreText.text = "Score: " + scoreTracker.ToString().PadLeft(7, '0');
    }

    public void UpdateHealth(int value) // pass an incremental value to this to up score
    {
        healthTracker -= value;
        healthText.text = "Health: " + healthTracker.ToString().PadLeft(2, '0');
        if (healthTracker < 1)
        {
            EndGame();
            bone1.SetActive(false);
        }
        if (healthTracker < 66)
        {
            bone3.SetActive(false);
        }
        if (healthTracker < 33)
        {
            bone2.SetActive(false);
        }

    }

    // Game functions
    public void StartGame()
    {
        StartGameState(false);
        Time.timeScale = 1;
    }

    public void RestartGame()
    {
        EndGameState(false);
        Time.timeScale = 1;
        scoreTracker = 0;

    }

    public void EndGame()
    {
        Time.timeScale = 0;
        EndGameState(true);
    }

    public void selectItem(string type)
    {
        if (type.ToLower() == "spray")
        {
            Cursor.SetCursor(sprayCursor, Vector2.zero, CursorMode.Auto);
            isActiveIcon = true;
            hexmap.SetBuildingSelection(2);
        }
        if (type.ToLower() == "wall")
        {
            Cursor.SetCursor(wallCursor, Vector2.zero, CursorMode.Auto);
            isActiveIcon = true;
            hexmap.SetBuildingSelection(1);
        }
    }
    public void clearItem()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        hexmap.SetBuildingSelection(0);
    }
    // Functions for handleing game logic
    private void StartGameState(bool isActive) // Handles Start game screen visibility
    {
        startScreen.SetActive(isActive);
        startButton.SetActive(isActive);
    }
    private void EndGameState(bool isActive) //Handles End game screen visibility
    {
        endScreen.SetActive(isActive);
        restartButton.SetActive(isActive);
    }

}
