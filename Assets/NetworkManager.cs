using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks
{
	private const string LEVEL = "level";
	private const string TEAM = "team";
	private const byte MAX_PLAYERS = 2;
	[SerializeField] private ChessUIManager uiManager;
	[SerializeField] private GameInitializer gameInitializer;
	private MultiplayerChessGameController chessGameController;

	private ChessLevel playerLevel;

	void Awake()
	{
		PhotonNetwork.AutomaticallySyncScene = true;
	}

	public void SetDependencies(MultiplayerChessGameController chessGameController)
	{
		this.chessGameController = chessGameController;
	}

	public void Connect()
	{
		if (PhotonNetwork.IsConnected)
		{
			PhotonNetwork.JoinRandomRoom(new ExitGames.Client.Photon.Hashtable() { { LEVEL, playerLevel } }, MAX_PLAYERS);
			//PhotonNetwork.JoinRandomRoom();
		}
		else
		{
			PhotonNetwork.ConnectUsingSettings();
		}
	}

	private void Update()
	{
		uiManager.SetConnectionStatusText(PhotonNetwork.NetworkClientState.ToString());
	}

	#region Photon Callbacks

	public override void OnConnectedToMaster()
	{
		
		Debug.LogError($"Connected to server. Looking for random room with level {playerLevel}");
		PhotonNetwork.JoinRandomRoom(new ExitGames.Client.Photon.Hashtable() { { LEVEL, playerLevel } }, MAX_PLAYERS);
		//PhotonNetwork.JoinRandomRoom();
	}

	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		Debug.LogError($"Joining random room failed becuse of {message}. Creating new one with player level {playerLevel}");
		PhotonNetwork.CreateRoom(null, new RoomOptions
		{
			CustomRoomPropertiesForLobby = new string[] { LEVEL },
			MaxPlayers = MAX_PLAYERS,
			CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { LEVEL, playerLevel } }
		});
		//PhotonNetwork.CreateRoom(null);
	}

	public override void OnJoinedRoom()
	{
		Debug.LogError($"Player {PhotonNetwork.LocalPlayer.ActorNumber} joined a room with level: {(ChessLevel)PhotonNetwork.CurrentRoom.CustomProperties[LEVEL]}");
		gameInitializer.CreateMultiplayerBoard();
		PrepareTeamSelectionOptions();
		uiManager.ShowTeamSelectionScreen();

	}


	private void PrepareTeamSelectionOptions()
	{
		if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
		{
			var player = PhotonNetwork.CurrentRoom.GetPlayer(1);
			if (player.CustomProperties.ContainsKey(TEAM))
			{
				var occupiedTeam = player.CustomProperties[TEAM];
				uiManager.RestrictTeamChoice((TeamColor)occupiedTeam);
			}
		}
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		Debug.LogError($"Player {newPlayer.ActorNumber} entered a room");
	}
	#endregion

	public void SetPlayerLevel(ChessLevel level)
	{
		playerLevel = level;
		PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { LEVEL, level } });
	}

	public void SetPlayerTeam(int teamInt)
	{
		if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
		{
			var player = PhotonNetwork.CurrentRoom.GetPlayer(1);
			if (player.CustomProperties.ContainsKey(TEAM))
			{
				var occupiedTeam = player.CustomProperties[TEAM];
				teamInt = (int)occupiedTeam == 0 ? 1 : 0;
			}
		}
		PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { TEAM, teamInt } });
		gameInitializer.InitializeMultiplayerController();
		chessGameController.SetupCamera((TeamColor)teamInt);
		chessGameController.SetLocalPlayer((TeamColor)teamInt);
		chessGameController.StartNewGame();
	}



	internal bool IsRoomFull()
	{
		return PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers;
	}

}
