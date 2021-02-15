using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameInitializer : MonoBehaviour
{
    [Header("Game mode dependent objects")]

    [SerializeField] private SingleplayerChessGameController singleplayerControllerPrefab;
    [SerializeField] private MultiplayerChessGameController multiplayerControllerPrefab;
    [SerializeField] private MultiplayerBoard multiplayerBoardPrefab;
    [SerializeField] private SinglePlayerBoard singleplayerBoardPrefab;

    [Header("Scene references")]
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private CameraSetup cameraSetup;
    [SerializeField] private ChessUIManager uiManager;
    [SerializeField] private Transform boardAnchor;

    public void CreateMultiplayerBoard()
    {
        if (!networkManager.IsRoomFull())
            PhotonNetwork.Instantiate(multiplayerBoardPrefab.name, boardAnchor.position, boardAnchor.rotation);
    }

    public void CreateSinglePlayerBoard()
    {
        Instantiate(singleplayerBoardPrefab, boardAnchor);
    }

    public void InitializeMultiplayerController()
    {
        MultiplayerBoard board = FindObjectOfType<MultiplayerBoard>();       
        MultiplayerChessGameController controller = Instantiate(multiplayerControllerPrefab);
        controller.SetDependencies(cameraSetup, uiManager, board);
        controller.InitializeGame();
        controller.SetNetworkManager(networkManager);
        networkManager.SetDependencies(controller);
        board.SetDependencies(controller);
    }

    public void InitializeSingleplayerController()
    {
        SinglePlayerBoard board = FindObjectOfType<SinglePlayerBoard>();
        SingleplayerChessGameController controller = Instantiate(singleplayerControllerPrefab);
        controller.SetDependencies(cameraSetup, uiManager, board);
        controller.InitializeGame();
        board.SetDependencies(controller);
        controller.StartNewGame();
    }
}
