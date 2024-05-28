using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TableManager : MonoBehaviour
{
    [SerializeField] Vector2 _tableSize = new Vector2(8, 8);
    public Vector2 TableSize { get { return _tableSize; } }

    [SerializeField] GameObject tableScale;
    public GameObject tableObject;
    [SerializeField] GameObject spawnerPrefab;
    public GameObject piecePrefab;
    [SerializeField] GameObject debrisPrefab;

    public bool isTableReady = false; //ready means all pieces are in place
    
    public TableSlot[][] TableSlotArray; //[y][x]
    List<PieceSpawner> SpawnerList = new List<PieceSpawner>();

    public int randomSeed = 60;

    public UnityEvent tableReadyEvent;

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
            return;
        }

        if (tableReadyEvent == null)
        {
            tableReadyEvent = new UnityEvent();
        }

        
        //int newSeed = Random.Range(0, 1000);
        randomSeed = 378;
        //Debug.Log("New Seed: " + newSeed);
    }

    private void Start()
    {
        SetTableSize(10,10);
        PlacingEachSpawner();

        RefillTable(true);
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

        return new Vector2(startX + indexX, startY - IndexY);
    }

    public TableSlot FindLowestAvailableSlot(int columnIndex)
    {
        for (int i = (int)_tableSize.y - 1; i >= 0; i--)
        {
            if (!TableSlotArray[i][columnIndex].havePiece)
            {
                return TableSlotArray[i][columnIndex];
            }
        }

        return null;
    }

    void PlacingEachSpawner()
    {
        //find the leftest collumn position
        float startX = (-_tableSize.x / 2) + 0.5f;

        for(int i = 0; i < _tableSize.x; i++)
        {
            SpawnerList.Add(CreateSpawner(i, new Vector2(startX, _tableSize.y * 2)));

            startX++;
        }
    }

    PieceSpawner CreateSpawner(int columnIndex, Vector2 position)
    {
        GameObject spawner = Instantiate(spawnerPrefab, position, Quaternion.identity, tableObject.transform);
        spawner.transform.localPosition = position;
        spawner.GetComponent<PieceSpawner>().columnIndex = columnIndex;

        return spawner.GetComponent<PieceSpawner>();
    }

    void RefillTable(bool isFirstTime = false)
    {
        Debug.Log("Refill Table");
        foreach (PieceSpawner spawner in SpawnerList)
        {
            spawner.MoveNewPieceDown(isFirstTime);
        }

        Invoke(nameof(InvokeTableReadyEvent), 3);
    }

    void InvokeTableReadyEvent()
    {
        tableReadyEvent.Invoke();
    }
}

[System.Serializable]
public class TableSlot
{
    public Vector2 slotIndex = Vector2.zero;
    public Vector2 SlotPosition = Vector2.zero;
    public PieceObject pieceObject { get; set; }
    public bool havePiece { get; private set; }

    public TableSlot(Vector2 index, Vector2 position, bool havePiece = false)
    {
        slotIndex = index;
        SlotPosition = position;
        this.havePiece = havePiece;
    }

    public void setOccupyStatus(bool status)
    {
        havePiece = status;
    }
}
