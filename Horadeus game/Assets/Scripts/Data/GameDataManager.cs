using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameDataManager : Singleton<GameDataManager>
{

    public string saveFolder;

    public static string ASSETS_ROOT_PATH;
    public static string PERSISTENT_DATA_PATH;

    public const string PLAYER_DATA_LOCATION = "/gameData.dat";

    public override void Init()
    {
        ASSETS_ROOT_PATH = Application.dataPath;
        PERSISTENT_DATA_PATH = Application.persistentDataPath;
    }

    protected override void Shutdown()
    {
        Save();
    }

    public void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            Save();
        }
    }

    public void Save()
    {
        GD_Game gdGame = Game.data;
        gdGame.timeInGame = gdGame.timeInGame + (int)Time.realtimeSinceStartup;

        FileStream fs = new FileStream(PERSISTENT_DATA_PATH + PLAYER_DATA_LOCATION, FileMode.Create);

        BinaryFormatter formatter = new BinaryFormatter();

        try
        {
            formatter.Serialize(fs, gdGame);
        } catch (SerializationException e)
        {
            Debug.Log("Failed to serialize. Reason: " + e.Message);
            throw;
        } finally
        {
            fs.Close();
        }
    }

    public GD_Game Load()
    {
        GD_Game gdGame = Load<GD_Game>(PERSISTENT_DATA_PATH + PLAYER_DATA_LOCATION);
        if (gdGame == null)
        {
            gdGame = new GD_Game();
        }

        gdGame.RestoreGame();
        return gdGame;
    }

    public T Load<T>(string filename)
    {
        if (File.Exists(filename))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(filename, FileMode.OpenOrCreate);
            bool flag = false;
            T result = default(T);
            if (fileStream != null)
            {
                try
                {
                    result = (T)(binaryFormatter.Deserialize(fileStream));
                } catch (Exception message)
                {
                    flag = true;
                    Debug.Log(message);
                }
                fileStream.Close();
            } else
            {
                flag = true;
            }
            if (!flag)
            {
                return result;
            }
        }
        return default(T);
    }

}
