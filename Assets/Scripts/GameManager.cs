using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class GameManager : MonoBehaviour, IDataPersistence
{
    public static GameManager instance;
    public enum GameState
    {
        Gameplay,
        Paused,
        GameOver,
        LevelUp
    }

    public GameState currentState;
    public GameState previousState;

    public bool IsGameOver { get { return currentState == GameState.GameOver; } }
    public bool ChoosingUpgrade { get { return currentState == GameState.LevelUp; } }

    [Header("Damage Text Settings")]
    public Canvas damageTextCanvas;
    public float textFontSize = 100;
    public TMP_FontAsset textFont;
    public Camera referenceCamera;

    [Header("Screens")]
    public GameObject pauseScreen;
    public GameObject resultsScreen;
    public GameObject levelUpScreen;
    int stackedLevelUps = 0;

    [Header("Results Screen Display")]
    public Image chosenCharacterImage;
    public TMP_Text resultDisplay;
    public TMP_Text chosenCharacterName;
    public TMP_Text levelReachedDisplay;
    public TMP_Text timeSurvivedDisplay;

    [Header("Stopwatch")]
    public float timeLimit;

    [HideInInspector] public float stopwatchTime;
    public TMP_Text stopwatchDisplay;

    public GameObject playerObject;

    [Header("Collected coins")]
    public TMP_Text coinsDisplay;
    int coins;

    public int Coins
    {
        get { return coins; }
        set
        {
            coins = value;
            UpdateCoinsDisplay();
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("EXTRA" + this + "DELETED");
            Destroy(gameObject);
        }
        DisableScreens();
        Coins = 0;
    }


    void Update()
    {
        switch (currentState)
        {
            case GameState.Gameplay:
                CheckForPauseAndResume();
                UpdateStopwatch();
                break;
            case GameState.Paused:
                CheckForPauseAndResume();
                break;
            case GameState.GameOver:
                break;
            case GameState.LevelUp:
                break;
            default:
                Debug.LogError("Invalid game state");
                break;
        }
    }

    public static void GenerateFloatingText(string text, Transform target, float duration = 1f, float speed = 1f)
    {
        if (!instance.damageTextCanvas) return;

        if (!instance.referenceCamera) instance.referenceCamera = Camera.main;

        instance.StartCoroutine(instance.GenerateFloatingTextCoroutine(text, target, duration, speed));
    }

    IEnumerator GenerateFloatingTextCoroutine(string text, Transform target, float duration = 1f, float speed = 50f)
    {
        GameObject textObj = new GameObject("Damage Floating Text");
        RectTransform rect = textObj.AddComponent<RectTransform>();
        TextMeshProUGUI tmPro = textObj.AddComponent<TextMeshProUGUI>();

        tmPro.text = text;
        tmPro.horizontalAlignment = HorizontalAlignmentOptions.Center;
        tmPro.verticalAlignment = VerticalAlignmentOptions.Middle;
        tmPro.fontSizeMin = textFontSize;
        if (textFont) tmPro.font = textFont;
        rect.position = referenceCamera.WorldToScreenPoint(target.position);

        Destroy(textObj, duration);

        textObj.transform.SetParent(instance.damageTextCanvas.transform);
        textObj.transform.SetSiblingIndex(0);

        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float t = 0;
        float yOffset = 0;
        Vector3 lastKnownPosition = target.position;

        while (t < duration)
        {
            if (rect) break;

            tmPro.color = new Color(tmPro.color.r, tmPro.color.g, tmPro.color.b, 1 - t / duration);

            if (target)
                lastKnownPosition = target.position;

            yOffset += speed * Time.deltaTime;
            rect.position = referenceCamera.WorldToScreenPoint(lastKnownPosition + new Vector3(0, yOffset));

            yield return w;
            t += Time.deltaTime;
        }

    }

    public void ChangeState(GameState newState)
    {
        previousState = currentState;
        currentState = newState;
    }

    public void PauseGame()
    {
        if (currentState != GameState.Paused)
        {
            ChangeState(GameState.Paused);
            Time.timeScale = 0f;
            pauseScreen.SetActive(true);
            Debug.Log("Game Paused");
        }
    }

    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
        {
            ChangeState(previousState);
            Time.timeScale = 1f;
            pauseScreen.SetActive(false);
            Debug.Log("Game Resumed");
        }
    }

    void CheckForPauseAndResume()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Paused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void DisableScreens()
    {
        pauseScreen.SetActive(false);
        resultsScreen.SetActive(false);
        levelUpScreen.SetActive(false);
    }

    public void GameOver()
    {
        timeSurvivedDisplay.text = stopwatchDisplay.text;
        if (stopwatchTime >= timeLimit)
        {
            resultDisplay.text = "Wygrana!";
            resultDisplay.color = Color.green;
        }
        else
        {
            resultDisplay.text = "Przegrana!";
            resultDisplay.color = Color.red;
        }
        DataPersistenceManager.instance.SaveGame(this);
        ChangeState(GameState.GameOver);
        Time.timeScale = 0f; //Stop the game entirely
        DisplayResults();
    }

    void DisplayResults()
    {
        resultsScreen.SetActive(true);
    }

    public void AssignChosenCharacterUI(CharacterData characterData)
    {
        chosenCharacterImage.sprite = characterData.Icon;
        chosenCharacterName.text = characterData.Name;
    }

    public void AssignLevelReachedUI(int level)
    {
        levelReachedDisplay.text = level.ToString();
    }

    void UpdateStopwatch()
    {
        stopwatchTime += Time.deltaTime;

        UpdateStopwatchDisplay();
    }

    void UpdateStopwatchDisplay()
    {
        int minutes = Mathf.FloorToInt(stopwatchTime / 60);
        int seconds = Mathf.FloorToInt(stopwatchTime % 60);

        stopwatchDisplay.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StartLevelUp(bool evolution = false)
    {
        ChangeState(GameState.LevelUp);
        if (levelUpScreen.activeSelf) stackedLevelUps++;
        else
        {
            levelUpScreen.SetActive(true);
            Time.timeScale = 0f;
            playerObject.SendMessage("RemoveAndApplyUpgrades", evolution);
        }
    }

    public void EndLevelUp()
    {
        Time.timeScale = 1f;
        levelUpScreen.SetActive(false);
        ChangeState(GameState.Gameplay);

        if (stackedLevelUps > 0)
        {
            stackedLevelUps--;
            StartLevelUp(false);
        }
    }

    public void UpdateCoinsDisplay()
    {
        coinsDisplay.text = Coins.ToString();
    }

    public void LoadData(GameData data)
    {
        // Nic nie robi, ale musi być, bo interfejs
    }
    public void SaveData(GameData data)
    {
        data.coinsCollected += Coins;
    }

}
