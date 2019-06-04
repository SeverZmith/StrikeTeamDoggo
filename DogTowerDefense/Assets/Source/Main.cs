using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    // Main game state objects
    [SerializeField] GameObject startScreen;
    [SerializeField] GameObject endScreen;
    [SerializeField] GameObject startButton;
    [SerializeField] GameObject restartButton;

    // updatable objects
    [SerializeField] Text timerText;
    [SerializeField] Text scoreText;

    float gameTimer = 0;
    int scoreTracker = 0;

    //Engine Functions
    private void Start()
    {
        Time.timeScale = 0;
        StartGameState(true);
        EndGameState(false);
        UpdateScore(5);


    }

    private void Update()
    {
        gameTimer += Time.deltaTime;
        timerText.text = gameTimer.ToString();
    }

    // Public Functions game functions for buttons and interactions

    public void UpdateScore(int value) // pass an incremental value to this to up score
    {
        scoreTracker += value;
        //string stringScore = scoreTracker.ToString().PadLeft(7, '0');
        scoreText.text = "Score: " + scoreTracker.ToString().PadLeft(7, '0');
    }

    // Game functions
    public void StartGame()
    {
        // should start game time
        StartGameState(false);
        Time.timeScale = 1;
    }

    public void RestartGame()
    {
        // should stop game time
        EndGameState(false);
        Time.timeScale = 1;
        scoreTracker = 0;

    }

    public void EndGame()
    {
        Time.timeScale = 0;
        EndGameState(true);
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
