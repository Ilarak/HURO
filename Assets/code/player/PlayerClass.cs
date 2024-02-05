using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


//this class must always be keeping
public class savePlayer : MonoBehaviour{ 
    public Player player = new Player();
    public dataSave dataSaveCVS;

    // Start is called before the first frame update
    void OnEnable(){
        if (PlayerPrefs.HasKey("playerName")){
            player.playerName = PlayerPrefs.GetString("playerName");
        }
        else {
            PlayerPrefs.SetString("playerName","unknown");
            player.playerName = "unknown";
        }

        exportJson();
        dataSaveCVS.initializePlayer(player);
    }

    private void OnDisable(){
        // we have to put this code of ligne when the player end the exercice
        //save the exercice
        //dataSaveCVS.SaveEx();

        //save the session
        dataSaveCVS.SaveSession();

        saveJson();
    }

    public void exportJson(){
        print("starting the export");
        #if UNITY_EDITOR
            var folder = Application.streamingAssetsPath;
        #else
            var folder = Application.persistentDataPath;
        #endif

        folder += "/"+player.playerName+"/data.txt";
        if (File.Exists(folder)){
            string json = File.ReadAllText(folder);
            print(json);
            
            player = JsonUtility.FromJson<Player>(json);

            print("Recover player data");

        }
        else {
            print("new Player");
        }
    }
    public void saveJson(){
        #if UNITY_EDITOR
            var folder = Application.streamingAssetsPath;
        #else
            var folder = Application.persistentDataPath;
        #endif
        string strOutput = JsonUtility.ToJson(player);
        File.WriteAllText(folder+"/"+player.playerName+"/data.txt", strOutput);
    }
}

[System.Serializable]
public class Player{
    public string playerName="unknown";
    public int nbSession=0;
    public int nbEx1=0;
    public int nbEx2=0;
    public int nbEx3=0;
    public float maxVel=0.0F;
    public float timeTot=0.0F;
    public float totDistance = 0.0F;
    //is in fonction of the force applied to the robot and c
    //public float meanSpeed = 0.0F;
    public float meanForce = 0.0F;
    public int weigthForce =0;
}
