using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderControl : MonoBehaviour{
    public List<Slider> slidersList = new List<Slider>();

    [HideInInspector]
    public List<float> pos = new List<float>();
    [HideInInspector]
    public List<float> valSliderIni = new List<float>();
    public List<TextMeshProUGUI> textList = new List<TextMeshProUGUI>();
    public TextMeshProUGUI textHeigth;

    public ControllerRobot controllerRobot;
    public MoveHand moveHand;
    public PosHouse posHouse;

    public Dictionary<int, List<int>> lastSliderValUse = new Dictionary<int, List<int>>();

    bool moveKeyPress = false;

    // Start is called before the first frame update
    void Start(){
        for (int i=0; i<6; i++){
            pos.Add(0.0f);
            valSliderIni.Add(0.0f);
        }
    }

    public void valueShoulder(float newValueDegre){
        actionWhenSliderMove(0, newValueDegre);
    }

    public void valueUppderArm(float newValueDegre){
        actionWhenSliderMove(1, newValueDegre);
    }

    public void valueForearm(float newValueDegre){
        actionWhenSliderMove(2, newValueDegre);

    }

    public void valueWrist1(float newValueDegre){
        actionWhenSliderMove(3, newValueDegre);
    }

    public void valueWrist2(float newValueDegre){
        actionWhenSliderMove(4, newValueDegre);
    }

    public void valueWrist3(float newValueDegre){
        actionWhenSliderMove(5, newValueDegre);
    }

    void actionWhenSliderMove(int i, float newValue){
        int roundNewValue = Mathf.RoundToInt(newValue);
        if (moveKeyPress){
            pos[i] = newValue * Mathf.PI / 180;
            textList[i].text = (-roundNewValue).ToString()+ "°";
            moveKeyPress = false;
            return;
        }
        if(RosSubscriberExample.instance.validPosSim || !containsValue(i,roundNewValue)){
            moveHand.lastStuckKeyPress.Clear();
            lastSliderValUse.Clear();
            textList[i].text = (-roundNewValue).ToString()+ "°";
            pos[i] = newValue * Mathf.PI / 180;
            RosPublisherExample.instance.pubPosJointRobotSim(pos);
            addToDic(i,roundNewValue);
        }
    }

    void addToDic(int key, int newValue){
        if(!lastSliderValUse.ContainsKey(key)){
            lastSliderValUse[key] = new List<int>();
        }
        lastSliderValUse[key].Add(newValue);
    }
    bool containsValue(int key, int value){
        if(lastSliderValUse.ContainsKey(key)){
            return lastSliderValUse[key].Contains(value);
        }
        return false;
    }

    public void posIni(){
        for (int i = 0; i < slidersList.Count; i++) {
            valSliderIni[i] = slidersList[i].value;
        }
    }

    public void goStartPos(){
        lastSliderValUse.Clear();
    }


    public void editSlider(List<float> jointPosSim){
        moveKeyPress = true;
        for (int i = 0; i < jointPosSim.Count; i++) {
            float val = jointPosSim[i];
            if (val < 180) {
                jointPosSim[i] = -val;
            } else {
                jointPosSim[i] = 360f -val;
            }
        }
        for (int i = 0; i < slidersList.Count; i++) {
            slidersList[i].value = jointPosSim[i];
            moveKeyPress = true;
        }
    }

    public void speed(float speed){
        moveHand.stepPos = speed;
    }

    public void heigthBanana(float heigth){
        float heigthRound = Mathf.Round(heigth*10f)/10f;
        posHouse.changeHeigtHouse(heigth);
        moveHand.changeHeigtRobot();
        textHeigth.text = heigthRound.ToString()+"m";
    }

    public void heigthStartBanana(float heigth){
        float heigthRound = Mathf.Round(heigth*10f)/10f;
        textHeigth.text = heigthRound.ToString()+"m";
    }
}
