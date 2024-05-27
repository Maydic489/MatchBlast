using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableManager : MonoBehaviour
{
    [SerializeField] Vector2 _tableSize = new Vector2(8, 8);
    [SerializeField] GameObject tableScale;
    [SerializeField] GameObject tableObject;
    [SerializeField] GameObject spawnerPrefab;
    public GameObject piecePrefab;
    [SerializeField] GameObject debrisPrefab;
    
    public TableSlot[][] TableSlotArray;
    List<PieceSpawner> SpawnerList = new List<PieceSpawner>();

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
        SetTableSize(9,10);
        PlacingEachSpawner();
    }

    public void SetTableSize(int sizeX, int sizeY)
    {
        _tableSize = new Vector2(sizeX, sizeY);
        tableObject.GetComponent<SpriteRenderer>().size = _tableSize;
        AdjustTableSize();

        //create table slot array
        TableSlotArray = new TableSlot[sizeY][];
        for (int i = 0; i < sizeY; i++)
        {
            TableSlotArray[i] = new TableSlot[sizeX];

            for (int j = 0; j < sizeX; j++)
            {
                TableSlotArray[i][j] = new TableSlot(new Vector2(j, i), FindSlotPosition(j, i));
            }
        }
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

    Vector2 FindSlotPosition(int indexX, int IndexY)
    {
        float startX = (-_tableSize.x / 2) + 0.5f;
        float startY = (_tableSize.y / 2) - 0.5f;

        Debug.Log("X: " + (startX + indexX) + " Y: " + (startY - IndexY));

        return new Vector2(startX + indexX, startY - IndexY);
    }

    void PlacingEachSpawner()
    {
        //find the leftest collumn position
        float startX = (-_tableSize.x / 2) + 0.5f;

        for(int i = 0; i < _tableSize.x; i++)
        {
            SpawnerList.Add(CreateSpawner(new Vector2(startX, _tableSize.y * 2)));

            startX++;
        }
    }

    PieceSpawner CreateSpawner(Vector2 position)
    {
        GameObject spawner = Instantiate(spawnerPrefab, position, Quaternion.identity, tableObject.transform);
        spawner.transform.localPosition = position;

        return spawner.GetComponent<PieceSpawner>();
    }
}

[System.Serializable]
public class TableSlot
{
    public Vector2 slotIndex = Vector2.zero;
    public Vector2 SlotPosition = Vector2.zero;
    public bool havePiece = false;

    public TableSlot(Vector2 index, Vector2 position, bool havePiece = false)
    {
        slotIndex = index;
        SlotPosition = position;
        this.havePiece = havePiece;
    }
}
