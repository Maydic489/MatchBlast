using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableManager : MonoBehaviour
{
    [SerializeField] Vector2 _tableSize = new Vector2(8, 8);
    [SerializeField] GameObject tableScale;
    [SerializeField] GameObject tableObject;
    [SerializeField] GameObject debrisPrefab;

    public static TableManager instance = null;
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

    private void Start()
    {
        SetTableSize(12,10);
    }

    public void SetTableSize(int sizeX, int sizeY)
    {
        _tableSize = new Vector2(sizeX, sizeY);
        tableObject.GetComponent<SpriteRenderer>().size = _tableSize;
        AdjustTableSize();
    }

    public void AdjustTableSize()
    {
        Debug.Log(Screen.width + " " + Screen.height);

        float aspecRatioMultiplier = GetAspectRatioMultiplier();

        float tableMultiplier = 1 / _tableSize.x;

        float newScale = tableMultiplier * aspecRatioMultiplier;

        tableScale.transform.localScale = new Vector3(newScale, newScale, 1);
    }

    float GetAspectRatioMultiplier()
    {
        Vector2 screenRatio = GetScreenAspectRatio();

        Debug.Log(screenRatio);

        if(screenRatio == new Vector2(9,16))
        {
            return 1.07f;
        }
        else if(screenRatio == new Vector2(10, 16))
        {
            return 1.2f;
        }
        else if(screenRatio == new Vector2(1, 2))
        {
            return 0.95f;
        }

        return 1;
    }

    Vector2 GetScreenAspectRatio()
    {
        int divideNum = GetSameDivideNumber(Screen.width, Screen.height);

        Debug.Log(divideNum);

        float widthAspect = (float)Screen.width / divideNum;
        float heightAspect = (float)Screen.height / divideNum;

        return new Vector2(widthAspect, heightAspect);
    }

    int GetSameDivideNumber(int a, int b)
    {
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }
}
