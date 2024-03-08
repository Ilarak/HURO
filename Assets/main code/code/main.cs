using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Diagnostics;
using UnityEngine.UI;
using System.Threading.Tasks;
using TMPro;

using System.Text;
using System.IO;
using System.Threading;
using System.ComponentModel;

public class main : MonoBehaviour
{
    //Process
    private Process processEx, processUR, processMoveit, processGoStart, processTcp, processCalibration;

    //Informations to start python script
    private static string activeCatkin = "source /home/huro/koralie/catkin_ws/devel/setup.bash";

    void Start(){
        //If it is the first instance, launch all the processes
        if(singletonPlayer.firstInstance){
            StartCoroutine(StartCoroutine());
        }
    } 
    IEnumerator StartCoroutine(){
        print("Launch UR control");
        string scriptPath;
        //scriptPath = "roslaunch ur_robot_driver ur3_bringup.launch ip:="+ip_of_the_robot;
        scriptPath = "roslaunch ur_robot_driver my_ur3_bringup.launch";
        StartCoroutine(LaunchCoroutine(processUR, scriptPath,"stop UR control"));
        //We have to wait, if not : pb with the rosmaster
        yield return new WaitForSeconds(1f);

        print("Launch tcp connection");
        scriptPath = "roslaunch ros_tcp_endpoint my_endpoint.launch";
        StartCoroutine(LaunchCoroutine(processTcp, scriptPath,"stop tcp"));

        yield return new WaitForSeconds(1f);

        print("Launch moveit");
        scriptPath = "roslaunch ur3_moveit_config moveit_planning_execution.launch";
        StartCoroutine(LaunchCoroutine(processMoveit,scriptPath,"stop moveit"));
        yield return new WaitForSeconds(1f);

        /*print("Launch main script");
        scriptPath = "rosrun control_mov main.py";
        StartCoroutine(LaunchCoroutine(processGoStart,scriptPath,"stop code"));
        infos.text="";*/
    }

    private void FindActiveCatkin(){
        Process process = new Process();
        string activeRos = "source /opt/ros/noetic/setup.bash";
        string path = "rospack find control_mov";
        ProcessStartInfo startInfo = new ProcessStartInfo("/bin/bash", $"-c \"{activeRos}  && {path} \"");
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo = startInfo;
        process.Start();

        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        if (output != ""){
            print(output);
        }
    }
  
    public IEnumerator LaunchCoroutine(Process process, string path, string message,int positionOrCalibration = 0){
        process = new Process();
        ProcessStartInfo startInfo = new ProcessStartInfo("/bin/bash", $"-c \"{activeCatkin}  && {path} \"");
        // Process parameters
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;
        process.StartInfo = startInfo;

        // Start the process
        process.Start();
        
        //wait until the end of the process
        yield return WaitEndProcess(process);

        print(message);
    }

    IEnumerator WaitEndProcess(Process process){
        while(!process.HasExited){
            yield return null;
        }
    }

    public void StopGame(){
        print("stop game");
        // All the processes that has to be closed
        string[] stringProcess = {"robot_state_publisher","robot_state_helper","ur_robot_driver_node","move_group","moveit_planning_execution.launch",
                "python3.8","my_endpoint.launch","my_ur3_bringup.launch","controller_stopper_node", 
                "go_to_start_position_UR.py", "unity_start_ex.py", "main.py"};

        foreach (string processName in stringProcess){
            foreach (Process p in Process.GetProcessesByName(processName)) {
                p.Kill(); 
            }
        }
    }

    public void launchSim(){
        SceneManager.LoadScene(6);
    }

    public void startKoraGame(){
        SceneManager.LoadScene(1);
    }

}
