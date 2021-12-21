using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[System.Serializable]
public class GD_Game : GD
{
    public int timeInGame;
    public int level;
    public int gameEntersCount;
    public bool soundOn;
    public bool vibrationOn;

    public List<GD> objects;
    public List<GD_ScriptableObject> scriptableObjects;

    public GD_PlayerInventory playerInventory;

    public GD_Game() : base(GDType.Game, GDLoadOrder.Pre_0)
    {
        SetDefaults(default);
    }

    public void RestoreGame()
    {
        playerInventory.RestoreInventory(Game.inst.player.inventory);        
    }

    public GD_Game(SerializationInfo info, StreamingContext sc) : base(info, sc)
    {
        timeInGame = info.GetInt32("timeInGame");
        level = info.GetInt32("level");
        gameEntersCount = info.GetInt32("gameEntersCount");
        playerInventory = (GD_PlayerInventory)info.GetValue("playerInventory", typeof(GD_PlayerInventory));
        soundOn = info.GetBoolean(nameof(soundOn));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue("timeInGame", timeInGame);
        info.AddValue("level", level);
        info.AddValue("gameEntersCount", gameEntersCount);
        info.AddValue("playerInventory", playerInventory);
        info.AddValue(nameof(soundOn), soundOn);
    }

    [OnDeserializing]
    protected override void SetDefaults(StreamingContext ds)
    {
        level = 0;
        timeInGame = 0;
        gameEntersCount = 0;
        soundOn = true;
        objects = new List<GD>();
        playerInventory = new GD_PlayerInventory();
    }

}
