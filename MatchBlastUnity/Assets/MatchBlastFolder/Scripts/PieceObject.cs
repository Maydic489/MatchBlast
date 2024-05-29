using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class PieceObject : MonoBehaviour
{
    public PieceData pieceData;
    public SpriteRenderer renderer;
    [SerializeField] Ease easeType = Ease.OutBounce;
    int moveDirection = -1;

    private void Start()
    {
        //7, 
        //TableManager.instance.randomSeed += 7;
        //UnityEngine.Random.InitState(TableManager.instance.randomSeed);

        if (pieceData == null)
            pieceData = new PieceData(GetRandomPieceColor(), new Vector2(0, 0));
        else
            SetPieceColor();
    }

    public void OnMouseDown()
    {
        if(TableManager.instance.isReadyToTouch)
        {
            TableManager.instance.CheckIfPieceMatch(this);
        }
    }

    public void MovePieceDown(float newPosY)
    {
        transform.DOLocalMoveY(newPosY, 0.5f).SetEase(easeType);
    }

    public void MovePieceHere(Vector2 newPos)
    {
        transform.localPosition = newPos;
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

    public void LeaveCurrentSlot()
    {
        TableManager.instance.PieceLeaveCurrentSlot(pieceData.slotIndex);
    }

    public void SetPieceColor(bool randomColor = true, PieceType color = PieceType.Red)
    {
        if (randomColor)
            color = GetRandomPieceColor();

        switch (color)
        {
            case PieceType.Red:
                renderer.color = Color.red;
                pieceData.pieceType = PieceType.Red;
                break;
            case PieceType.Green:
                renderer.color = Color.green;
                pieceData.pieceType = PieceType.Green;
                break;
            case PieceType.Blue:
                renderer.color = Color.blue;
                pieceData.pieceType = PieceType.Blue;
                break;
            case PieceType.Yellow:
                renderer.color = Color.yellow;
                pieceData.pieceType = PieceType.Yellow;
                break;
            case PieceType.Bomb:
                renderer.color = Color.black;
                pieceData.pieceType = PieceType.Bomb;
                break;
            case PieceType.Disco:
                renderer.color = Color.magenta;
                pieceData.pieceType = PieceType.Disco;
                break;
        }
    }

    public void HighlightPotentailBonusPiece(PieceType color = PieceType.Bomb)
    {
        switch (color)
        {
            case PieceType.Bomb:
                renderer.color = Color.cyan;
                break;
            case PieceType.Disco:
                renderer.color = Color.magenta;
                break;
        }
    }

    public void DisableHighlight()
    {
        switch (pieceData.pieceType)
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
        }
    }

    PieceType GetRandomPieceColor()
    {
        return (PieceType)UnityEngine.Random.Range(0, 4);
    }

    //not really destroy, just return to pool
    public void DestroyPiece()
    {
        this.gameObject.SetActive(false);
        LeaveCurrentSlot();
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
