using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class PieceObject : BasePiece
{
    //public PieceData pieceData;
    //public SpriteRenderer renderer;
    [SerializeField] SpriteRenderer iconRenderer;
    [SerializeField] List<Sprite> pieceSprites = new List<Sprite>();//0 = normal, 1 = bomb, 2 = disco
    [SerializeField] List<Sprite> PieceIcons = new List<Sprite>();//0 = X,1 = O, 2 = SQ, 3 = TRI
    [SerializeField] List<Sprite> hintIconSprite = new List<Sprite>();//0 = bomb, 1 = disco

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

    public override void ResetPiece()
    {
        Debug.Log("Reset Piece",this);
        renderer.transform.localScale = Vector3.one;
        renderer.flipY = false;
        renderer.flipX = false;
    }

    public void OnMouseDown()
    {
        if(TableManager.instance.isReadyToTouch && (MatchBlastManager.instance.moveNum > 0 && MatchBlastManager.instance.starNum > 0))
        {
            TableManager.instance.CheckIfPieceMatch(this);
        }
    }

    public override void MovePieceDown(float newPosY, float time)
    {
        transform.DOLocalMoveY(newPosY, time).SetEase(easeType);

        if (time == 0.7f)
        {
            transform.localScale = new Vector3(0.5f, 1.5f, 1);
            transform.DOScaleY(1, time).SetEase(easeType);
            transform.DOScaleX(1, time).SetEase(easeType);
        }
    }

    public override void MovePieceHere(Vector2 newPos)
    {
        transform.localPosition = newPos;
    }

    public override void ShakingPiece()
    {
        transform.DOShakeRotation(0.5f, 30, 30);
    }

    public override void SetPieceData(Vector2 slotIndex, PieceType piecetype = PieceType.Red, bool randomColor = true, PieceType discoColor = PieceType.Red)
    {
        if (pieceData == null)
        {
            pieceData = new PieceData(piecetype, slotIndex);
        }
        else
        {
            ResetPiece();
            pieceData.slotIndex = slotIndex;
            renderer.sortingOrder = (int)slotIndex.y * -1;
            iconRenderer.sortingOrder = (int)slotIndex.y * -1;
            pieceData.pieceType = piecetype;

            if (pieceData.pieceType == PieceType.Bomb)
                renderer.sprite = pieceSprites[1];
            else if (pieceData.pieceType == PieceType.Disco)
            {
                renderer.sprite = pieceSprites[2];
                pieceData.discoColor = discoColor;
                StartCoroutine(DiscoAnimation());
            }
            else if (!MatchingManager.instance.IsThisASpecialPiece(pieceData))
            {
                renderer.sprite = pieceSprites[0];
            }
        }

        SetPieceColor(randomColor, piecetype, discoColor);
    }

    public override void LeaveCurrentSlot()
    {
        TableManager.instance.PieceLeaveCurrentSlot(pieceData.slotIndex);
    }

    public override void SetPieceColor(bool randomColor = true, PieceType color = PieceType.Red, PieceType discoColor = PieceType.Red)
    {
        if (randomColor)
            color = GetRandomPieceColor();

        renderer.sprite = pieceSprites[0];

        switch (color)
        {
            case PieceType.Red:
                iconRenderer.sprite = PieceIcons[0];
                iconRenderer.color = MatchBlastManager.instance.red;
                renderer.color = MatchBlastManager.instance.red;
                pieceData.pieceType = PieceType.Red;
                break;
            case PieceType.Green:
                iconRenderer.sprite = PieceIcons[1];
                iconRenderer.color = MatchBlastManager.instance.green;
                renderer.color = MatchBlastManager.instance.green;
                pieceData.pieceType = PieceType.Green;
                break;
            case PieceType.Blue:
                iconRenderer.sprite = PieceIcons[2];
                iconRenderer.color = MatchBlastManager.instance.blue;
                renderer.color = MatchBlastManager.instance.blue;
                pieceData.pieceType = PieceType.Blue;
                break;
            case PieceType.Yellow:
                iconRenderer.sprite = PieceIcons[3];
                iconRenderer.color = MatchBlastManager.instance.yellow;
                renderer.color = MatchBlastManager.instance.yellow;
                pieceData.pieceType = PieceType.Yellow;
                break;
            case PieceType.Bomb:
                iconRenderer.sprite = null;
                renderer.sprite = pieceSprites[1];
                renderer.color = Color.white;
                pieceData.pieceType = PieceType.Bomb;
                break;
            case PieceType.Disco:
                iconRenderer.sprite = null;
                renderer.sprite = pieceSprites[2];
                renderer.color = GetDiscoColor(discoColor);
                pieceData.pieceType = PieceType.Disco;
                break;
        }
    }

    public override void HighlightPotentailBonusPiece(PieceType color = PieceType.Bomb)
    {
        switch (color)
        {
            case PieceType.Bomb:
                iconRenderer.sprite = hintIconSprite[0];
                break;
            case PieceType.Disco:
                iconRenderer.sprite = hintIconSprite[1];
                break;
        }
    }

    public override void DisableHighlight()
    {
        switch (pieceData.pieceType)
        {
            case PieceType.Red:
                iconRenderer.sprite = PieceIcons[0];
                renderer.color = MatchBlastManager.instance.red;
                break;
            case PieceType.Green:
                iconRenderer.sprite = PieceIcons[1];
                renderer.color = MatchBlastManager.instance.green;
                break;
            case PieceType.Blue:
                iconRenderer.sprite = PieceIcons[2];
                renderer.color = MatchBlastManager.instance.blue;
                break;
            case PieceType.Yellow:
                iconRenderer.sprite = PieceIcons[3];
                renderer.color = MatchBlastManager.instance.yellow;
                break;
        }
    }

    PieceType GetRandomPieceColor()
    {
        return (PieceType)UnityEngine.Random.Range(0, 4);
    }

    //not really destroy, just return to pool
    public override void DestroyPiece()
    {
        this.gameObject.SetActive(false);
        LeaveCurrentSlot();
    }

    IEnumerator DiscoAnimation()
    {
        while (pieceData.pieceType == PieceType.Disco)
        {
            renderer.transform.DOScale(1.2f, 0.5f);
            FlipDisco();
            yield return new WaitForSeconds(0.5f);
            renderer.transform.DOScale(1, 0.5f);
            FlipDisco();
            yield return new WaitForSeconds(0.5f);
        }

        renderer.transform.localScale = Vector3.one;
        renderer.flipY = false;
        renderer.flipX = false;
    }

    void FlipDisco()
    {
        if(!renderer.flipX && !renderer.flipY)
        {
            renderer.flipX = true;
        }
        else if(renderer.flipX && !renderer.flipY)
        {
            renderer.flipY = true;
        }
        else if(renderer.flipX && renderer.flipY)
        {
            renderer.flipX = false;
        }
        else if(!renderer.flipX && renderer.flipY)
        {
            renderer.flipY = false;
        }
    }

    Color GetDiscoColor(PieceType discoColor)
    {
        switch (discoColor)
        {
            case PieceType.Red:
                return MatchBlastManager.instance.red;
            case PieceType.Green:
                return MatchBlastManager.instance.green;
            case PieceType.Blue:
                return MatchBlastManager.instance.blue;
            case PieceType.Yellow:
                return MatchBlastManager.instance.yellow;
            default:
                return Color.white;
        }
    }
}

[Serializable]
public class PieceData
{
    public PieceType pieceType;
    public Vector2 slotIndex;
    public PieceType discoColor;

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
    Disco,
    Obstacle,
    ObstacleBig
}
