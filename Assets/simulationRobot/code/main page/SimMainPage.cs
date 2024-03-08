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

public class SimMainPage : MonoBehaviour
{
    public GameObject canvasMain, canvasSettings;

    public Button exitButton;

    public SimSettings config;

    public void startSim(){
        PlayerPrefs.SetFloat("simX",config.x);
        PlayerPrefs.SetFloat("simY",config.y);
        PlayerPrefs.SetFloat("simZ",config.z);
        PlayerPrefs.SetFloat("simRotZ",config.rotZ);
        RosSubscriberExample.instance.simRunning=true;
        RosPublisherExample.instance.pubStartSim(true);
        if (config.isUr3){
            SceneManager.LoadScene(7);
        }
        else {
            SceneManager.LoadScene(8);
        }
    }

    public void quit(){
        //go back to main scene
        SceneManager.LoadScene(0);
    }

    public void goSettings(){
        canvasMain.SetActive(false);
        canvasSettings.SetActive(true);
    }

    public void quitSettings(){
        canvasMain.SetActive(true);
        canvasSettings.SetActive(false);
    }
}
