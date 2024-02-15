using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Diagnostics;

// when the player log in, active this fonction

//this class must always be keeping
public class savePlayer : MonoBehaviour{ 
    
    public Player player = new Player();
    public dataSave dataSaveCVS;

    void Start(){
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
        // if the gameObject is destroy, dont do that
        if (!singletonPlayer.isDestroying){
            singletonPlayer.isDestroying = false;
            //save the session
            dataSaveCVS.SaveSession();

            saveJson();

            //Kill all the processes on Disable
            string[] stringProcess = {"robot_state_publisher","robot_state_helper","ur_robot_driver_node","move_group",
                    "python3.8","my_endpoint.launch","my_ur3_bringup.launch","controller_stopper_node", "go_to_start_position_UR.py", "main.py"};

            foreach (string processName in stringProcess){
                foreach (Process p in Process.GetProcessesByName(processName)) {
                    p.Kill(); 
                }
            }
        }
        
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
    public float totDistance=0.0F;
    public float averageForce=0.0F;
    public int weigthForce=0;
}
