using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class Launch3DEx : MonoBehaviour{

    public static int force, axis;
    public static float c;
    public static string outset_force_sensor; 

    public static Process processEx;
    private static string activeCatkin = "source /home/huro/koralie/catkin_ws/devel/setup.bash";   
    void OnEnable(){
        force = PlayerPrefs.GetInt("force");
        axis = PlayerPrefs.GetInt("axis");
        c = PlayerPrefs.GetFloat("c");
        outset_force_sensor = PlayerPrefs.GetString("outset_force_sensor");
    }
    void Start(){
        //StartCoroutine(StartCoroutine());
    }
    IEnumerator StartCoroutine(){
        string scriptPath;
        print("Launch script");
        scriptPath = "rosrun control_mov 3D_unity_start_ex.py "+ axis.ToString()+ " " + force.ToString() + " " + outset_force_sensor + " " + c.ToString(); 
        print(scriptPath);
        StartCoroutine(LaunchCoroutine(processEx ,scriptPath,"Program ex finish"));
        yield return null;
    }

    public IEnumerator LaunchCoroutine(Process process, string path, string message){
        process = new Process();
        ProcessStartInfo startInfo = new ProcessStartInfo("/bin/bash", $"-c \"{activeCatkin}  && {path} \"");
        // Process parameters
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;
        process.StartInfo = startInfo;
        // Start the process
        process.Start();

        yield return WaitEndProcess(process);
        print(message);
    }

    IEnumerator WaitEndProcess(Process process){
        while(!process.HasExited){
            yield return null;
        }
    }
}
