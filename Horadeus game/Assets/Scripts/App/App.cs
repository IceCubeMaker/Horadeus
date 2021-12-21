using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class App : Singleton<App>
{
    public AppUI appUI;

    public SO_AppConfig appConfig;

    public override void Init()
    {
        InitAppConfig();

        HSceneManager.InitIfNeeded(null);
        GameStateManager.InitIfNeeded(null);

        appUI.Init();
    }

    protected override void Shutdown()
    {
        
    }    

    private void InitAppConfig()
    {
        Application.targetFrameRate = appConfig.targetFrameRate;
    }

    private void InternalAwake()
    {

    }

    private void InternalStart()
    {

    }

    private void InternalPreUpdate()
    {
        HGameLoop.inst.InternalPreUpdate();
    }

    private void InternalUpdate()
    {
        HGameLoop.inst.InternalUpdate();
    }

    private void InternalLateUpdate()
    {
        HGameLoop.inst.InternalLateUpdate();
    }

    private void InternalFixedUpdate()
    {
        HGameLoop.inst.InternalFixedUpdate();
    }

    private void Awake()
    {
        Init();
        InternalAwake();
    }

    private void Start()
    {
        InternalStart();
    }

    private void Update()
    {
        InternalUpdate();
    }

    private void LateUpdate()
    {
        InternalLateUpdate();
    }

    private void FixedUpdate()
    {
        InternalFixedUpdate();
    }

    private void OnDestroy()
    {
        
    }

    private void OnApplicationQuit()
    {
        
    }

    private void OnApplicationFocus(bool focus)
    {
        
    }
}
