using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor.Experimental.GraphView;
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
        TableManager.instance.randomSeed += 7;
        UnityEngine.Random.InitState(TableManager.instance.randomSeed);

        if (pieceData == null)
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

    PieceType GetRandomPieceColor()
    {
        return (PieceType)UnityEngine.Random.Range(0, 4);
    }

    public void OnMouseDown()
    {
        Debug.Log("Piece clicked");

        MatchingManager.instance.SelectMatchGroup(this);
    }

    //not really destroy, just return to pool
    public void DestroyPiece()
    {
        this.gameObject.SetActive(false);
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
