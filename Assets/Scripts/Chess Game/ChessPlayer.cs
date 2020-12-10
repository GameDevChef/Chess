using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChessPlayer
{
	public TeamColor team { get; set; }
	public Board board { get; set; }
	public List<Piece> activePieces { get; private set; }

	public ChessPlayer(TeamColor team, Board board)
	{
		activePieces = new List<Piece>();
		this.board = board;
		this.team = team;
	}

	public void AddPiece(Piece piece)
	{
		if (!activePieces.Contains(piece))
			activePieces.Add(piece);
	}

	public void RemovePiece(Piece piece)
	{
		if (activePieces.Contains(piece))
			activePieces.Remove(piece);
	}

	public void GenerateAllPossibleMoves()
	{
		foreach (var piece in activePieces)
		{
			if(board.HasPiece(piece))
				piece.SelectAvaliableSquares();
		}
	}

	public Piece[] GetPieceAtackingOppositePiceOfType<T>() where T : Piece
	{
		return activePieces.Where(p => p.IsAttackingPieceOfType<T>()).ToArray();
	}

	public Piece[] GetPiecesOfType<T>() where T : Piece
	{
		return activePieces.Where(p => p is T).ToArray();
	}

	public void RemoveMovesEnablingAttakOnPieceOfType<T>(ChessPlayer opponent, Piece selectedPiece) where T : Piece
	{
		List<Vector2Int> coordsToRemove = new List<Vector2Int>();

		coordsToRemove.Clear();
		foreach (var coords in selectedPiece.avaliableMoves)
		{
			Piece pieceOnCoords = board.GetPieceOnSquare(coords);
			board.UpdateBoardOnPieceMove(coords, selectedPiece.occupiedSquare, selectedPiece, null);
			opponent.GenerateAllPossibleMoves();
			if (opponent.CheckIfIsAttacigPiece<T>())
				coordsToRemove.Add(coords);
			board.UpdateBoardOnPieceMove(selectedPiece.occupiedSquare, coords, selectedPiece, pieceOnCoords);
		}
		foreach (var coords in coordsToRemove)
		{
			selectedPiece.avaliableMoves.Remove(coords);
		}

	}

	internal bool CheckIfIsAttacigPiece<T>() where T : Piece
	{
		foreach (var piece in activePieces)
		{
			if (board.HasPiece(piece) && piece.IsAttackingPieceOfType<T>())
				return true;
		}
		return false;
	}

	public bool CanHidePieceFromAttack<T>(ChessPlayer opponent) where T : Piece
	{
		foreach (var piece in activePieces)
		{
			foreach (var coords in piece.avaliableMoves)
			{
				Piece pieceOnCoords = board.GetPieceOnSquare(coords);
				board.UpdateBoardOnPieceMove(coords, piece.occupiedSquare, piece, null);
				opponent.GenerateAllPossibleMoves();
				if (!opponent.CheckIfIsAttacigPiece<T>())
				{
					board.UpdateBoardOnPieceMove(piece.occupiedSquare, coords, piece, pieceOnCoords);
					return true;
				}
				board.UpdateBoardOnPieceMove(piece.occupiedSquare, coords, piece, pieceOnCoords);
			}
		}
		return false;
	}

	internal void OnGameRestarted()
	{
		activePieces.Clear();
	}



}