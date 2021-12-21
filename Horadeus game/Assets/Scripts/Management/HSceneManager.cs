using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HSceneManager : Singleton<HSceneManager>
{

    public static Action<SceneEvent> OnSceneChangeStart;
    public static Action<SceneEvent> OnSceneChangeEnd;

    public static GameScene currentScene { get; private set; }

    private static SceneEvent currentEvent;

    public override void Init()
    {
        Scene startScene = SceneManager.GetActiveScene();
        currentScene = new GameScene() { scene = startScene, name = startScene.name };

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    protected override void Shutdown()
    {

    }

    private void Internal_LoadScene(string name)
    {
        GameScene nextScene = new GameScene() {
            name = name
        };

        currentEvent = new SceneEvent() {
            prev = currentScene,
            next = nextScene
        };

        OnSceneChangeStart?.Invoke(currentEvent);
        SceneManager.LoadScene(name, LoadSceneMode.Single);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        OnSceneChangeEnd?.Invoke(currentEvent);
    }

    #region Wrappers

    public static void LoadScene(string name)
    {
        inst.Internal_LoadScene(name);
    }

    public static void ReloadScene()
    {
        string currScene = SceneManager.GetActiveScene().name;
        inst.Internal_LoadScene(currScene);
    }

    #endregion

}

public struct SceneEvent
{
    public GameScene prev;
    public GameScene next;
}

public struct GameScene
{
    public string name;
    public Scene scene;
}