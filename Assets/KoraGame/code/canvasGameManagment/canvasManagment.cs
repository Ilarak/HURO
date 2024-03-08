using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class canvasManagment : MonoBehaviour
{

    public GameObject canvas;
    public void CanvasStopExercice(){
        //stop the infinit loop of the exercice
        RosPublisherExample.instance.pubStartEx(false); 
        RosSubscriberExample.instance.exerciceRunning = false;

        //active the canvas 
        canvas.SetActive(true);

        //save the object
        GameObject obj = GameObject.FindWithTag("saveObject");
        obj.SendMessage("SaveEx");
    }

    public void CanvasStopSim(){
        RosPublisherExample.instance.pubStartSim(false); 
        canvas.SetActive(true);
    }

    public void endSession(){
        Application.Quit();
    }

    public void newExercice(){
        SceneManager.LoadScene(0);
    }
}


