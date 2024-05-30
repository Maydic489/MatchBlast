using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoBehaviour
{
    [SerializeField] ParticleSystem popFX;
    [SerializeField] Transform FXTransform;
    [SerializeField] TrailRenderer discoTrail;
    [SerializeField] ParticleSystem discoFX;
    [SerializeField] Transform discoFXTransform;
    ParticleSystem.MainModule mainPopFX;
    ParticleSystem.MainModule mainDiscoFX;

    private void Start()
    {
        mainPopFX = popFX.main;
        mainDiscoFX = discoFX.main;
    }

    public void PlayPopEffect(List<Vector3> popPositions, PieceType pieceType)
    {
        //Debug.Log("FX color: " + pieceType.ToString());
        //Debug.Log("Get color: " + GetColor(pieceType).ToString());

        mainPopFX.startColor = GetColor(pieceType);

        StartCoroutine(MoveFXThroughPieces(popPositions));
    }

    IEnumerator MoveFXThroughPieces(List<Vector3> popPositions)
    {
        popFX.Stop();
        //popFX.Clear();

        foreach (Vector3 pos in popPositions)
        {
            FXTransform.localPosition = new Vector3(pos.x, pos.y, -0.5f);

            popFX.Play();

            yield return new WaitForEndOfFrame();
        }

        FXTransform.localPosition = Vector3.down * 100;
    }

    public IEnumerator PlayTrailEffect(Vector3 startPos, Vector3 endPos, PieceType pieceType)
    {
        startPos = new Vector3(startPos.x, startPos.y, -0.5f);
        endPos = new Vector3(endPos.x, endPos.y, -0.5f);

        discoFXTransform.localPosition = startPos;

        discoFXTransform.DOLocalMove(endPos, 0.15f);

        discoTrail.Clear();
        discoTrail.startColor = GetColor(pieceType);
        discoTrail.emitting = true;

        discoFX.Clear();
        mainDiscoFX.startColor = GetColor(pieceType);
        discoFX.Play();

        yield return new WaitForSeconds(0.15f);

        PlayPopEffect(new List<Vector3> { endPos }, pieceType);

        discoTrail.Clear();
        discoTrail.emitting = false;

        discoFX.Clear();
        discoFX.Stop();
    }

    Color GetColor(PieceType type)
    {
        switch (type)
        {
            case PieceType.Red:
                return MatchBlastManager.instance.red;
            case PieceType.Green:
                return MatchBlastManager.instance.green;
            case PieceType.Blue:
                return MatchBlastManager.instance.blue;
            case PieceType.Yellow:
                return MatchBlastManager.instance.yellow;
            case PieceType.Bomb:
                return MatchBlastManager.instance.brown;
            case PieceType.Disco:
                return Color.magenta;
            default:
                return Color.white;
        }
    }
}
