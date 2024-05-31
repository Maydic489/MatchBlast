using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleObject : BasePiece
{
    public override void SetPieceData(Vector2 slotIndex, PieceType piecetype = PieceType.Red, bool randomColor = true, PieceType discoColor = PieceType.Red)
    {
        if (pieceData == null)
        {
            pieceData = new PieceData(piecetype, slotIndex);
        }
        else
        {
            renderer.sortingOrder = (int)slotIndex.y * -1;
            pieceData.slotIndex = slotIndex;
            pieceData.pieceType = piecetype;
        }
    }

    public override void MovePieceDown(float newPosY, float time)
    {
        if(pieceData.pieceType == PieceType.ObstacleBig)
        {
            if (!CheckSlotForBigObstacle())
                return;
        }

        //fall out of the table
        if(pieceData.pieceType == PieceType.Obstacle && pieceData.slotIndex.y == TableManager.instance.TableSize.y - 1)
        {
            newPosY = TableManager.instance.TableSize.y - 30;
            time = 1.5f;

            LeaveCurrentSlot();
        }

        transform.DOLocalMoveY(newPosY, time).SetEase(easeType);

        if (time == 0.7f)
        {
            transform.localScale = new Vector3(0.5f, 1.5f, 1);
            transform.DOScaleY(1, time).SetEase(easeType);
            transform.DOScaleX(1, time).SetEase(easeType);
        }
    }

    bool CheckSlotForBigObstacle()
    {
        var freeSlots = TableManager.instance.FindTwoLowestAvailableSlot((int)pieceData.slotIndex.x);

        if (freeSlots == null)
            return false;
        else
            return true;
    }

    public override void LeaveCurrentSlot()
    {
        TableManager.instance.PieceLeaveCurrentSlot(pieceData.slotIndex);
    }
}
