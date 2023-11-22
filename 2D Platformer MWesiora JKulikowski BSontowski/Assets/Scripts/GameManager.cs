using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public enum GameState { GS_PAUSEMENU, GS_GAME, GS_LEVELCOMPLETED, GS_GAME_OVER }

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameState currentGameState = GameState.GS_GAME;
    public Canvas inGameCanvas;
    public Text scoreText;
    private int score = 0;
    public Image[] keysTab;
    private int keysFound = 0;
    public Image[] livesTab;
    private int lives = 3;
    public TMP_Text timeText;
    private float timer = 0;
    public TMP_Text killsText;
    private int kills = 0;

    private void Awake()
    {
        instance = this;

        scoreText.text = score.ToString();
        for (int i = 0; i < keysTab.Length; i++)
        {
            keysTab[i].color = Color.grey;
        }
        livesTab[3].enabled = false;
        timeText.text = string.Format("{0:00}:{1:00}", 0, 0);
        killsText.text = kills.ToString();
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
            else
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

        if (currentGameState == GameState.GS_GAME)
        { 
            inGameCanvas.enabled = true;
        }
        else 
        { 
            inGameCanvas.enabled = false; 
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
}
