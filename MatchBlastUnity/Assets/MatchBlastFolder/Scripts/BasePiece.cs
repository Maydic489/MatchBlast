using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePiece : MonoBehaviour
{
    public PieceData pieceData;
    public SpriteRenderer renderer;
    public Ease easeType = Ease.OutBounce;

    public virtual void MovePieceDown(float newPosY, float time)
    {
        transform.DOLocalMoveY(newPosY, time).SetEase(easeType);

        if (time == 0.7f)
        {
            transform.localScale = new Vector3(0.5f, 1.5f, 1);
            transform.DOScaleY(1, time).SetEase(easeType);
            transform.DOScaleX(1, time).SetEase(easeType);
        }
    }

    public virtual void MovePieceHere(Vector2 newPos)
    { transform.localPosition = newPos; }
    public virtual void ResetPiece()
    { }
    public virtual void ShakingPiece()
    { }
    public virtual void SetPieceData(Vector2 slotIndex, PieceType piecetype = PieceType.Red, bool randomColor = true, PieceType discoColor = PieceType.Red)
    { }
    public virtual void LeaveCurrentSlot()
    { }
    public virtual void SetPieceColor(bool randomColor = true, PieceType color = PieceType.Red, PieceType discoColor = PieceType.Red)
    { }
    public virtual void HighlightPotentailBonusPiece(PieceType color = PieceType.Bomb)
    { }
    public virtual void DisableHighlight()
    { }
    //public abstract PieceType GetRandomPieceColor();

    public virtual void DestroyPiece()
    { }
    //public abstract IEnumerator DiscoAnimation();

    //public abstract void FlipDisco();

    //public abstract Color GetDiscoColor(PieceType discoColor);
}
