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
    public static int force, axis, nbBall;
    public static float c;
    public static string outset_force_sensor = "2.52a2.71a-39.15";
    //Parameters to check if the exercice can start
    public static bool robot_in_position, good_force, calibration, good_c;
    //Textes on unity
    public TextMeshProUGUI notAllGood, notGoodForce,inCalibration, notGoodC, infos;
    //Process
    public static Process processEx, processUR, processMoveit, processGoStart, processTcp, processCalibration;

    //Informations to start python script
    private static string activeCatkin = "source /home/huro/koralie/catkin_ws/devel/setup.bash";

    void Start(){
        StartCoroutine(StartCoroutine());
    } 

    void OnDisable(){
        print("nb ball : " + nbBall);
        PlayerPrefs.SetInt("nbBall",nbBall);
        PlayerPrefs.SetInt("axis",axis);
        PlayerPrefs.SetInt("force",force);
        PlayerPrefs.SetFloat("c",c);
        PlayerPrefs.SetString("outset_force_sensor",outset_force_sensor);
        //StopGame();
    }

    IEnumerator StartCoroutine(){
        print("Launch UR control");
        string scriptPath;
        scriptPath = "roslaunch ur_robot_driver my_ur3_bringup.launch";
        StartCoroutine(LaunchCoroutine(processUR,scriptPath,"stop UR connection"));
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
        yield return new WaitForSeconds(2f);

        print("Launch go start position");
        scriptPath = "rosrun control_mov go_to_start_position_UR.py";
        StartCoroutine(LaunchCoroutine(processGoStart,scriptPath,"robot in position", 1));
        yield return new WaitForSeconds(4f);
        infos.text="";

        //default values of c
        c=1.7f;
        good_c = true;
    }
  
    // if it is to go to the start position ( = 1) or calibration ( = 2), do others things
    public IEnumerator LaunchCoroutine(Process process, string path, string message,int positionOrCalibration = 0){
        process = new Process();
        ProcessStartInfo startInfo = new ProcessStartInfo("/bin/bash", $"-c \"{activeCatkin}  && {path} \"");
        // Process parameters
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;
        startInfo.RedirectStandardOutput = true;
        process.StartInfo = startInfo;
        
        //We read the message of the process to see if it is a succes
        if(positionOrCalibration==1){
            process.OutputDataReceived += new DataReceivedEventHandler((s,e) =>{
                if(e.Data.Contains("The robot is in position")){
                    robot_in_position = true;
                    print(message);
                    inCalibration.text = "";
                };
            });
        }

        //We recover the data of the strengh sensor
        else if (positionOrCalibration==2){
            process.OutputDataReceived += new DataReceivedEventHandler((s,e) =>{
                if(e.Data.Contains("a")){
                    outset_force_sensor = e.Data;
                }
            });
        }

        // Start the process
        process.Start();

        process.BeginOutputReadLine();
        
        //wait until the end of the process
        yield return WaitEndProcess(process);

        if (positionOrCalibration == 1) {
            if(!robot_in_position){
                infos.text = "You have to reset, the robot is not in position";
                print("You have to reset, the robot is not in position");
            }
        }
        else if (positionOrCalibration == 2){
            calibration = true;
            print("outset " + outset_force_sensor + "test");
            inCalibration.text = "Calibration done";
            print(message);
        }
        else {
            print(message);
        }
    }
    public IEnumerator TestLaunchCoroutine(Process process, string path, string message){
            process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo("/bin/bash", $"-c \"{activeCatkin}  && {path} \"");
            // Process parameters
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            process.StartInfo = startInfo;

            // Set our event handler to asynchronously read the sort output.
            process.OutputDataReceived += new DataReceivedEventHandler((s,e) =>{
                if(e.Data.Contains("The robot is in position")){
                    robot_in_position = true;
                    print(message);
                    inCalibration.text = "";
                };
            });
            // Start the process.
            process.Start();

            // Start the asynchronous read of the sort output stream.
            process.BeginOutputReadLine();

            // Wait for the sort process to write the sorted text lines.
            yield return WaitEndProcess(process);

            process.Close();
            //print(message);
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
                "go_to_start_position_UR.py", "unity_start_ex.py", "unity_start_ex.py"};

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
    }

    public void ReadStringForceInput(string s){
        if (int.TryParse(s, out force)){
            if (force<0){
                good_force = false;
                notGoodForce.text = "Force too small (<0)";
            }
            else if (force > 100){
                good_force = false;
                notGoodForce.text = "Force too big (>100)";
            }
            else {
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
            notGoodForce.text = "Not an int";
        }
    }
    public void ReadStringCInput(string s){
        if (float.TryParse(s, out c)){
            print("la transfo a marche" + c);
            if (c <= 0){
                good_c = false;
                notGoodC.text = "c too small (<=0)";
            }
            else if (c > 100){
                good_c = false;
                notGoodC.text = "c too big (>100)";
            }
            else {
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
        axis = s;
    }

    public void goCalibration(){
        if (robot_in_position){
            inCalibration.color = Color.white;
            inCalibration.text = "In calibration, wait...";
            string scriptPath = "rosrun control_mov calibration.py";
            StartCoroutine(LaunchCoroutine(processGoStart,scriptPath,"Calibration done",2));
        }
        else {
            inCalibration.text = "The robot must be in position, wait...";
            inCalibration.color = Color.red;
        }
        
    }

    public void changeScene(){
        //nb of the balls to reach for the exercice
        GameObject obj = GameObject.Find("playerManagment");
        DontDestroyOnLoad(obj);
        nbBall=22;
        SceneManager.LoadScene(3);
        /*if (axis == 0){
            nbBall=11;
            SceneManager.LoadScene(1);
        }
        if (axis == 1){
            nbBall=9;
            SceneManager.LoadScene(2);
        }
        if (axis == 2){
            nbBall=17;
            SceneManager.LoadScene(3);
        }
        if (axis == 3){
            nbBall=22;
            SceneManager.LoadScene(4);
        } */       
    }

    public void startEx(){
        changeScene();
        /*
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
        }*/
    }
}
