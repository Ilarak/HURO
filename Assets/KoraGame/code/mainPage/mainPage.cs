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

public class mainPage : MonoBehaviour
{
    //public KoraGameConfiguration config;
    public settingsManagment settingsMngt;
    public Image sliderInCalibration;

    public GameObject calibrationMessage;
    public GameObject mainCanvas;

    public Button startButton;

    public void Start(){
        RosPublisherExample.instance.pubStartCalib(true);
        startButton.interactable = false;
        //go to the start position of the robot
        List<float> startPos = new List<float>(){73f * Mathf.PI / 180f, -95f * Mathf.PI / 180f, 95f * Mathf.PI / 180f, -Mathf.PI / 2f, -Mathf.PI / 2f, -15f * Mathf.PI / 180f};
        RosPublisherExample.instance.pubGoJointPosition(startPos);
        StartCoroutine(wait8secondes()); 
    }

    IEnumerator wait8secondes(){
        yield return new WaitForSeconds(8);
        startButton.interactable = true;
        //save that this position is the start position
        RosPublisherExample.instance.pubSaveTCPInitialPos(true);
    }

    public void changeScene(){
        print(settingsMngt.config.axis);
        if (settingsMngt.config.axis == 0){
            SceneManager.LoadScene(2);
        }
        else if (settingsMngt.config.axis == 1){
            SceneManager.LoadScene(3);
        }
        else if (settingsMngt.config.axis == 2){
            SceneManager.LoadScene(4);
        }
        else if (settingsMngt.config.axis == 3){
            SceneManager.LoadScene(5);
        }
    }

    public void startEx(){
        if(settingsMngt.isCalibrated){
            changeScene();
        }
        else{
            mainCanvas.SetActive(false);
            calibrationMessage.SetActive(true);
        }
    }

    public void launchCalibration(){
        RosPublisherExample.instance.pubStartCalib(true);
        StartCoroutine(fillBar());
    }

    IEnumerator fillBar(){
        float elapsedTime = 0f; 
        float totTime = 4.0f;

        while (elapsedTime < totTime){
            float progression = elapsedTime / totTime;

            sliderInCalibration.fillAmount = progression;

            yield return null;

            elapsedTime += Time.deltaTime;
        }
        changeScene();
    }

    public void quit(){
        //go back to main scene
        SceneManager.LoadScene(0);
    }

    public void notLaunchCalibration(){
        changeScene();
    }
}
