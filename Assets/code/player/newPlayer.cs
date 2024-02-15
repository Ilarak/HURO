using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class newPlayer : MonoBehaviour
{
    public string NewplayerName; 
    Player player;

    void addFile(){
        #if UNITY_EDITOR
            var folder = Application.streamingAssetsPath;
        #else
            var folder = Application.persistentDataPath;
        #endif
        Directory.CreateDirectory(folder+"/"+NewplayerName);
        player.playerName = NewplayerName;
    }
}
