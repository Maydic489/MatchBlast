using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor.Experimental.GraphView;

public class PieceObject : MonoBehaviour
{
    [SerializeField] PieceData pieceData;
    public SpriteRenderer renderer;
    [SerializeField] Ease easeType = Ease.Linear;
    int moveDirection = -1;

    private void OnMouseDown()
    {
        MovePieceDown();
    }

    public void MovePieceDown()
    {
        transform.DOMoveY(transform.position.y - 5 * moveDirection, 0.5f).SetEase(easeType);

        moveDirection *= -1;
    }
}
