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
        if(pieceData == null)
            pieceData = new PieceData(GetRandomPieceColor(), new Vector2(0, 0));
        else
            SetRendererColor(GetRandomPieceColor());
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

    void SetRendererColor(PieceType color)
    {
        switch (color)
        {
            case PieceType.Red:
                renderer.color = Color.red;
                break;
            case PieceType.Green:
                renderer.color = Color.green;
                break;
            case PieceType.Blue:
                renderer.color = Color.blue;
                break;
            case PieceType.Yellow:
                renderer.color = Color.yellow;
                break;
            case PieceType.Bomb:
                renderer.color = Color.black;
                break;
            case PieceType.Disco:
                renderer.color = Color.magenta;
                break;
        }
    }

    PieceType GetRandomPieceColor()
    {
        return (PieceType)UnityEngine.Random.Range(0, 4);
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
