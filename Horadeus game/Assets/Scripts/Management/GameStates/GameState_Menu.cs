using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState_Menu : GameState
{

	public GameState_Menu() : base(GameStateType.Menu)
	{

	}

	protected override void OnSwitchedToMe(GameState prevState, GameContext context)
	{
		Debug.Log("OnSwitchedToMe: menu");
	}

	protected override bool SwitchTo(GameState state, GameContext context)
	{
		switch (state.type)
		{
			case GameStateType.Menu:
				return false;
			case GameStateType.Gameplay:
				context.SetState(state);
				return true;
		}

		return false;
	}

}