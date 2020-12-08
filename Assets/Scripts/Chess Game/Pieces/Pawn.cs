using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    public override List<Vector2Int> SelectAvaliableSquares()
    {
        avaliableMoves.Clear();

        Vector2Int direction = team == TeamColor.White ? Vector2Int.up : Vector2Int.down;
        float range = hasMoved ? 1 : 2;
        for (int i = 1; i <= range; i++)
        {
            Vector2Int nextCoords = occupiedSquare + direction * i;
            Piece piece = board.GetPieceOnSquare(nextCoords);
            if (!board.CheckIfCoordinatesAreOnBoard(nextCoords))
                break;
            if (piece == null)
                TryToAddMove(nextCoords);
            else
                break;
        }

        Vector2Int[] takeDirections = new Vector2Int[] { new Vector2Int(1, direction.y), new Vector2Int(-1, direction.y) };
        for (int i = 0; i < takeDirections.Length; i++)
        {
            Vector2Int nextCoords = occupiedSquare + takeDirections[i];
            Piece piece = board.GetPieceOnSquare(nextCoords);
            if (!board.CheckIfCoordinatesAreOnBoard(nextCoords))
                continue;
            if (piece != null && !piece.IsFromSameTeam(this))
            {
                TryToAddMove(nextCoords);
            }
        }
        return avaliableMoves;
    }

    public override void MovePiece(Vector2Int coords)
    {
        base.MovePiece(coords);
        CheckPromotion();
    }

    private void CheckPromotion()
    {
        int endOfBoardYCoord = team == TeamColor.White ? Board.BOARD_SIZE - 1 : 0;
        if (occupiedSquare.y == endOfBoardYCoord)
        {
            board.PromotePiece(this);
        }
    }
}
