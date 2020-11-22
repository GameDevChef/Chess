using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PiecesCreator : MonoBehaviour
{
    [SerializeField] private Piece[] piecesPrefabs;
    [SerializeField] private Material blackMaterial;
    [SerializeField] private Material whiteMaterial;
    private Dictionary<string, Piece> nameToPieceDict = new Dictionary<string, Piece>();

    private void Awake()
    {
        foreach (var piece in piecesPrefabs)
        {
            nameToPieceDict.Add(piece.GetType().ToString(), piece);
        }
    }

    public Piece CreatePiece(Type type, TeamColor teamColor)
    {
        Piece prefab = nameToPieceDict[type.ToString()];
        if (prefab)
        {
            Piece newPiece = Instantiate(prefab);
            Material selectedMaterial = teamColor == TeamColor.White ? whiteMaterial : blackMaterial;
            newPiece.SetMaterial(selectedMaterial);
            return newPiece;
        }
        return null;
    }
}
