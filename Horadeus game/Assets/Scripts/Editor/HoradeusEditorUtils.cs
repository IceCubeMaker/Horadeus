using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public static class HoradeusEditorUtils
{
    [MenuItem("Horadeus/DeletePlayerData")]
    public static void DeletePlayerData()
    {
        string path = Application.persistentDataPath + GameDataManager.PLAYER_DATA_LOCATION;

        if(File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("PlayerData deleted successfully!");
        }    else
        {
            Debug.Log("No data found");
        }     
    }
}
