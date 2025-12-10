using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum GameState
    {
        Playing,
        Paused,
        GameOver,
        LevelUp
    }

    public GameState state;
    public GameState previousState;

    [Header("Screens")]
    public GameObject pauseMenuUI;
    public GameObject gameOverUI;
    public GameObject levelUpUI;

    [Header("Current Stats")]
    public TMP_Text currentHealthDisplay;
    public TMP_Text currentRecoveryDisplay;
    public TMP_Text currentMoveSpeedDisplay;
    public TMP_Text currentMightDisplay;
    public TMP_Text currentProjectileSpeedDisplay;
    public TMP_Text currentMagnetDisplay;

    [Header("Game Over Stats")]
    public Image playerCharacterImage;
    public TMP_Text playerCharacterName;
    public TMP_Text levelReached;
    public TMP_Text timeSurvived;
    public List<Image> chosenWeaponsUI = new List<Image>(6);
    public List<Image> chosenItemsUI = new List<Image>(6);

    [Header("Timer")]
    public float timeLimit;
    float timer;
    public TMP_Text timerDisplay;

    public bool levelingUp = false;
    public GameObject playerObject;
    public bool isGameOver = false;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DisableScreens();
    }

    void Update()
    {
        switch(state)
        {
            case GameState.Playing:
                CheckState();
                Time.timeScale = 1f;
                UpdateTimer();
                break;

            case GameState.Paused:
                CheckState();
                Time.timeScale = 0f;
                break;

            case GameState.GameOver:
                if(!isGameOver)
                {
                    isGameOver = true;
                    Time.timeScale = 0f;
                    DisplayResults();
                }
                break;

            case GameState.LevelUp:
                if(!levelingUp)
                {
                    levelingUp = true;
                    Time.timeScale = 0f;
                    levelUpUI.SetActive(true);
                }
                break;

            default:
                Debug.LogError("Unknown game state");
                break;
        }
    }

    public void ChangeState(GameState newState)
    {
        state = newState;
        Debug.Log("Game State changed to: " + state.ToString());
    }

    public void PauseGame()
    {
        if(state != GameState.Paused)
        {
            previousState = state;
            ChangeState(GameState.Paused);
            pauseMenuUI.SetActive(true);
            Debug.Log("Game Paused");
        }
    }
        
    public void ResumeGame()
    {
        if(state == GameState.Paused)
        {
            ChangeState(previousState);
            pauseMenuUI.SetActive(false);
            Debug.Log("Game Resumed");
        }
    }

    void CheckState()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(state == GameState.Playing)
            {
                PauseGame();
            }
            else if(state == GameState.Paused)
            {
                ResumeGame();
            }
        }
    }

    void DisableScreens()
    {
        pauseMenuUI.SetActive(false);
        gameOverUI.SetActive(false);
        levelUpUI.SetActive(false);
    }

    public void GameOver()
    {
        timeSurvived.text = timerDisplay.text;
        ChangeState(GameState.GameOver);
    }

    void DisplayResults()
    {
        gameOverUI.SetActive(true);
    }

    public void AssignPlayerCharacter(CharacterScriptableObject playerCharacter)
    {
        playerCharacterImage.sprite = playerCharacter.Icon;
        playerCharacterName.text = playerCharacter.Name;
    }

    public void AssignLevelReached(int level)
    {
        levelReached.text = level.ToString();
    }

    public void AssignChosenWeapons(List<Image> chosenWeapons, List<Image> chosenItems)
    {
        if(chosenWeapons.Count != chosenWeaponsUI.Count || chosenItems.Count != chosenItemsUI.Count)
        {
            Debug.LogError("Chosen weapons or items count does not match UI slots count.");
            return;
        }

        for(int i = 0; i < chosenWeaponsUI.Count; i++)
        {
            if (chosenWeapons[i].sprite)
            {
                chosenWeaponsUI[i].enabled = true;
                chosenWeaponsUI[i].sprite = chosenWeapons[i].sprite;
            }
            else
            {
                chosenWeaponsUI[i].enabled = false;
            }
        }

        for(int i = 0; i < chosenItemsUI.Count; i++)
        {
            if (chosenItems[i].sprite)
            {
                chosenItemsUI[i].enabled = true;
                chosenItemsUI[i].sprite = chosenItems[i].sprite;
            }
            else
            {
                chosenItemsUI[i].enabled = false;
            }
        }
    }

    void UpdateTimer()
    {
        timer += Time.deltaTime;

        UpdateTimerDisplay();

        if (timer >= timeLimit)
        {
            playerObject.SendMessage("Kill");
        }
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);
        timerDisplay.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StartLevelUp()
    {
        ChangeState(GameState.LevelUp);
        playerObject.SendMessage("RemoveAndApplyUpgrades");
    }

    public void EndLevelUp()
    {
        levelingUp = false;
        Time.timeScale = 1f;
        levelUpUI.SetActive(false);
        ChangeState(GameState.Playing);
    }
}
