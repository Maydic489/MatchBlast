using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
            //CombineAdjacentMatchGroup();
        }
        else
        {
            Debug.Log("no match group found");
        }
    }

    List<PieceObject> FindThisPieceGroup(PieceObject selectedPiece)
    {
        int i = 0;

        foreach (List<PieceObject> thisGroup in groupPieces)
        {
            if(thisGroup.Contains(selectedPiece))
            {
                Debug.Log("destroying group " + i + " with "+thisGroup.Count);

                return thisGroup;
            }
            i++;
        }

        return null;
    }

    void CheckMatches()
    {
        groupPieces.Clear();

        List<PieceObject> matchedPieces = new List<PieceObject>();

        //check horizontal matches
        for (int i = 0; i < _tableManager.TableSize.y; i++)
        {
            //if on a new row but still have match pieces from previous row, add and clear it firest
            if(matchedPieces.Count >= 2)
            {
                //create new group to contain matched pieces data, before clear matchedPieces
                List<PieceObject> newGroup = new List<PieceObject>(matchedPieces);

                groupPieces.Add(newGroup);

                matchedPieces.Clear();
            }

            for (int j = 0; j < _tableManager.TableSize.x; j++)
            {
                if (j < _tableManager.TableSize.x - 1)
                {
                    if (_tableManager.TableSlotArray[i][j].havePiece && _tableManager.TableSlotArray[i][j + 1].havePiece /*&& _tableManager.TableSlotArray[i][j + 2].havePiece*/)
                    {
                        if (_tableManager.TableSlotArray[i][j].pieceObject.pieceData.pieceType == _tableManager.TableSlotArray[i][j + 1].pieceObject.pieceData.pieceType)
                        {
                            matchedPieces.Add(_tableManager.TableSlotArray[i][j].pieceObject);
                            matchedPieces.Add(_tableManager.TableSlotArray[i][j + 1].pieceObject);
                        }
                        else if (matchedPieces.Count >= 2)
                        {
                            //create new group to contain matched pieces data, before clear matchedPieces
                            List<PieceObject> newGroup = new List<PieceObject>(matchedPieces);

                            groupPieces.Add(newGroup);

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
            //if on a new row but still have match pieces from previous row, add and clear it firest
            if (matchedPieces.Count >= 2)
            {
                //create new group to contain matched pieces data, before clear matchedPieces
                List<PieceObject> newGroup = new List<PieceObject>(matchedPieces);

                groupPieces.Add(newGroup);

                matchedPieces.Clear();
            }

            for (int j = 0; j < _tableManager.TableSize.y; j++)
            {
                if (j < _tableManager.TableSize.y - 1)
                {
                    if (_tableManager.TableSlotArray[j][i].havePiece && _tableManager.TableSlotArray[j + 1][i].havePiece /*&& _tableManager.TableSlotArray[j + 2][i].havePiece*/)
                    {
                        if (_tableManager.TableSlotArray[j][i].pieceObject.pieceData.pieceType == _tableManager.TableSlotArray[j + 1][i].pieceObject.pieceData.pieceType)
                        {
                            matchedPieces.Add(_tableManager.TableSlotArray[j][i].pieceObject);
                            matchedPieces.Add(_tableManager.TableSlotArray[j + 1][i].pieceObject);

                        }
                        else if(matchedPieces.Count >= 2)
                        {
                            List<PieceObject> newGroup = new List<PieceObject>(matchedPieces);

                            groupPieces.Add(newGroup);

                            Debug.Log("found match at " + matchedPieces[0].pieceData.slotIndex + " and " + matchedPieces[matchedPieces.Count - 1].pieceData.slotIndex);

                            matchedPieces.Clear();
                        }
                    }
                }
            }
        }

        if(groupPieces.Count > 0)
        {
            Debug.Log("group pieces count "+groupPieces.Count);
            CombineAdjacentMatchGroup();
        }
        else
        {
            Debug.Log("no match found");
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
                        groupPieces[i].AddRange(groupPieces[j]);

                        if(i == 3)
                        {
                            Debug.Log("group 3 combine group "+j);
                        }

                        groupPieces.RemoveAt(j);

                        //compensate for removed a group, have to check same index again that now have new group
                        j--;
                    }
                }
            }
        }

        Debug.Log("have final " + groupPieces.Count + " group pieces");
    }

    bool AreGroupsAdjacent(List<PieceObject> group1, List<PieceObject> group2, int g1, int g2)
    {
        foreach (PieceObject piece1 in group1)
        {
            foreach (PieceObject piece2 in group2)
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

    bool ArePiecesAdjacent(PieceObject piece1, PieceObject piece2, int g1, int g2)
    {
        Vector2 pos1 = piece1.pieceData.slotIndex;
        Vector2 pos2 = piece2.pieceData.slotIndex;

        //check horizontally
        if (pos1.y == pos2.y && Math.Abs(pos1.x - pos2.x) <= 1)
        {
            //if (pos1 == new Vector2(1, 2))
            //{
            //    Debug.Log("piece 1 at " + pos1 + " and piece 2 at " + pos2);
            //}

            if(g1 == 3 && g2 == 4)
                Debug.Log("piece 1 at " + pos1 + " and piece 2 at " + pos2);

            return true;
        }

        //check vertically
        if (pos1.x == pos2.x && Math.Abs(pos1.y - pos2.y) <= 1)
        {
            //if (pos1 == new Vector2(1, 2))
            //{
            //    Debug.Log("piece 1 at " + pos1 + " and piece 2 at " + pos2);
            //}

            if (g1 == 3 && g2 == 4)
                Debug.Log("piece 1 at " + pos1 + " and piece 2 at " + pos2);

            return true;
        }

        return false;
    }

}
