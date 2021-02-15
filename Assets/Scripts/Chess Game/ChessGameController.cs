
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms;

[RequireComponent(typeof(PiecesCreator))]
public abstract class ChessGameController : MonoBehaviour
{
    protected const byte SET_GAME_STATE_EVENT_CODE = 1;

    [SerializeField] private BoardLayout startingBoardLayout;


    private ChessUIManager UIManager;
    private CameraSetup cameraSetup;
    private Board board;
    private PiecesCreator pieceCreator;
    protected ChessPlayer whitePlayer;
    protected ChessPlayer blackPlayer;
    protected ChessPlayer activePlayer;
    

    protected GameState state;

    private void Awake()
    {
        pieceCreator = GetComponent<PiecesCreator>();
    }

    internal void SetDependencies(CameraSetup cameraSetup, ChessUIManager UIManager, Board board)
    {
        this.cameraSetup = cameraSetup;
        this.UIManager = UIManager;
        this.board = board;
    }

    public void InitializeGame()
    {     
        CreatePlayers();       
    }


    private void CreatePlayers()
    {
        whitePlayer = new ChessPlayer(TeamColor.White, board);
        blackPlayer = new ChessPlayer(TeamColor.Black, board);
    }


    public void StartNewGame()
    {
        UIManager.OnGameStarted();
        SetGameState(GameState.Init);       
        CreatePiecesFromLayout(startingBoardLayout);
        activePlayer = whitePlayer;
        GenerateAllPossiblePlayerMoves(activePlayer);
        TryToStartThisGame();
     
    }

    protected abstract void SetGameState(GameState state);
    public abstract void TryToStartThisGame();
    public abstract bool CanPerformMove();

    internal bool IsGameInProgress()
    {
        return state == GameState.Play;
    }

    

    private void CreatePiecesFromLayout(BoardLayout layout)
    {
        for (int i = 0; i < layout.GetPiecesCount(); i++)
        {
            Vector2Int squareCoords = layout.GetSquareCoordsAtIndex(i);
            TeamColor team = layout.GetSquareTeamColorAtIndex(i);
            string typeName = layout.GetSquarePieceNameAtIndex(i);

            Type type = Type.GetType(typeName);
            CreatePieceAndInitialize(squareCoords, team, type);
        }
    }

   

    public void CreatePieceAndInitialize(Vector2Int squareCoords, TeamColor team, Type type)
    {
        Piece newPiece = pieceCreator.CreatePiece(type).GetComponent<Piece>();
        newPiece.SetData(squareCoords, team, board);

        Material teamMaterial = pieceCreator.GetTeamMaterial(team);
        newPiece.SetMaterial(teamMaterial);

        board.SetPieceOnBoard(squareCoords, newPiece);

        ChessPlayer currentPlayer = team == TeamColor.White ? whitePlayer : blackPlayer;
        currentPlayer.AddPiece(newPiece);
    }

	internal void SetupCamera(TeamColor team)
	{
        cameraSetup.SetupCamera(team);
	}

    

    private void GenerateAllPossiblePlayerMoves(ChessPlayer player)
    {
        player.GenerateAllPossibleMoves();
    }

    public bool IsTeamTurnActive(TeamColor team)
    {
        return activePlayer.team == team;
    }

    public void EndTurn()
    {
        GenerateAllPossiblePlayerMoves(activePlayer);
        GenerateAllPossiblePlayerMoves(GetOpponentToPlayer(activePlayer));
        if (CheckIfGameIsFinished())
        {
            EndGame();
        }
        else
        {
            ChangeActiveTeam();
        }
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
            if (avaliableKingMoves == 0)
            {
                bool canCoverKing = oppositePlayer.CanHidePieceFromAttack<King>(activePlayer);
                if (!canCoverKing)
                    return true;
            }
        }
        return false;
    }

    private void EndGame()
    {
        SetGameState(GameState.Finished);
        UIManager.OnGameFinished(activePlayer.team.ToString());
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

    private void ChangeActiveTeam()
    {
        activePlayer = activePlayer == whitePlayer ? blackPlayer : whitePlayer;
    }

    private ChessPlayer GetOpponentToPlayer(ChessPlayer player)
    {
        return player == whitePlayer ? blackPlayer : whitePlayer;
    }

    internal void OnPieceRemoved(Piece piece)
    {
        ChessPlayer pieceOwner = (piece.team == TeamColor.White) ? whitePlayer : blackPlayer;
        pieceOwner.RemovePiece(piece);
    }

    internal void RemoveMovesEnablingAttakOnPieceOfType<T>(Piece piece) where T : Piece
    {
        activePlayer.RemoveMovesEnablingAttakOnPieceOfType<T>(GetOpponentToPlayer(activePlayer), piece);
    }

   
}

