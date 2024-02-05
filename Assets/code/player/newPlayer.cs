using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class newPlayer : MonoBehaviour
{
    public Player player;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void addFile(){
        #if UNITY_EDITOR
            var folder = Application.streamingAssetsPath;
        #else
            var folder = Application.persistentDataPath;
        #endif
        Directory.CreateDirectory(folder+"/"+player.playerName);
    }
}
