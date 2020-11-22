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
    [SerializeField] private ChessUIController UIManager;

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

    internal bool IsGameInProgress()
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

    public void RestartGame()
    {
        DestroyPieces();
        board.OnGameRestarted();
        whitePlayer.OnGameRestarted();
        blackPlayer.OnGameRestarted();
        StartNewGame();
    }

    private void DestroyPieces()
    {
        whitePlayer.activePieces.ForEach(p => Destroy(p.gameObject));
        blackPlayer.activePieces.ForEach(p => Destroy(p.gameObject));
    }

    private void StartNewGame()
    {
        SetGameState(GameState.Init);
        UIManager.HideUI();
        InitializeGame(startingBoardLayout);
        activePlayer = whitePlayer;
        GeneratePlayersValidMoves(activePlayer);
        SetGameState(GameState.Play);
    }

    private void SetGameState(GameState state)
    {
        this.state = state;
    }

    private void InitializeGame(BoardLayout layout)
    {
        board.SetDependencies(this);
        CreatePiecesFromLayout(layout);
    }



    private void CreatePiecesFromLayout(BoardLayout layout)
    {
        for (int i = 0; i < layout.GetPiecesCount(); i++)
        {
            Vector2Int squareCoordinates = layout.GetSquareCoordsAtIndex(i);
            TeamColor color = layout.GetSquareTeamColorAtIndex(i);
            string typeName = layout.GetSquarePieceNameAtIndex(i);
            Type type = Type.GetType(typeName);
            CreatePieceAndInitialize(squareCoordinates, color, type);
        }
    }

    public void CreatePieceAndInitialize(Vector2Int squareCoordinates, TeamColor color, Type type)
    {
        Piece newPiece = CreatPieceOfTypeForTeam(type, color);
        newPiece.SetData(squareCoordinates, color, board);
        board.SetPieceOnSquare(squareCoordinates, newPiece);
        AssignPieceToPlyer(color, newPiece);
    }

    public void AssignPieceToPlyer(TeamColor color, Piece newPiece)
    {
        ChessPlayer currentPlayer = (color == TeamColor.White) ? whitePlayer : blackPlayer;
        currentPlayer.AddPiece(newPiece);
    }


    internal Piece CreatPieceOfTypeForTeam(Type type, TeamColor team)
    {
        return pieceCreator.CreatePiece(type, team);
    }

    internal void RemoveMovesEnablingAttakOnPieceOfType<T>(Piece piece) where T : Piece
    {
        activePlayer.RemoveMovesEnablingAttakOnPieceOfType<T>(GetOpponentToPlayer(activePlayer), piece);
    }

    internal bool IsTeamTurnActive(TeamColor team)
    {
        return activePlayer.team == team;
    }

	public void EndTurn()
    {
        GeneratePlayersValidMoves(activePlayer);
        GeneratePlayersValidMoves(GetOpponentToPlayer(activePlayer));
        if (CheckIfGameIsFinished())
        {
            EndGame();
        }
        else
        {
            ChangeActiveTeam();
        }
    }

    private void EndGame()
    {
        SetGameState(GameState.Finished);
        Debug.LogError("GameEnded");
        UIManager.OnGameFinished(activePlayer.team.ToString());
    }

    private void GeneratePlayersValidMoves(ChessPlayer player)
    {
        player.GenerateAllPossibleMoves();
    }

    private bool CheckIfGameIsFinished()
    {        
        Piece[] kingAttackingPieces = activePlayer.GetPieceAtackingOppositePiceOfType<King>();
        if (kingAttackingPieces.Length > 0)
        {
            ChessPlayer oppositePlayer = GetOpponentToPlayer(activePlayer);
            Piece attackedKing = oppositePlayer.GetPiecesOfType<King>().FirstOrDefault();
            oppositePlayer.RemoveMovesEnablingAttakOnPieceOfType<King>(activePlayer, attackedKing);

            int avaliableKingMoves = attackedKing.avaliableMoves.Count;
            if(avaliableKingMoves == 0)
            {
                bool canCoverKing = oppositePlayer.CanHidePieceFromAttack<King>(activePlayer);
                Debug.LogError(string.Format("Can cover: {0}, king moves: {1}", canCoverKing, avaliableKingMoves));
                if (!canCoverKing)
                    return true;
            }         
        }
        return false;
    }

    private void ChangeActiveTeam()
    {
        if (activePlayer == whitePlayer)
            activePlayer = blackPlayer;
        else
            activePlayer = whitePlayer;
    }

    internal void OnPieceRemoved(Piece piece)
    {
        ChessPlayer pieceOwner = (piece.team == TeamColor.White) ? whitePlayer : blackPlayer;
        pieceOwner.RemovePiece(piece);
    }

    private ChessPlayer GetOpponentToPlayer(ChessPlayer player)
    {
        return player == whitePlayer ? blackPlayer : whitePlayer;
    }
}