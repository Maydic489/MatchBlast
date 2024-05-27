using System;
using UnityEngine;

[Serializable]
public class PieceData
{
    public PieceType pieceType;
    public Vector2 gridPosition;

    public PieceData(PieceType pieceType, Vector2 gridPosition)
    {
        this.pieceType = pieceType;
        this.gridPosition = gridPosition;
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
