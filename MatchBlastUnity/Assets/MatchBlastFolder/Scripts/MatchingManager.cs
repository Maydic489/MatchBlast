using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchingManager : MonoBehaviour
{
    [SerializeField] TableManager _tableManager;

    List<List<PieceObject>> groupPieces = new List<List<PieceObject>>();

    public static MatchingManager instance = null;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _tableManager.tableReadyEvent.AddListener(CheckMatches);
    }

    public void SelectMatchGroup(PieceObject selectedPiece)
    {
        var selectedGroup = FindThisPieceGroup(selectedPiece);

        if(selectedGroup != null)
        {
            foreach (PieceObject piece in selectedGroup)
            {
                piece.DestroyPiece();
            }

            groupPieces.Remove(selectedGroup);
            CombineAdjacentMatchGroup();
        }
        else
        {
            Debug.Log("no match group found");
        }
    }

    List<PieceObject> FindThisPieceGroup(PieceObject selectedPiece)
    {
        Debug.Log("find this piece group");

        foreach (List<PieceObject> thisGroup in groupPieces)
        {
            if(thisGroup.Contains(selectedPiece))
            {
                return thisGroup;
            }
        }

        return null;
    }

    void CheckMatches()
    {
        Debug.Log("check matches");

        List<PieceObject> matchedPieces = new List<PieceObject>();

        //check horizontal matches
        for (int i = 0; i < _tableManager.TableSize.y; i++)
        {
            for (int j = 0; j < _tableManager.TableSize.x; j++)
            {
                if (j < _tableManager.TableSize.x - 1)
                {
                    if (_tableManager.TableSlotArray[i][j].havePiece && _tableManager.TableSlotArray[i][j + 1].havePiece /*&& _tableManager.TableSlotArray[i][j + 2].havePiece*/)
                    {
                        Debug.Log("check match at " + i + " " + j);
                        Debug.Log("piece type " + _tableManager.TableSlotArray[i][j].pieceObject.pieceData.pieceType);

                        if (_tableManager.TableSlotArray[i][j].pieceObject.pieceData.pieceType == _tableManager.TableSlotArray[i][j + 1].pieceObject.pieceData.pieceType)
                        {
                            Debug.Log("horizontal match");
                            matchedPieces.Add(_tableManager.TableSlotArray[i][j].pieceObject);
                            matchedPieces.Add(_tableManager.TableSlotArray[i][j + 1].pieceObject);

                            //_tableManager.TableSlotArray[i][j].pieceObject.gameObject.SetActive(false);
                            //_tableManager.TableSlotArray[i][j + 1].pieceObject.gameObject.SetActive(false);
                        }
                        else if(matchedPieces.Count >= 2)
                        {
                            groupPieces.Add(matchedPieces);
                            Debug.Log("row "+i+" column "+j+" match found "+groupPieces.Count);
                            matchedPieces.Clear();
                        }
                    }
                }
            }
        }

        matchedPieces.Clear();

        //check vertical matches
        for (int i = 0; i < _tableManager.TableSize.x; i++)
        {
            for (int j = 0; j < _tableManager.TableSize.y; j++)
            {
                if (j < _tableManager.TableSize.y - 1)
                {
                    if (_tableManager.TableSlotArray[j][i].havePiece && _tableManager.TableSlotArray[j + 1][i].havePiece /*&& _tableManager.TableSlotArray[j + 2][i].havePiece*/)
                    {
                        if (_tableManager.TableSlotArray[j][i].pieceObject.pieceData.pieceType == _tableManager.TableSlotArray[j + 1][i].pieceObject.pieceData.pieceType)
                        {
                            Debug.Log("vertical match");
                            matchedPieces.Add(_tableManager.TableSlotArray[j][i].pieceObject);
                            matchedPieces.Add(_tableManager.TableSlotArray[j + 1][i].pieceObject);

                            //_tableManager.TableSlotArray[j][i].pieceObject.gameObject.SetActive(false);
                            //_tableManager.TableSlotArray[j+1][i].pieceObject.gameObject.SetActive(false);
                        }
                        else if(matchedPieces.Count >= 2)
                        {
                            groupPieces.Add(matchedPieces);
                            Debug.Log("row "+j+" column "+i+" match found "+groupPieces.Count);
                            Debug.Log("matched pieces count "+matchedPieces.Count);
                            matchedPieces.Clear();
                        }
                    }
                }
            }
        }

        if(groupPieces.Count > 0)
        {
            Debug.Log("group pieces count "+groupPieces.Count);
            //CombineAdjacentMatchGroup();
        }
        else
        {
            Debug.Log("no match found");
        }
    }

    //combine match group with the same type that adjacent to eachother
    void CombineAdjacentMatchGroup()
    {
        for(int i = 0; i < groupPieces.Count; i++)
        {
            for(int j = i+1; j < groupPieces.Count; j++)
            {
                Debug.Log("check combine " + i + " " + j);

                if (groupPieces[i][0].pieceData.pieceType == groupPieces[j][0].pieceData.pieceType)
                {
                    groupPieces[i].AddRange(groupPieces[j]);
                    groupPieces.RemoveAt(j);
                }
            }
        }
    }
}
