using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceSpawner : MonoBehaviour
{
    public int columnIndex;
    List<PieceObject> piecePool = new List<PieceObject>();

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
                piece.MovePieceDown(freeSlot.SlotPosition.y);

                freeSlot.setOccupyStatus(true);
                freeSlot.pieceObject = piece;

                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
