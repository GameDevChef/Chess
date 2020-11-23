using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PiecesCreator))]
public class ChessGameController : MonoBehaviour
{
    private enum GameState
    {
        Init, Play, Finished
    }

    [SerializeField] private BoardLayout startingBoardLayout;
    [SerializeField] private Board board;

    private GameState state;
    private ChessPlayer whitePlayer;
    private ChessPlayer blackPlayer;
    private ChessPlayer activePlayer;
    private PiecesCreator pieceCreator;

    private void Awake()
    {
        SetDependencies();
        CreatePlayers();
    }

    private void SetDependencies()
    {
        pieceCreator = GetComponent<PiecesCreator>();
    }

    public bool IsGameInProgress()
    {
        return state == GameState.Play;
    }

    private void CreatePlayers()
    {
        whitePlayer = new ChessPlayer(TeamColor.White, board);
        blackPlayer = new ChessPlayer(TeamColor.Black, board);
    }

    private void Start()
    {
        StartNewGame();
    }


    private void StartNewGame()
    {
        SetGameState(GameState.Init);
        board.SetDependencies(this);
        CreatePiecesFromLayout(startingBoardLayout);
        activePlayer = whitePlayer;
        GeneratePlayersValidMoves(activePlayer);
        SetGameState(GameState.Play);
    }

    private void SetGameState(GameState state)
    {
        this.state = state;
    }


    private void CreatePiecesFromLayout(BoardLayout layout)
    {
        for (int i = 0; i < layout.GetPiecesCount(); i++)
        {
            Vector2Int squareCoordinates = layout.GetSquareCoordsAtIndex(i);
            TeamColor team = layout.GetSquareTeamColorAtIndex(i);
            string typeName = layout.GetSquarePieceNameAtIndex(i);
            Type type = Type.GetType(typeName);
            CreatePieceAndInitialize(squareCoordinates, team, type);
        }
    }

    public void CreatePieceAndInitialize(Vector2Int squareCoordinates, TeamColor team, Type type)
    {
        Piece newPiece = pieceCreator.CreatePiece(type).GetComponent<Piece>();
        newPiece.SetData(squareCoordinates, team, board);

        Material teamMaterial = pieceCreator.GetTeamMaterial(team);
        newPiece.SetMaterial(teamMaterial);

        board.SetPieceOnSquare(squareCoordinates, newPiece);

        ChessPlayer currentPlayer = (team == TeamColor.White) ? whitePlayer : blackPlayer;
        currentPlayer.AddPiece(newPiece);
    }

    public bool IsTeamTurnActive(TeamColor team)
    {
        return activePlayer.team == team;
    }

	public void EndTurn()
    {
        GeneratePlayersValidMoves(activePlayer);
        GeneratePlayersValidMoves(GetOpponentToPlayer(activePlayer));
        ChangeActiveTeam();       
    }

    private void GeneratePlayersValidMoves(ChessPlayer player)
    {
        player.GenerateAllPossibleMoves();
    }

    private void ChangeActiveTeam()
    {
        if (activePlayer == whitePlayer)
            activePlayer = blackPlayer;
        else
            activePlayer = whitePlayer;
    }

    public void OnPieceRemoved(Piece piece)
    {
        ChessPlayer pieceOwner = (piece.team == TeamColor.White) ? whitePlayer : blackPlayer;
        pieceOwner.RemovePiece(piece);
    }

    private ChessPlayer GetOpponentToPlayer(ChessPlayer player)
    {
        return player == whitePlayer ? blackPlayer : whitePlayer;
    }
}