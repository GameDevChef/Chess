using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleplayerChessGameController : ChessGameController
{
	protected override void SetGameState(GameState state)
	{
		this.state = state;
	}

	public override void TryToStartThisGame()
	{
		SetGameState(GameState.Play);

	}

}
