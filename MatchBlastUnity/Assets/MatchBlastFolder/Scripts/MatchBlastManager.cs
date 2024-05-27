using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchBlastManager : MonoBehaviour
{
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
    }
}
