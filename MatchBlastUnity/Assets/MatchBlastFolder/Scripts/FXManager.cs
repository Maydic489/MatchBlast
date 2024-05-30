using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoBehaviour
{
    [SerializeField] ParticleSystem popFX;
    [SerializeField] Transform FXTransform;
    ParticleSystem.MainModule main;

    private void Start()
    {
        main = popFX.main;
    }

    public void PlayPopEffect(List<Vector3> popPositions, PieceType pieceType)
    {
        //Debug.Log("FX color: " + pieceType.ToString());
        //Debug.Log("Get color: " + GetColor(pieceType).ToString());

        main.startColor = GetColor(pieceType);

        StartCoroutine(MoveFXThroughPieces(popPositions));
    }

    IEnumerator MoveFXThroughPieces(List<Vector3> popPositions)
    {
        popFX.Stop();

        foreach (Vector3 pos in popPositions)
        {
            FXTransform.localPosition = new Vector3(pos.x, pos.y, -0.5f);

            popFX.Play();

            yield return new WaitForEndOfFrame();
        }

        FXTransform.localPosition = Vector3.down * 100;
    }

    Color GetColor(PieceType type)
    {
        switch (type)
        {
            case PieceType.Red:
                return Color.red;
            case PieceType.Green:
                return Color.green;
            case PieceType.Blue:
                return Color.blue;
            case PieceType.Yellow:
                return Color.yellow;
            case PieceType.Bomb:
                return Color.black;
            case PieceType.Disco:
                return Color.magenta;
            default:
                return Color.white;
        }
    }
}
