using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

namespace _188898
{
    public enum GameState { GS_PAUSEMENU, GS_GAME, GS_LEVELCOMPLETED, GS_GAME_OVER, GS_OPTIONS }

    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;
        public GameState currentGameState;
        public GameObject inGameCanvas;
        public Text scoreText;
        private int score = 0;
        public Image[] keysTab;
        private int keysFound = 0;
        public Image[] livesTab;
        private int lives = 3;
        public Text timeText;
        private float timer = 0;
        public Text killsText;
        private int kills = 0;
        public GameObject cannotFinishText;
        public GameObject pauseMenuCanvas;
        public GameObject levelCompletedCanvas;
        public GameObject opstionsCanvas;
        public GameObject gameOverCanvas;
        private const string keyHighScore = "HighScoreLevel1_188898";
        public Text scoreTextCompleted;
        public Text highScoreText;
        public Text qualityText;
        public bool coroutineRunning = false;

        private void Awake()
        {
            instance = this;

            if (!PlayerPrefs.HasKey(keyHighScore))
            {
                PlayerPrefs.SetInt(keyHighScore, 0);
            }

            scoreText.text = score.ToString();
            for (int i = 0; i < keysTab.Length; i++)
            {
                keysTab[i].color = Color.grey;
            }
            livesTab[3].enabled = false;
            timeText.text = string.Format("{0:00}:{1:00}", 0, 0);
            killsText.text = kills.ToString();
            qualityText.text = "Quality: " + QualitySettings.names[QualitySettings.GetQualityLevel()];

            InGame();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (currentGameState == GameState.GS_PAUSEMENU)
                {
                    InGame();
                }
                else if (currentGameState == GameState.GS_GAME)
                {
                    PauseMenu();
                }
            }

            if (currentGameState == GameState.GS_GAME)
            {
                timer += Time.deltaTime;
                int minutes = (int)(timer / 60);
                int seconds = (int)(timer % 60);
                timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
        }

        void SetGameState(GameState newGameState)
        {
            currentGameState = newGameState;

            inGameCanvas.SetActive(currentGameState == GameState.GS_GAME);
            pauseMenuCanvas.SetActive(currentGameState == GameState.GS_PAUSEMENU);
            levelCompletedCanvas.SetActive(currentGameState == GameState.GS_LEVELCOMPLETED);
            opstionsCanvas.SetActive(currentGameState == GameState.GS_OPTIONS);
            gameOverCanvas.SetActive(currentGameState == GameState.GS_GAME_OVER);

            if (newGameState == GameState.GS_LEVELCOMPLETED)
            {
                Scene currentScene = SceneManager.GetActiveScene();

                if (currentScene.name == "188898")
                {
                    int timeBonus = 300 - ((int)(timer / 60) * 60 + (int)(timer % 60));
                    if (timeBonus > 0)
                    {
                        score += timeBonus;
                    }

                    int highScore = PlayerPrefs.GetInt(keyHighScore);

                    if (highScore < score)
                    {
                        highScore = score;
                        PlayerPrefs.SetInt(keyHighScore, highScore);
                    }

                    scoreTextCompleted.text = "Your score = " + score;
                    highScoreText.text = "The best score = " + highScore;
                }
            }

            if (newGameState == GameState.GS_OPTIONS || currentGameState == GameState.GS_PAUSEMENU)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }

        public void PauseMenu()
        {
            SetGameState(GameState.GS_PAUSEMENU);
        }

        public void InGame()
        {
            SetGameState(GameState.GS_GAME);
        }

        public void LevelCompleted()
        {
            SetGameState(GameState.GS_LEVELCOMPLETED);
        }

        public void GameOver()
        {
            SetGameState(GameState.GS_GAME_OVER);
        }

        public void Options()
        {
            SetGameState(GameState.GS_OPTIONS);
        }

        public void AddPoints(int points)
        {
            score += points;
            scoreText.text = score.ToString();
        }

        public void AddKeys(Color color)
        {
            keysTab[keysFound].color = color;
            keysFound++;
        }

        public void AddLive()
        {
            lives++;
            livesTab[lives - 1].enabled = true;
        }

        public void RemoveLive()
        {
            lives--;
            if (lives >= 0)
            {
                livesTab[lives].enabled = false;
            }
        }

        public void AddKill()
        {
            kills++;
            killsText.text = kills.ToString();
        }

        public void OnResumeButtonClicked()
        {
            InGame();
        }

        public void OnRestartButtonClicked()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void OnReturnToMainMenuButtonClicked()
        {
            int sceneIndex = SceneUtility.GetBuildIndexByScenePath("Scenes/Main Menu");
            if (sceneIndex >= 0)
            {
                SceneManager.LoadSceneAsync(sceneIndex); //³adowanie sceny ³¹cz¹cej gry
            }
            else
            {
                SceneManager.LoadScene("MainMenu_188898");
                //sceneIndex jest równe -1. Nie znaleziono sceny.
                //³adowanie innej sceny docelowo na laboratorium
            }
        }

        public void OnOptionsButtonClicked()
        {
            Options();
        }

        public void IncreaseQuality()
        {
            QualitySettings.IncreaseLevel();
            qualityText.text = "Quality: " + QualitySettings.names[QualitySettings.GetQualityLevel()];
        }

        public void DecreaseQuality()
        {
            QualitySettings.DecreaseLevel();
            qualityText.text = "Quality: " + QualitySettings.names[QualitySettings.GetQualityLevel()];
        }

        public void SetVolume(Slider slider)
        {
            AudioListener.volume = slider.value;
        }

        public IEnumerator NotEnoughGems()
        {
            coroutineRunning = true;
            cannotFinishText.SetActive(true);
            yield return new WaitForSeconds(5.0f);
            cannotFinishText.SetActive(false);
            coroutineRunning = false;
        }
    }
}
