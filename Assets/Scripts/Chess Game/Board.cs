using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Board : MonoBehaviour
{
	public const int BOARD_SIZE = 8;

	[SerializeField] private Transform bottomLeftSquareTransform;
	[SerializeField] private float squareSize;

	private ChessGameController chessController;
	private Piece selectedPiece;
	private SquareSelectorCreator squareSelector;
	private Piece[,] grid;

	private void Awake()
	{
		squareSelector = GetComponent<SquareSelectorCreator>();
		CreateGrid();
	}

	private void CreateGrid()
	{
		grid = new Piece[BOARD_SIZE, BOARD_SIZE];
	}

	public void SetDependencies(ChessGameController chessController)
	{
		this.chessController = chessController;
	}

	public void OnSquareSelected(Vector3 inputPosition)
	{
		if (!chessController.IsGameInProgress())
			return;
		Vector2Int coords = CalculateCoordsFromPosition(inputPosition);
		Piece piece = GetPieceOnSquare(coords);
		if (selectedPiece)
		{
			if (piece != null && selectedPiece == piece)
			{
				DeselectPiece();
			}
			else if (piece != null && selectedPiece != piece && chessController.IsTeamTurnActive(piece.team))
			{
				SelectPiece(piece);
			}
			else if (selectedPiece.CanMoveTo(coords))
			{
				OnSelectedPieceMoved(coords, selectedPiece);
			}
		}
		else
		{
			if (piece != null && chessController.IsTeamTurnActive(piece.team))
			{
				SelectPiece(piece);
			}
		}
	}

	private Vector2Int CalculateCoordsFromPosition(Vector3 inputPosition)
	{
		int x = Mathf.FloorToInt(inputPosition.x / squareSize) + BOARD_SIZE / 2;
		int y = Mathf.FloorToInt(inputPosition.z / squareSize) + BOARD_SIZE / 2;
		return new Vector2Int(x, y);
	}

	public Piece GetPieceOnSquare(Vector2Int coords)
	{
		if (CheckIfCoordinatesAreOnBoard(coords))
			return grid[coords.x, coords.y];
		return null;
	}

	private void DeselectPiece()
	{
		selectedPiece = null;
		squareSelector.ClearSelection();
	}

	private void SelectPiece(Piece piece)
	{
		selectedPiece = piece;
		List<Vector2Int> selection = selectedPiece.avaliableMoves;
		ShowSelectionSquares(selection);
	}

	private void OnSelectedPieceMoved(Vector2Int coords, Piece piece)
	{
		TryToTakeOppositePiece(coords);
		UpdateBoardOnPieceMove(coords, piece.occupiedSquare, piece, null);
		selectedPiece.MovePiece(coords);
		DeselectPiece();
		EndTurn();
	}

	public void SetPieceOnSquare(Vector2Int coords, Piece piece)
	{
		if (CheckIfCoordinatesAreOnBoard(coords))
		{
			grid[coords.x, coords.y] = piece;
		}
	}

	public bool HasPiece(Piece piece, TeamColor team)
	{
		for (int i = 0; i < BOARD_SIZE; i++)
		{
			for (int j = 0; j < BOARD_SIZE; j++)
			{
				if (grid[i, j] == piece)
					return true;
			}
		}
		return false;
	}

	public bool CheckIfCoordinatesAreOnBoard(Vector2Int coords)
	{
		if (coords.x < 0 || coords.y < 0 || coords.x >= BOARD_SIZE || coords.y >= BOARD_SIZE)
		{
			return false;
		}
		return true;
	}	

	public void PromotePiece(Piece piece)
	{
		TakePiece(piece);
		chessController.CreatePieceAndInitialize(piece.occupiedSquare, piece.team, typeof(Queen));
	}

	private void ShowSelectionSquares(List<Vector2Int> selection)
	{
		Dictionary<Vector3, bool> selectionPositions = new Dictionary<Vector3, bool>();
		for (int i = 0; i < selection.Count; i++)
		{
			Vector3 squarePosition = CalculatePositionFromCoords(selection[i]);
			bool isSquareFree = GetPieceOnSquare(selection[i]) == null;
			selectionPositions.Add(squarePosition, isSquareFree);
		}
		squareSelector.ShowSelection(selectionPositions);
	}

	public Vector3 CalculatePositionFromCoords(Vector2Int squareCoordinates)
	{
		return bottomLeftSquareTransform.position +new Vector3(squareCoordinates.x * squareSize, 0f, squareCoordinates.y * squareSize);
	}

	private void TryToTakeOppositePiece(Vector2Int coords)
	{
		Piece piece = GetPieceOnSquare(coords);
		if (piece && !selectedPiece.IsFromSameTeam(piece))
		{
			TakePiece(piece); 
		}
	}

	private void TakePiece(Piece piece)
	{
		if (piece)
		{
			grid[piece.occupiedSquare.x, piece.occupiedSquare.y] = null;
			chessController.OnPieceRemoved(piece);
			Destroy(piece.gameObject);
		}
	}

	private void EndTurn()
	{
		chessController.EndTurn();
	}

	public void UpdateBoardOnPieceMove(Vector2Int newCoords, Vector2Int oldCoords, Piece newPiece, Piece oldPiece)
	{
		grid[oldCoords.x, oldCoords.y] = oldPiece;
		grid[newCoords.x, newCoords.y] = newPiece;
	}
}