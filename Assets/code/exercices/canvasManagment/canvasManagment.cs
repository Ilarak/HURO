using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class canvasManagment : MonoBehaviour
{

    public GameObject canvas;
    public void CanvasStopExercice(){
        canvas.SetActive(true);

        GameObject[] objs = GameObject.FindGameObjectsWithTag("saveObject");
        
        GameObject obj = objs[0];  
        obj.SendMessage("SaveEx");

        
    }

    public void endSession(){
        Application.Quit();
    }

    public void newExercice(){
        GameObject obj = GameObject.Find("playerManagment");
        DontDestroyOnLoad(obj);
        SceneManager.LoadScene(0);
    }
}


