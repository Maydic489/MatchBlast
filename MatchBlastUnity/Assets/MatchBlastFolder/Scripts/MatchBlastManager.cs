using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchBlastManager : MonoBehaviour
{
    public Color red;
    public Color green;
    public Color blue;
    public Color yellow;

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
}
