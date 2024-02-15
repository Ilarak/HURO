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
    private int axis;
    //Parameters to check if the exercice can start
    private bool robot_in_position, good_force, calibration, good_c;

    //Textes on unity
    public TextMeshProUGUI notAllGood, notGoodForce,inCalibration, notGoodC, infos;
    //Process
    private Process processEx, processUR, processMoveit, processGoStart, processTcp, processCalibration;

    //Informations to start python script
    private static string activeCatkin = "source /home/huro/koralie/catkin_ws/devel/setup.bash";

    void Start(){
        if(singletonPlayer.firstInstance){
            robot_in_position = true;
            StartCoroutine(StartCoroutine());
        }
        else {
            robot_in_position = true;
            good_force = true;
            calibration = true;
            good_c = true;
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
        infos.text = "start the program on the tablet";
        yield return new WaitForSeconds(1f);

        /*print("Launch main script");
        scriptPath = "rosrun control_mov main.py";
        StartCoroutine(LaunchCoroutine(processGoStart,scriptPath,"stop code"));
        infos.text="";*/

        //publish default values of c
        RosPublisherExample.instance.pubCValue(1.7f);
        good_c = true;
    }
  
    public IEnumerator LaunchCoroutine(Process process, string path, string message,int positionOrCalibration = 0){
        process = new Process();
        ProcessStartInfo startInfo = new ProcessStartInfo("/bin/bash", $"-c \"{activeCatkin}  && {path} \"");
        // Process parameters
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;
        startInfo.RedirectStandardOutput = true;
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
    public void ResetGame(){
        infos.text="";
        StartCoroutine(Reset());
    }
    public IEnumerator Reset(){
        StopGame();
        yield return new WaitForSeconds(3f);
        StartCoroutine(StartCoroutine());
        good_force = false;
        robot_in_position = false;
        calibration = false;
        good_c = false;
    }

    public void ReadStringForceInput(string s){
        float force;
        if (float.TryParse(s, out force)){
            if (force<0){
                good_force = false;
                notGoodForce.text = "Force too small (<0)";
            }
            else if (force > 100){
                good_force = false;
                notGoodForce.text = "Force too big (>100)";
            }
            else {
                RosPublisherExample.instance.pubOpposingForce(force);
                good_force = true;
                notGoodForce.text = "";
            }
        }
        else if (s==""){
            good_force = false;
            notGoodForce.text = "";
        }
        else {
            good_force = false;
            notGoodForce.text = "Not an float";
        }
        
    }
    public void ReadStringCInput(string s){
        float c;
        if (float.TryParse(s, out c)){
            if (c <= 0){
                good_c = false;
                notGoodC.text = "c too small (<=0)";
            }
            else if (c > 100){
                good_c = false;
                notGoodC.text = "c too big (>100)";
            }
            else {
                RosPublisherExample.instance.pubCValue(c);
                good_c = true;
                notGoodC.text = "";
            }
        }
        else if (s==""){
            good_c = false;
            notGoodC.text = "";
        }
        else {
            good_c = false;
            notGoodC.text = "Not an float";
        }
    }


    public void ReadIntInput(int s){
        RosPublisherExample.instance.pubPlaneOfWork(s);
        axis = s;
    }

    public void goCalibration(){
        calibration = true;
        RosPublisherExample.instance.pubStartCalib(true);
        
    }

    public void changeScene(){
        if (axis == 0){
            SceneManager.LoadScene(1);
        }
        else if (axis == 1){
            SceneManager.LoadScene(2);
        }
        else if (axis == 2){
            SceneManager.LoadScene(3);
        }
        else if (axis == 3){
            SceneManager.LoadScene(4);
        }
    }

    public void startEx(){
        changeScene();
        if (good_force && robot_in_position && calibration && good_c){
            changeScene();
        }
        else if (!good_force){
            notAllGood.text="You have to write a correct force";
        }
        else if (!good_c){
            notAllGood.text="You have to write a correct c";
        }
        else if (!robot_in_position){
            notAllGood.text="Robot not in position, wait";
        }
        else {
            notAllGood.text="You have to calibrate the robot";
        }
    }
}
