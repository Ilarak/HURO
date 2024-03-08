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

public class settingsManagment : MonoBehaviour
{
    public KoraGameConfiguration config = new KoraGameConfiguration();
    //Parameters to check if the exercice can start
    bool good_force = true, good_c = true, good_timer = true;
    public bool isCalibrated;

    //Textes on unity
    public TextMeshProUGUI notAllGood, notGoodForce, notGoodC, notGoodTimer;

    public TMP_InputField inputC, inputForce, inputTimer;
    public TMP_Dropdown dropdownAxis;
    public Toggle toogleIsTimer;
    public Image sliderInCalibration;

    string playerName = "";

    public GameObject canvasSettings, canvasMain, inputFieldTime, minText;

    void Start(){
        playerName = PlayerPrefs.GetString("playerName");
        loadConfiguration();
    }

    public void ReadStringForceInput(string s){
        if (float.TryParse(s, out config.force)){
            if (config.force<0){
                good_force = false;
                notGoodForce.text = "Force too small (<0)";
            }
            else if (config.force > 100){
                good_force = false;
                notGoodForce.text = "Force too big (>100)";
            }
            else {
                RosPublisherExample.instance.pubOpposingForce(config.force);
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
        if (float.TryParse(s, out config.c)){
            if (config.c <= 0){
                good_c = false;
                notGoodC.text = "c too small (<=0)";
            }
            else if (config.c > 20){
                good_c = false;
                notGoodC.text = "c too big (>20)";
            }
            else {
                RosPublisherExample.instance.pubCValue(config.c);
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

    public void ReadStringTimerInput(string s){
        if (int.TryParse(s, out config.timer)){
            if (config.timer < 0){
                good_timer = false;
                notGoodTimer.text = "timer too small (<=0)";
            }
            else if (config.timer > 20){
                good_timer = false;
                notGoodTimer.text = "timer too big (>20)";
            }
            else {
                good_timer = true;
                notGoodTimer.text = "";
                PlayerPrefs.SetInt("timerEx",config.timer);
            }
        }
        else if (s==""){
            good_timer = false;
            notGoodTimer.text = "";
        }
        else {
            good_timer = false;
            notGoodTimer.text = "Not an int";
        }
    }

    public void isTimer(bool isTimer){
        inputFieldTime.SetActive(isTimer);
        minText.SetActive(isTimer);
        if(!isTimer){
            notGoodTimer.text = "";
            good_timer = true;
            config.timer=0;
            PlayerPrefs.SetInt("timerEx",config.timer);
        }
    }

    public void ReadIntInput(int s){
        config.axis = s;
        RosPublisherExample.instance.pubPlaneOfWork(config.axis);
    }

    public void goCalibration(){
        isCalibrated = true;
        RosPublisherExample.instance.pubStartCalib(true);
        StartCoroutine(fillBar());
    }

    public void displaySettingsMenu(){
        canvasSettings.SetActive(true);
        canvasMain.SetActive(false);
    }

    public void exitSettingsMenu(){
        // if all the options are correct : 
        if(good_force && good_c && good_timer){
            canvasSettings.SetActive(false);
            canvasMain.SetActive(true);
        }
        else if (!good_force){
            notAllGood.text="You have to write a correct force";
        }
        else if (!good_c){
            notAllGood.text="You have to write a correct c";
        }
    }

    public void saveConfiguration(){
        if(good_force && good_c && good_timer){
            #if UNITY_EDITOR
            var folder = Application.streamingAssetsPath;
            #else
                var folder = Application.persistentDataPath;
            #endif
            string strOutput = JsonUtility.ToJson(config);
            print(strOutput);
            File.WriteAllText(folder+"/"+playerName+"/KoraFruitConfiguration.txt", strOutput);
        }   
        else{
            notAllGood.text ="Can't load the config, not correct values";
        }
    }

    public void loadConfiguration(){
        #if UNITY_EDITOR
            var folder = Application.streamingAssetsPath;
        #else
            var folder = Application.persistentDataPath;
        #endif

        folder += "/"+playerName+"/KoraFruitConfiguration.txt";
        if (File.Exists(folder)){
            string json = File.ReadAllText(folder);
            
            config = JsonUtility.FromJson<KoraGameConfiguration>(json);
            publishConfigCanvas();
            publicshConfigRos();

            notAllGood.text ="";
            notGoodForce.text ="";
            notGoodC.text ="";
            notGoodTimer.text ="";
        }
    }

    void publishConfigCanvas(){
        inputC.text = config.c.ToString();
        inputForce.text = config.force.ToString();
        if (config.timer == 0){
            toogleIsTimer.isOn = false;
        }
        else{
            toogleIsTimer.isOn = true;
        }
        PlayerPrefs.SetInt("timerEx",config.timer);
        inputTimer.text = config.timer.ToString();
        dropdownAxis.value = config.axis;
    }

    void publicshConfigRos(){
        RosPublisherExample.instance.pubCValue(config.c);
        RosPublisherExample.instance.pubOpposingForce(config.force);
        RosPublisherExample.instance.pubPlaneOfWork(config.axis);
        PlayerPrefs.SetInt("timerEx",config.timer);
        RosPublisherExample.instance.sendCalibVal(config.calib);
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
    }
}

[System.Serializable]
public class KoraGameConfiguration{
    public int axis = 3;
    public float c = 1.7f;
    public float force = 2f;
    public int timer = 2;
    public List<float> calib = new List<float> {0.0f,0.0f,0.0f};
}



