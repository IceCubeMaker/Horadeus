using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState_Gameplay : GameState
{

	public GameState_Gameplay() : base(GameStateType.Gameplay)
	{

	}

	protected override void OnSwitchedToMe(GameState prevState, GameContext context)
	{
		context.game.StartGame();
	}

	protected override bool SwitchTo(GameState state, GameContext context)
	{
		switch (state.type)
		{
			case GameStateType.Menu:
				context.SetState(state);
				return true;
			case GameStateType.Gameplay:
				return false;
		}

		return false;
	}

}