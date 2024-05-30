using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MatchingManager : MonoBehaviour
{
    [SerializeField] TableManager _tableManager;

    List<List<BasePiece>> groupPieces = new List<List<BasePiece>>();

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

    public List<BasePiece> SelectMatchGroup(BasePiece selectedPiece)
    {
        var selectedGroup = FindThisPieceGroup(selectedPiece);

        if(selectedGroup != null)
        {
            groupPieces.Remove(selectedGroup);
            return selectedGroup;
        }
        else
        {
            Debug.Log("no match group found");
            selectedPiece.ShakingPiece();
            return null;
        }
    }

    List<BasePiece> FindThisPieceGroup(BasePiece selectedPiece)
    {
        int i = 0;

        foreach (List<BasePiece> thisGroup in groupPieces)
        {
            if(thisGroup.Exists(x => x.pieceData.slotIndex == selectedPiece.pieceData.slotIndex))
            {
                //Debug.Log("destroying group " + i + " with "+thisGroup.Count);

                return thisGroup;
            }
            i++;
        }

        return null;
    }

    void CheckMatches()
    {
        groupPieces.Clear();

        List<BasePiece> matchedPieces = new List<BasePiece>();

        //check horizontal matches
        for (int i = 0; i < _tableManager.TableSize.y; i++)
        {
            //if on a new row but still have match pieces from previous row, add and clear it firest
            if(matchedPieces.Count >= 2)
            {
                AddMatchedPiecesToGroup(matchedPieces);
            }

            for (int j = 0; j < _tableManager.TableSize.x; j++)
            {
                //_tableManager.TableSlotArray[i][j].pieceObject.DisableHighlight();

                if (j < _tableManager.TableSize.x - 1)
                {
                    if (_tableManager.TableSlotArray[i][j].havePiece && _tableManager.TableSlotArray[i][j + 1].havePiece)
                    {
                        if (_tableManager.TableSlotArray[i][j].pieceObject.pieceData.pieceType == _tableManager.TableSlotArray[i][j + 1].pieceObject.pieceData.pieceType
                            && !IsThisASpecialPiece(_tableManager.TableSlotArray[i][j].pieceObject.pieceData))
                        {
                            if(matchedPieces.Count == 0)
                                matchedPieces.Add(_tableManager.TableSlotArray[i][j].pieceObject);

                            matchedPieces.Add(_tableManager.TableSlotArray[i][j + 1].pieceObject);
                        }
                        else if (matchedPieces.Count >= 2)
                        {
                            AddMatchedPiecesToGroup(matchedPieces);
                        }
                    }
                }
            }
        }

        //when reach the end of the table, add the last matched pieces to group
        if (matchedPieces.Count >= 2)
        {
            AddMatchedPiecesToGroup(matchedPieces);
        }

        matchedPieces.Clear();

        //check vertical matches
        for (int i = 0; i < _tableManager.TableSize.x; i++)
        {
            //if on a new row but still have match pieces from previous row, add and clear it firest
            if (matchedPieces.Count >= 2)
            {
                AddMatchedPiecesToGroup(matchedPieces);
            }

            for (int j = 0; j < _tableManager.TableSize.y; j++)
            {
                if (j < _tableManager.TableSize.y - 1)
                {
                    if (_tableManager.TableSlotArray[j][i].havePiece && _tableManager.TableSlotArray[j + 1][i].havePiece)
                    {
                        if (_tableManager.TableSlotArray[j][i].pieceObject.pieceData.pieceType == _tableManager.TableSlotArray[j + 1][i].pieceObject.pieceData.pieceType)
                        {
                            matchedPieces.Add(_tableManager.TableSlotArray[j][i].pieceObject);
                            matchedPieces.Add(_tableManager.TableSlotArray[j + 1][i].pieceObject);

                        }
                        else if(matchedPieces.Count >= 2)
                        {
                            AddMatchedPiecesToGroup(matchedPieces);
                        }
                    }
                }
            }
        }

        //when reach the end of the table, add the last matched pieces to group
        if (matchedPieces.Count >= 2)
        {
            AddMatchedPiecesToGroup(matchedPieces);
        }

        if (groupPieces.Count > 0)
        {
            //Debug.Log("group pieces count "+groupPieces.Count);

            //loop Combine() until no more group can be combined
            int oldGroupCount = 0;
            do
            {
                //record old count number
                oldGroupCount = groupPieces.Count;

                //combine adjacent group
                CombineAdjacentMatchGroup();
            }
            while (groupPieces.Count > 1 && oldGroupCount != groupPieces.Count);

            //TODO: if there're no match at all, respawn the table
        }
        else
        {
            Debug.Log("no match found");
        }

        void AddMatchedPiecesToGroup(List<BasePiece> matchedPieces)
        {
            //create new group to contain matched pieces data, before clear matchedPieces
            List<BasePiece> newGroup = new List<BasePiece>(matchedPieces);

            groupPieces.Add(newGroup);

            matchedPieces.Clear();
        }
    }

    void CombineAdjacentMatchGroup()
    {
        for (int i = 0; i < groupPieces.Count; i++)
        {
            for (int j = i + 1; j < groupPieces.Count; j++)
            {
                if (groupPieces[i][0].pieceData.pieceType == groupPieces[j][0].pieceData.pieceType)
                {
                    //check if group adjacent to eachother (by checking its piece by piece)
                    if (AreGroupsAdjacent(groupPieces[i], groupPieces[j],i,j))
                    {
                        CombineGroupWithoutDuplicatePiece(groupPieces[i], groupPieces[j]);

                        groupPieces.RemoveAt(j);

                        //compensate for removed a group, have to check same index again that now have new group
                        j--;

                        StartCoroutine(SetForSpecialPiece(groupPieces[i]));
                    }
                }
            }
        }

        //Debug.Log("have final " + groupPieces.Count + " group pieces");
    }

    bool AreGroupsAdjacent(List<BasePiece> group1, List<BasePiece> group2, int g1, int g2)
    {
        foreach (BasePiece piece1 in group1)
        {
            foreach (BasePiece piece2 in group2)
            {
                //Check if piece1 and piece2 are adjacent in the table
                if (ArePiecesAdjacent(piece1, piece2,g1,g2))
                {
                    return true;
                }
            }
        }

        return false;
    }

    bool ArePiecesAdjacent(BasePiece piece1, BasePiece piece2, int g1, int g2)
    {
        Vector2 pos1 = piece1.pieceData.slotIndex;
        Vector2 pos2 = piece2.pieceData.slotIndex;
        //check horizontally
        if (pos1.y == pos2.y && Math.Abs(pos1.x - pos2.x) <= 1)
        {
            return true;
        }

        //check vertically
        if (pos1.x == pos2.x && Math.Abs(pos1.y - pos2.y) <= 1)
        {
            return true;
        }

        return false;
    }

    void CombineGroupWithoutDuplicatePiece(List<BasePiece> group1, List<BasePiece> group2)
    {
        foreach (BasePiece piece2 in group2)
        {
            bool isDuplicate = false;
            foreach (BasePiece piece1 in group1)
            {
                if (piece1.pieceData.slotIndex == piece2.pieceData.slotIndex)
                {
                    isDuplicate = true;
                    break;
                }
            }

            if (!isDuplicate)
            {
                group1.Add(piece2);
            }
        }
    }

    IEnumerator SetForSpecialPiece(List<BasePiece> pieceObjects)
    {
        yield return new WaitUntil(() => TableManager.instance.isReadyToTouch);

        TableManager.instance.ResetPiecesHighLight();

        yield return new WaitForEndOfFrame();

        if (CheckForSpecialPiece(pieceObjects) == PieceType.Disco)
        {
            foreach (BasePiece piece in pieceObjects)
            {
                piece.HighlightPotentailBonusPiece(PieceType.Disco);
            }
        }
        else if(CheckForSpecialPiece(pieceObjects) == PieceType.Bomb)
        {
            foreach (BasePiece piece in pieceObjects)
            {
                piece.HighlightPotentailBonusPiece(PieceType.Bomb);
            }
        }
        else
        {
            foreach (BasePiece piece in pieceObjects)
            {
                //piece.DisableHighlight();
                piece.SetPieceColor(false, piece.pieceData.pieceType);
            }
        }
    }

    public PieceType CheckForSpecialPiece(List<BasePiece> pieceObjects)
    {
        if (pieceObjects.Count >= 10)
        {
            return PieceType.Disco;
        }
        else if (pieceObjects.Count >= 6)
        {
            return PieceType.Bomb;
        }
        else
        {
            return PieceType.Red;
        }
    }

    public bool IsThisASpecialPiece(PieceData piece)
    {
        if (piece.pieceType == PieceType.Bomb || piece.pieceType == PieceType.Disco
            || piece.pieceType == PieceType.Obstacle || piece.pieceType == PieceType.ObstacleBig)
        {
            return true;
        }

        return false;
    }
}
