using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : Singleton<Game> {

	public static GD_Game data;

	public HCamera mainCamera;

	public Player player;
    public Map map;

	public override void Init()
	{

	}

	protected override void Shutdown()
	{

	}

	public void StartGame()
	{
		GameDataManager.InitIfNeeded(null);
		data = GameDataManager.inst.Load();

		StartGameLogic();

		HGameLoop.Update.Register(InternalUpdate);
		HGameLoop.FixedUpdate.Register(InternalFixedUpdate);
	}

	private void StartGameLogic()
	{
		mainCamera.Init();

		player.Init(mainCamera);
		map.Init();

		mainCamera.SetTarget(new CameraTarget()
		{
			transform = player.movement.transform
		});
	}

	private void InternalUpdate()
	{
		player.InternalUpdate();
		map.InternalUpdate();
		mainCamera.InternalUpdate();
	}

	private void InternalFixedUpdate()
    {
		player.InternalFixedUpdate();
	}

	public void RestartLevel()
	{
		HSceneManager.ReloadScene();
	}

}
