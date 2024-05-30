using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceSpawner : MonoBehaviour
{
    public int columnIndex;
    List<PieceObject> piecePool = new List<PieceObject>();
    public int specialPieceIndexY = -1;
    public PieceType specialPiece;
    public PieceType discoColor;


    private void Start()
    {
        CreatePiecePool();

        //StartCoroutine(MoveEachNewPieceDown(true));
    }

    void CreatePiecePool()
    {
        for (int i = (int)TableManager.instance.TableSize.y - 1; i >= 0; i--)
        {
            PieceObject newPiece = Instantiate(TableManager.instance.piecePrefab.GetComponent<PieceObject>(), this.transform.localPosition, Quaternion.identity, TableManager.instance.tableObject.transform);
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

        foreach (PieceObject piece in piecePool)
        {
            if(piece.gameObject.activeSelf == false || isFirstTime)
            {
                if(!isFirstTime)
                    piece.gameObject.SetActive(true);

                piece.transform.localPosition = this.transform.localPosition;

                freeSlot = TableManager.instance.FindLowestAvailableSlot(columnIndex);

                piece.SetPieceData(freeSlot.slotIndex);
                piece.MovePieceDown(freeSlot.SlotPosition.y, 0.7f);

                freeSlot.setOccupyStatus(true);
                freeSlot.pieceObject = piece;

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
        foreach (PieceObject piece in piecePool)
        {
            if (piece.gameObject.activeSelf != true)
            {
                piece.MovePieceHere(this.transform.localPosition);
                piece.gameObject.SetActive(true);
                piece.SetPieceData(new Vector2(columnIndex, -1));
                //piece.SetPieceColor();
            }
        }
    }

    IEnumerator MoveActivePiecesDownInterval()
    {
        //yield return new WaitForEndOfFrame();

        TableSlot freeSlot;
        PieceObject pieceToFall = new PieceObject();
        int lowestPieceIndexY = -2; //lower number means higher position

        while (TableManager.instance.FindLowestAvailableSlot(columnIndex) != null)
        {
            freeSlot = TableManager.instance.FindLowestAvailableSlot(columnIndex);

            foreach (PieceObject piece in piecePool)
            {
                if (piece.gameObject.activeSelf == true && piece.pieceData.slotIndex.y < freeSlot.slotIndex.y)
                {
                    if(piece.pieceData.slotIndex.y > lowestPieceIndexY || lowestPieceIndexY == -2)
                    {
                        lowestPieceIndexY = (int)piece.pieceData.slotIndex.y;
                        pieceToFall = piece;
                    }
                }
            }

            if (pieceToFall == null)
                yield break;

            float fallTime = pieceToFall.pieceData.slotIndex.y < 0 ? 0.7f : 0.5f;

            bool isSpecialPiece = false;
            if((specialPiece == PieceType.Bomb || specialPiece == PieceType.Disco)
                && freeSlot.slotIndex.y <= specialPieceIndexY && pieceToFall.pieceData.slotIndex.y < 0)
            {
                isSpecialPiece = true;
            }

            //Debug.Log("move piece down " + pieceToFall.pieceData.slotIndex + " to " + freeSlot.slotIndex);
            
            pieceToFall.LeaveCurrentSlot();

            if (isSpecialPiece)
            {
                if(specialPiece == PieceType.Bomb)
                    pieceToFall.SetPieceData(freeSlot.slotIndex, specialPiece, false);
                else if(specialPiece == PieceType.Disco)
                    pieceToFall.SetPieceData(freeSlot.slotIndex, specialPiece, false, discoColor);

                //reset checker
                specialPieceIndexY = -1;
                specialPiece = PieceType.Red;
            }
            else if (pieceToFall.pieceData.pieceType == PieceType.Bomb || pieceToFall.pieceData.pieceType == PieceType.Disco)
            {
                pieceToFall.SetPieceData(freeSlot.slotIndex, pieceToFall.pieceData.pieceType, false);
            }
            else
                pieceToFall.SetPieceData(freeSlot.slotIndex);

            pieceToFall.MovePieceDown(freeSlot.SlotPosition.y, fallTime);

            freeSlot.setOccupyStatus(true);
            freeSlot.pieceObject = pieceToFall;
            
            //yield return new WaitForSeconds(0.1f);
            //yield return new WaitForEndOfFrame();

            pieceToFall = null;
            lowestPieceIndexY = -2;
        }
    }

    public void SpawnSpecialPiece(Vector2 slotIndex, PieceType specialType, PieceType _discoColor)
    {
        specialPieceIndexY = (int)slotIndex.y;
        specialPiece = specialType;
        discoColor = _discoColor;
    }
}
