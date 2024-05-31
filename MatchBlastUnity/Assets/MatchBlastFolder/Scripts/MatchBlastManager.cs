using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchBlastManager : MonoBehaviour
{
    public Color red;
    public Color green;
    public Color blue;
    public Color yellow;
    public Color brown;

    public int moveNum { get; private set; }
    public int starNum { get; private set; }

    public static MatchBlastManager instance = null;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        Application.targetFrameRate = 60;
    }

    public void StartGameStageA()
    {
        moveNum = 20;
        starNum = 4;
        TableManager.instance.GameStageA();
        UIManager.instance.StartGame();
        UIManager.instance.UpdateMovesText(moveNum);
    }
    public void StartGameStageB()
    {
        moveNum = 25;
        starNum = 6;
        TableManager.instance.GameStageB();
        UIManager.instance.StartGame();
        UIManager.instance.UpdateMovesText(moveNum);
    }
    public void StartGameStageC()
    {
        moveNum = 30;
        starNum = 10;
        TableManager.instance.GameStageC();
        UIManager.instance.StartGame();
        UIManager.instance.UpdateMovesText(moveNum);
    }

    public void StopGame()
    {
        TableManager.instance.ClearTable();
    }

    public void StarFall()
    {
        starNum--;

        if(starNum == 0)
        {
            StartCoroutine(UIManager.instance.GameOverScreen());
        }
    }

    public void ValidClick()
    {
        if(moveNum > 0)
        {
            moveNum--;
            UIManager.instance.UpdateMovesText(moveNum);

            if(moveNum == 0)
            {
                StartCoroutine(UIManager.instance.GameOverScreen());
            }
        }
    }
}
