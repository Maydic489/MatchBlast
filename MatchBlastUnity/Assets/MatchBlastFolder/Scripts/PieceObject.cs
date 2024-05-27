using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor.Experimental.GraphView;
using System;

public class PieceObject : MonoBehaviour
{
    [SerializeField] PieceData pieceData;
    public SpriteRenderer renderer;
    [SerializeField] Ease easeType = Ease.OutBounce;
    int moveDirection = -1;

    private void Start()
    {
        //TODO: randomize piece type
        if(pieceData == null)
            pieceData = new PieceData(PieceType.Red, new Vector2(0, 0));
    }

    public void MovePieceDown(float newPosY)
    {
        transform.DOLocalMoveY(newPosY, 0.5f).SetEase(easeType);
    }

    public void SetPieceData(Vector2 slotIndex)
    {
        if(pieceData == null)
        {
            pieceData = new PieceData(PieceType.Red, slotIndex);
        }
        else
            pieceData.slotIndex = slotIndex;
    }
}

[Serializable]
public class PieceData
{
    public PieceType pieceType;
    public Vector2 slotIndex;

    public PieceData(PieceType pieceType, Vector2 slotIndex)
    {
        this.pieceType = pieceType;
        this.slotIndex = slotIndex;
    }
}

public enum PieceType
{
    Red,
    Green,
    Blue,
    Yellow,
    Bomb,
    Disco
}
