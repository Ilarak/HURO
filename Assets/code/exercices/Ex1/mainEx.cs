using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using TMPro;

public class mainEx : MonoBehaviour
{
    public static int nbBall;

    private GameObject middleSphere;
    public GameObject ParentsphereToReach;

    private bool playExercice = true;
    public static int gamePoint = 0;
    public TextMeshProUGUI score;

    public static List<float> timeBetween2Scores = new List<float>();

    void OnEnable(){
        nbBall = PlayerPrefs.GetInt("nbBall");
        PlayerPrefs.SetString("currentEx","ex1");
    }

    void Start(){
        HideAllSphere();
        StartCoroutine(exercice());
    }

    void Update(){
        /*if (Input.anyKeyDown){
            StopEx();
        }*/
    }
    
    public void StopEx(){
        print("Stop exercices");
        playExercice=false;

        string[] stringProcess = {"robot_state_publisher","robot_state_helper","ur_robot_driver_node","move_group",
                "python3.8","my_endpoint.launch","my_ur3_bringup.launch","controller_stopper_node", "go_to_start_position_UR.py", "unity_start_ex.py"};

        foreach (string processName in stringProcess){
            foreach (Process p in Process.GetProcessesByName(processName)) { 
                p.Kill(); 
            }
        }
    }

    public void OnDisable(){
        StopEx();
    }

    IEnumerator exercice(){
        print("start exercice");
        float lastTime = Time.time;
        while (playExercice){
            //Choose a random ball to reach
            int random = Random.Range(1,nbBall+1);
            string nameSphere = "Sphere" + random.ToString();
            //Find the object connected to this number of ball
            GameObject SphereToReach = ParentsphereToReach.transform.Find(nameSphere).gameObject;
            //Display the ball
            DisplaySphere(SphereToReach);
            //block the code until the ball has been reached
            while (!CollisionCheck.reachSphere){
                yield return null;
            }
            //Go back to the central ball
            CollisionCheck.reachSphere = false;
            DisplaySphere(middleSphere);
            HideSphere(SphereToReach);
            //block the code until the central ball has been reached
            while (!CollisionCheck.reachSphere){
                yield return null;
            }
            CollisionCheck.reachSphere = false;
            //increase the score
            gamePoint+=1;
            score.text="score : "+gamePoint.ToString();
            HideSphere(middleSphere);
            timeBetween2Scores.Add(Time.time - lastTime);
            lastTime = Time.time;

        }
    }
    void DisplaySphere(GameObject nameObject){   
        nameObject.SetActive(true);
        nameObject.tag = "VisibleBall";
    }

    void HideSphere(GameObject nameObject){
        nameObject.tag = "HiddenBall";
        nameObject.SetActive(false);
    }
    void HideAllSphere(){
        middleSphere = GameObject.Find("Sphere0");
        GameObject[] allSphere = GameObject.FindGameObjectsWithTag("HiddenBall");

        foreach (GameObject sphere in allSphere){
            sphere.SetActive(false);
        }
    }
}
