using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceSpawner : MonoBehaviour
{
    public int columnIndex;
    List<BasePiece> piecePool = new List<BasePiece>();
    public int specialPieceIndexY = -1;
    public PieceType specialPiece;
    public PieceType discoColor;

    public bool toSpawnObstacle;
    public PieceType obstacleType;


    private void Start()
    {
        TableManager.instance.movePieceDownEvent.AddListener(RefillColumn);

        CreatePiecePool();

        //StartCoroutine(MoveEachNewPieceDown(true));
    }

    void CreatePiecePool()
    {
        for (int i = (int)TableManager.instance.TableSize.y - 1; i >= 0; i--)
        {
            BasePiece newPiece = Instantiate(TableManager.instance.piecePrefab.GetComponent<BasePiece>(), this.transform.localPosition, Quaternion.identity, TableManager.instance.tableObject.transform);
            piecePool.Add(newPiece);
            //newPiece.gameObject.SetActive(false);
        }
    }

    public void MoveNewPieceDown(bool isFirstTime = false)
    {
        StartCoroutine(MoveEachNewPieceDown(isFirstTime));
    }

    //move available pieces in pool to the lowest available slot
    IEnumerator MoveEachNewPieceDown(bool isFirstTime = false)
    {
        //wait for piece pools to be created
        yield return new WaitForEndOfFrame ();

        TableSlot freeSlot;

        foreach (BasePiece piece in piecePool)
        {
            if(piece.gameObject.activeSelf == false || isFirstTime)
            {
                if(!isFirstTime)
                    piece.gameObject.SetActive(true);

                piece.transform.localPosition = this.transform.localPosition;

                freeSlot = TableManager.instance.FindLowestAvailableSlot(columnIndex);

                //if(freeSlot == null && toSpawnObstacle)
                //{
                //    freeSlot = TableManager.instance.FindTwoLowestAvailableSlot(columnIndex);
                //}

                if (toSpawnObstacle
                    && ((obstacleType == PieceType.Obstacle && freeSlot.slotIndex.y == 0)
                    || (obstacleType == PieceType.ObstacleBig && freeSlot.slotIndex.y == 1)))
                {
                    int pieceIndex = piecePool.FindIndex(x => x == piece);
                    if (obstacleType == PieceType.Obstacle)
                    {
                        var obstacle = Instantiate(TableManager.instance.ObstaclePrefab.GetComponent<ObstacleObject>(), this.transform.localPosition, Quaternion.identity, TableManager.instance.tableObject.transform);
                        obstacle.gameObject.transform.localPosition = this.transform.localPosition;
                        //piecePool[pieceIndex] = obstacle;
                        piecePool.Add(obstacle);

                        obstacle.SetPieceData(freeSlot.slotIndex, PieceType.Obstacle);
                        obstacle.MovePieceDown(freeSlot.SlotPosition.y, 0.7f);

                        freeSlot.setOccupyStatus(true);
                        freeSlot.pieceObject = obstacle;

                        toSpawnObstacle = false;
                    }
                    //else if (obstacleType == PieceType.ObstacleBig)
                    //{
                    //    var bigPiece = Instantiate(TableManager.instance.ObstaclePrefab.GetComponent<ObstacleObject>(), this.transform.localPosition, Quaternion.identity, TableManager.instance.tableObject.transform);
                    //    bigPiece.gameObject.transform.localScale = Vector3.one * 2;
                    //    piecePool[pieceIndex] = bigPiece;

                    //    bigPiece.SetPieceData(freeSlot.slotIndex, PieceType.Obstacle);

                    //    bigPiece.MovePieceDown(freeSlot.SlotPosition.y, 0.7f);

                    //    SetBigObstacleOccupyStatus(freeSlot.slotIndex, bigPiece);

                    //    toSpawnObstacle = false;
                    //}

                    TableManager.instance.FindLowestAvailableSlot(columnIndex);
                    yield break;
                }
                else
                {

                    piece.SetPieceData(freeSlot.slotIndex);
                    piece.MovePieceDown(freeSlot.SlotPosition.y, 0.7f);

                    freeSlot.setOccupyStatus(true);
                    freeSlot.pieceObject = piece;
                }

                yield return new WaitForSeconds(0.1f);
            }
        }

        //trigger table ready event
        TableManager.instance.FindLowestAvailableSlot(columnIndex);
    }

    public void RefillColumn()
    {
        RecallUsedPiece();
        StartCoroutine(MoveActivePiecesDownInterval());
    }

    void RecallUsedPiece()
    {
        foreach (BasePiece piece in piecePool)
        {
            if (piece.gameObject.activeSelf == false)
            {
                piece.ResetPiece();
                piece.MovePieceHere(this.transform.localPosition);
                piece.gameObject.SetActive(true);
                piece.SetPieceData(new Vector2(columnIndex, -1));
                //piece.SetPieceColor();
            }
        }
    }

    IEnumerator MoveActivePiecesDownInterval()
    {
        if(specialPieceIndexY != -1)
        {
            yield return StartCoroutine(SpawnSpecialPiece());
        }

        TableSlot freeSlot;
        BasePiece pieceToFall = new BasePiece();
        int lowestPieceIndexY = -2; //lower number means higher position

        while (TableManager.instance.FindLowestAvailableSlot(columnIndex) != null)
        {
            freeSlot = TableManager.instance.FindLowestAvailableSlot(columnIndex);

            foreach (BasePiece piece in piecePool)
            {
                if (piece.gameObject.activeSelf == true && piece.pieceData.slotIndex.y < freeSlot.slotIndex.y)
                {
                    if(piece.pieceData.slotIndex.y > lowestPieceIndexY || lowestPieceIndexY == -2)
                    {
                        lowestPieceIndexY = (int)piece.pieceData.slotIndex.y;
                        pieceToFall = piece;
                        piece.ResetPiece();
                    }
                }
            }

            if (pieceToFall == null)
                yield break;

            float fallTime = pieceToFall.pieceData.slotIndex.y < 0 ? 0.7f : 0.5f;

            //Debug.Log("move piece down " + pieceToFall.pieceData.slotIndex + " to " + freeSlot.slotIndex);
            
            pieceToFall.LeaveCurrentSlot();

            if (MatchingManager.instance.IsThisASpecialPiece(pieceToFall.pieceData))
            {
                pieceToFall.SetPieceData(freeSlot.slotIndex, pieceToFall.pieceData.pieceType, false, pieceToFall.pieceData.discoColor);
            }
            else if(pieceToFall.pieceData.slotIndex.y >= 0) //still active piece
                pieceToFall.SetPieceData(freeSlot.slotIndex, pieceToFall.pieceData.pieceType, false, default);
            else //piece from pool
                pieceToFall.SetPieceData(freeSlot.slotIndex);

            pieceToFall.MovePieceDown(freeSlot.SlotPosition.y, fallTime);

            if (pieceToFall.pieceData.pieceType == PieceType.Obstacle && pieceToFall.pieceData.slotIndex.y == TableManager.instance.TableSize.y - 1)
            {
                pieceToFall = null;
                lowestPieceIndexY = -2;
            }
            else
            {
                freeSlot.setOccupyStatus(true);
                freeSlot.pieceObject = pieceToFall;

                //yield return new WaitForSeconds(0.1f);
                //yield return new WaitForEndOfFrame();

                pieceToFall = null;
                lowestPieceIndexY = -2;
            }
        }
    }

    public void SetupSpecialPiece(Vector2 slotIndex, PieceType specialType, PieceType _discoColor)
    {
        specialPieceIndexY = (int)slotIndex.y;
        specialPiece = specialType;
        discoColor = _discoColor;
    }

    IEnumerator SpawnSpecialPiece()
    {
        TableSlot freeSlot;
        BasePiece pieceToFall = new BasePiece();

        freeSlot = TableManager.instance.TableSlotArray[specialPieceIndexY][columnIndex];

        if (freeSlot == null)
            yield break;

        foreach (BasePiece piece in piecePool)
        {
            if (piece.gameObject.activeSelf == true && piece.pieceData.slotIndex.y == -1)
            {
                pieceToFall = piece;
            }
        }

        if (pieceToFall == null)
            yield break;

        if (specialPiece == PieceType.Bomb)
            pieceToFall.SetPieceData(freeSlot.slotIndex, specialPiece, false);
        else if (specialPiece == PieceType.Disco)
            pieceToFall.SetPieceData(freeSlot.slotIndex, specialPiece, false, discoColor);

        pieceToFall.MovePieceHere(freeSlot.SlotPosition);

        freeSlot.setOccupyStatus(true);
        freeSlot.pieceObject = pieceToFall;

        //reset checker
        specialPieceIndexY = -1;
        specialPiece = PieceType.Red;
        discoColor = PieceType.Red;
    }

    public void SetupObstacle(PieceType _obstacleType)
    {
        obstacleType = _obstacleType;
        toSpawnObstacle = true;
    }

    void SetBigObstacleOccupyStatus(Vector2 slotIndex, BasePiece bigPiece)
    {
        Debug.Log("set big obs occupy status");

        for (int i = (int)slotIndex.y; i >= 0; i--)
        {
            for(int j = (int)slotIndex.x; j < (int)slotIndex.y + 2; j++)
            {
                TableManager.instance.TableSlotArray[i][j].setOccupyStatus(true);
                TableManager.instance.TableSlotArray[i][j].SetObstacleStatus(true);
                TableManager.instance.TableSlotArray[i][j].pieceObject = bigPiece;
            }
        }

        //while(TableManager.instance.FindLowestAvailableSlot(columnIndex) != null)
        //{
        //    var freeSlot = TableManager.instance.FindLowestAvailableSlot(columnIndex);

        //    freeSlot.setOccupyStatus(true);
        //}
        //while (TableManager.instance.FindLowestAvailableSlot(columnIndex + 1) != null)
        //{
        //    var freeSlot = TableManager.instance.FindLowestAvailableSlot(columnIndex + 1);

        //    freeSlot.setOccupyStatus(true);
        //}
    }

    void FreeBigObstacleOccupyStatus(Vector2 slotIndex)
    {
        for (int i = (int)slotIndex.x; i > 0; i--)
        {
            for (int j = (int)slotIndex.y; j < (int)slotIndex.y + 1; j++)
            {
                TableManager.instance.TableSlotArray[i][j].setOccupyStatus(false);
                TableManager.instance.TableSlotArray[i][j].SetObstacleStatus(false);
                //TableManager.instance.TableSlotArray[i][j].pieceObject = bigPiece;
            }
        }
    }
}
