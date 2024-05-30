using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TableManager : MonoBehaviour
{
    [SerializeField] Vector2 _tableSize = new Vector2(8, 8);
    public Vector2 TableSize { get { return _tableSize; } }

    [SerializeField] GameObject tableScale;
    public GameObject tableObject;
    [SerializeField] GameObject spawnerPrefab;
    public GameObject piecePrefab;
    public GameObject ObstaclePrefab;
    [SerializeField] FXManager fxManager;


    public bool isTableReady = false; //ready means all pieces are in place, but not calculate matches yet
    int filledColumn = 0;//count how many column is full
    public bool isReadyToTouch { get; private set; }

    public TableSlot[][] TableSlotArray; //[y][x]
    List<PieceSpawner> SpawnerList = new List<PieceSpawner>();

    public int randomSeed = 60;

    Coroutine discoCo;

    public UnityEvent tableReadyEvent;
    public UnityEvent movePieceDownEvent;

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

        if (movePieceDownEvent == null)
        {
            movePieceDownEvent = new UnityEvent();
        }

        //for testing, to get same random table everytimes
        //int newSeed = Random.Range(0, 1000);
        //randomSeed = 378;
        //Debug.Log("New Seed: " + newSeed);
    }

    private void Start()
    {
        SetTableSize(10,10);
        PlacingEachSpawner();

        //ReserveSpotForObstacle(new Vector2(3, 1), PieceType.ObstacleBig);
        ReserveSpotForObstacle(new Vector2(1, 7), PieceType.Obstacle);

        FillTable(true);
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
            if (!TableSlotArray[i][columnIndex].havePiece && !TableSlotArray[i][columnIndex].haveObstacle)
            {
                Debug.Log("Found lowest available slot: " + TableSlotArray[i][columnIndex].slotIndex);
                return TableSlotArray[i][columnIndex];
            }
        }

        //check if every column refilled
        if(filledColumn < _tableSize.y -1)
        {
            Debug.Log("Column " + columnIndex + " is full");
            filledColumn++;
        }
        else
        {
            //Debug.Log("Table is ready");
            filledColumn = 0;
            Invoke(nameof(InvokeTableReadyEvent), 0.5f);
            //InvokeTableReadyEvent();
        }

        return null;
    }

    public TableSlot FindTwoLowestAvailableSlot(int columnIndex)
    {
        for (int i = (int)_tableSize.y - 1; i >= 0; i--)
        {
            if (!TableSlotArray[i][columnIndex].havePiece && !TableSlotArray[i][columnIndex+1].havePiece)
            {
                return TableSlotArray[i][columnIndex];
            }
        }

        //check if every column refilled
        if (filledColumn < _tableSize.y - 1)
        {
            filledColumn++;
        }
        else
        {
            filledColumn = 0;
            Invoke(nameof(InvokeTableReadyEvent), 0.5f);
        }

        return null;
    }

    public void PieceLeaveCurrentSlot(Vector2 slotIndex)
    {
        //for check reused piece
        if(slotIndex.y < 0)
        {
            return;
        }

        TableSlotArray[(int)slotIndex.y][(int)slotIndex.x].setOccupyStatus(false);
        //Debug.Log("Piece leave slot " + slotIndex + " occupy " + TableSlotArray[(int)slotIndex.y][(int)slotIndex.x].havePiece);
    }

    void ReserveSpotForObstacle(Vector2 slotIndex, PieceType obstacleType)
    {
        //Debug.Log("reserve 1");
        //for (int i = (int)slotIndex.y; i >= 0; i--)
        //{
        //    for (int j = (int)slotIndex.x; j < (int)slotIndex.x + 2; j++)
        //    {
        //        TableSlotArray[i][j].SetObstacleStatus(true);
        //        Debug.Log("reserve 2");
        //    }
        //}

        SpawnerList[(int)slotIndex.x].SetupObstacle(obstacleType);
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

    //move all active pieces down
    void MoveActivePiecesDown()
    {
        movePieceDownEvent.Invoke();
    }

    //make inactive piece in pool active and move it to the lowest available slot
    void FillTable(bool isFirstTime = false)
    {
        //Debug.Log("Refill Table");
        //foreach (PieceSpawner spawner in SpawnerList)
        //{
        //    spawner.MoveNewPieceDown(isFirstTime);
        //}

        for (int i = 0; i < _tableSize.x; i++)
        {
            SpawnerList[i].MoveNewPieceDown(isFirstTime);
        }

        //Invoke(nameof(InvokeTableReadyEvent), 1);
    }

    void InvokeTableReadyEvent()
    {
        tableReadyEvent.Invoke();
        

        Invoke(nameof(InvokeReadyToTouch), 0.1f);//the same duration as falling animation
    }

    void InvokeReadyToTouch()
    {
        isReadyToTouch = true;
    }

    //call from pieceObject
    public void CheckIfPieceMatch(BasePiece selectedPiece)
    {
        if (selectedPiece.pieceData.pieceType == PieceType.Bomb)
        {
            UseBome(selectedPiece, selectedPiece.pieceData.slotIndex);
            return;
        }
        else if(selectedPiece.pieceData.pieceType == PieceType.Disco)
        {
            discoCo = StartCoroutine(UseDisco(selectedPiece, selectedPiece.pieceData.discoColor));
            return;
        }

        var matchGroup = MatchingManager.instance.SelectMatchGroup(selectedPiece);

        if(matchGroup != null && matchGroup.Count >= 6)
            SpawnSpecialPiece(matchGroup, selectedPiece.pieceData, MatchingManager.instance.CheckForSpecialPiece(matchGroup));

        if(matchGroup != null)
        {
            DestroyMatchGroup(matchGroup);
        }
    }

    void DestroyMatchGroup(List<BasePiece> matchedPieces)
    {
        Debug.Log("destroy " + matchedPieces.Count + " pieces");

        //foreach(PieceObject piece in matchedPieces)
        //{
        //    Debug.Log("piece slotIndex "+ piece.pieceData.slotIndex);
        //}

        isReadyToTouch = false;

        foreach (BasePiece piece in matchedPieces)
        {
            piece.DestroyPiece();
            //TableSlotArray[(int)piece.pieceData.slotIndex.y][(int)piece.pieceData.slotIndex.x].setOccupyStatus(false);
        }

        DestroyAfterEffect(matchedPieces, matchedPieces[0].pieceData.pieceType);
    }

    void DestroyAfterEffect(List<BasePiece> matchedPieces, PieceType _pieceType)
    {
        List<Vector3> piecesPos = matchedPieces.FindAll(x => x != null).ConvertAll(x => x.transform.localPosition);
        fxManager.PlayPopEffect(piecesPos, _pieceType);

        Invoke(nameof(MoveActivePiecesDown), 0.1f);
        //MoveActivePiecesDown();
    }

    public void UseBome(BasePiece bombPiece, Vector2 pieceIndex)
    {
        List<BasePiece> destroyPieces = new List<BasePiece>();

        for (int i = 0; i < _tableSize.x; i++)
        {
            for (int j = 0; j < _tableSize.y; j++)
            {
                if (i == pieceIndex.x || j == pieceIndex.y)
                {
                    destroyPieces.Add(TableSlotArray[j][i].pieceObject);
                }
            }
        }

        //Sort the pieces by distance to the bomb piece
        destroyPieces.Sort((piece1, piece2) =>
        {
            float distance1 = Vector2.Distance(pieceIndex, piece1.pieceData.slotIndex);
            float distance2 = Vector2.Distance(pieceIndex, piece2.pieceData.slotIndex);
            return distance1.CompareTo(distance2);
        });

        //Destroy the pieces in order
        foreach (BasePiece piece in destroyPieces)
        {
            piece.DestroyPiece();
        }

        DestroyAfterEffect(destroyPieces, PieceType.Bomb);
    }

    IEnumerator UseDisco(BasePiece discoPiece, PieceType discoColor)
    {
        isReadyToTouch = false;

        List<BasePiece> destroyPieces = new List<BasePiece>();
        List<BasePiece> randomOrderPieces = new List<BasePiece>();

        //destroyPiece.Add(discoPiece);

        for (int i = 0; i < _tableSize.x; i++)
        {
            for (int j = 0; j < _tableSize.y; j++)
            {
                if (TableSlotArray[j][i].pieceObject.pieceData.pieceType == discoColor)
                {
                    destroyPieces.Add(TableSlotArray[j][i].pieceObject);
                }
            }
        }

        BasePiece tempPiece = new BasePiece();

        for(int i = 0; i < destroyPieces.Count; i++)
        {
            do
            {
                tempPiece = destroyPieces[Random.Range(0, destroyPieces.Count)];
            }
            while (randomOrderPieces.Contains(tempPiece));

            randomOrderPieces.Add(tempPiece);
        }

        foreach (BasePiece piece in randomOrderPieces)
        {
            yield return StartCoroutine(fxManager.PlayTrailEffect(discoPiece.transform.localPosition, piece.transform.localPosition, discoColor));

            piece.DestroyPiece();
        }


        discoPiece.DestroyPiece();
        DestroyAfterEffect(new List<BasePiece> { discoPiece }, discoColor);
        //DestroyAfterEffect(destroyPiece, discoColor);

        discoCo = null;

        //Invoke(nameof(MoveActivePiecesDown), 0.1f);
    }

    void SpawnSpecialPiece(List<BasePiece> _matchGroup, PieceData pieceData, PieceType specialType)
    {
        SpawnerList[(int)pieceData.slotIndex.x].SetupSpecialPiece(pieceData.slotIndex, specialType, pieceData.pieceType);
    }

    public void ResetPiecesHighLight()
    {
        for (int i = 0; i < TableSize.y; i++)
        {
            for (int j = 0; j < TableSize.x; j++)
            {
                if(TableSlotArray[i][j].pieceObject != null)
                {
                    TableSlotArray[i][j].pieceObject.DisableHighlight();
                }
            }
        }
    }
}

[System.Serializable]
public class TableSlot
{
    public Vector2 slotIndex = Vector2.zero;//x = horizontal, y = vertical
    public Vector2 SlotPosition = Vector2.zero;
    public BasePiece pieceObject { get; set; }
    public bool havePiece { get; private set; }
    public bool haveObstacle { get; private set; }

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

    public void SetObstacleStatus(bool status)
    {
        haveObstacle = status;
    }
}
