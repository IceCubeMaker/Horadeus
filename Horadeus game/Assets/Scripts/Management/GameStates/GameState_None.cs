using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState_None : GameState
{

	public GameState_None() : base(GameStateType.None)
	{

	}

	protected override void OnSwitchedToMe(GameState prevState, GameContext context)
	{

	}

	protected override bool SwitchTo(GameState state, GameContext context)
	{

		switch (state.type)
		{
			case GameStateType.Menu:
				context.SetState(state);
				return true;
			case GameStateType.Gameplay:
				context.SetState(state);
				return true;
		}

		return false;
	}

}