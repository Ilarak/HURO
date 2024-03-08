using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using TMPro;

public class mainEx1 : MonoBehaviour
{
    private int nbBall;

    private GameObject basket;
    public GameObject ParentsphereToReach;
    public GameObject ParentFruitBasket;

    public static int gamePoint = 0;
    public TextMeshProUGUI score;
    public TextMeshProUGUI timerText;

    public ChangeMesh changeMesh;

    public static List<float> timeBetween2Scores = new List<float>();

    float timeLeftSec;

    public canvasManagment canvasManagment;

    void OnEnable(){
        timeLeftSec = PlayerPrefs.GetInt("timerEx")*60;

        //start the exercice
        PlayerPrefs.SetString("currentEx","ex1");
        RosPublisherExample.instance.pubStartEx(true);
        RosSubscriberExample.instance.exerciceRunning = true;
    }

    void Start(){
        StartCoroutine(RosSubscriberExample.instance.findHand());
        
        setNbObjet();

        basket = GameObject.Find("Basket");

        StartCoroutine(exercice());
        gamePoint = 0;
    }

    private void setNbObjet(){
        nbBall = ParentsphereToReach.transform.childCount -1 ;
    }

    IEnumerator startTimer(){
        while(timeLeftSec > 0){
            timerText.text = timeLeftSec.ToString();
            yield return new WaitForSeconds(1f);
            timeLeftSec -= 1f;
        }
        canvasManagment.CanvasStopExercice();
    }
    string timeFormat(){
        string str = "";
        if (timeLeftSec >= 60 ){
            str = (timeLeftSec/60).ToString() + "min ";
        }
        str += timeLeftSec%60 + "sec";
        
        return str;
    }

    IEnumerator exercice(){
        print("start exercice");
        //Start the countdown
        if (timeLeftSec != 0){

            StartCoroutine(startTimer());
        }
        
        float lastTime = Time.time;
        while (RosSubscriberExample.instance.exerciceRunning){
            //Choose a random ball to reach
            int random = Random.Range(1,nbBall+1);

            //Find the object connected to this number of ball a put it a random fruit mesh
            string nameSphere = "Sphere" + random.ToString();
            GameObject SphereToReach = ParentsphereToReach.transform.Find(nameSphere).gameObject;
            changeMesh.changeMeshFruit(SphereToReach);
            //Display the ball
            DisplaySphere(SphereToReach);
            //block the code until the ball hasnt been reached
            while (!CollisionCheck.reachSphere){
                yield return null;
            }

            // Keep in memory the initial position of the sphere
            Vector3 posSphere = SphereToReach.transform.position;

            CollisionCheck.reachSphere = false;
            basket.tag = "objToReach";
            if(changeMesh.isStrawberry){
                SphereToReach.tag = "strawberry";
            }
            else {
                SphereToReach.tag = "Untagged";
            }
            //block the code until the central basket has been reached

            while (!CollisionCheck.reachSphere){
                //The fruit follow the hand
                SphereToReach.transform.position = RosSubscriberExample.posHand;
                yield return null;
            }
            basket.tag = "Untagged";
            SphereToReach.transform.position = posSphere;
            HideSphere(SphereToReach);

            //add the fruit to the basket
            if (ParentFruitBasket.transform.childCount > gamePoint){
                int score = gamePoint + 1;
                string nameFruitBasket = "basketFr" + (score+1 ).ToString();
                GameObject FruitBasket = ParentFruitBasket.transform.Find(nameFruitBasket).gameObject;
                changeMesh.changeBasketMesh(FruitBasket);
                DisplayFruitBasket(FruitBasket);
            }
            
            CollisionCheck.reachSphere = false;
            //increase the score
            gamePoint+=1;
            score.text="score : "+gamePoint.ToString();

            //Keep in memory the time
            timeBetween2Scores.Add(Time.time - lastTime);
            lastTime = Time.time;

        }
    }
    void DisplaySphere(GameObject nameObject){   
        nameObject.SetActive(true);
        nameObject.tag = "objToReach";
    }

    void DisplayFruitBasket(GameObject nameObject){
        nameObject.SetActive(true);
    }

    void HideSphere(GameObject nameObject){
        nameObject.SetActive(false);
    }
}
