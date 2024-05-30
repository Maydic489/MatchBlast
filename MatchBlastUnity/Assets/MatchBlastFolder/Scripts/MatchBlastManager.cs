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
        TableManager.instance.GameStageA();
    }
    public void StartGameStageB()
    {
        TableManager.instance.GameStageB();
    }
    public void StartGameStageC()
    {
        TableManager.instance.GameStageC();
    }

    public void StopGame()
    {
        TableManager.instance.ClearTable();
    }
}
