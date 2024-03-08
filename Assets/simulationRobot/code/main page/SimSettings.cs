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

public class SimSettings : MonoBehaviour
{
    public GameObject textUR3, textUR10;
    public GameObject settingsTextUR3, settingsTextUR10;

    public GameObject crossX, crossY, crossZ, crossRot, crossAdd;

    public Button exitButton;

    public bool isUr3 = false;

    [HideInInspector]
    public float x = 1, y = 0, z = 1, rotZ = 0;

    List<float> basicUr3Values = new List<float>(){0.35f,0f,0.9f,0f};
    List<float> basicUr10Values = new List<float>(){0.8f,-0.2f,1f,0f};

    bool goodX = true ,goodY = true, goodZ = true, goodRot = true, goodAddXY = true;

    public TextMeshProUGUI textMinRot, textMaxRot;
    float rotMax = Mathf.PI/2;
    float rotMin = -Mathf.PI/2;
    public TextMeshProUGUI textPosMinX, textPosMaxX;
    public TextMeshProUGUI textPosMinY, textPosMaxY;
    float posMaxXY;
    float posMinXY;
    public TextMeshProUGUI textMaxAddXY;
    public TextMeshProUGUI textAdd;
    float maxAddXY;
    public TextMeshProUGUI textPosMinZ, textPosMaxZ;
    float posMaxZ;
    float posMinZ;

    bool inverse = false;
    
    public List<TMP_InputField> input = new List<TMP_InputField> ();

    void Start(){
        toogleUR(true);
    }

    public void startSim(){
        PlayerPrefs.SetFloat("simX",x);
        PlayerPrefs.SetFloat("simY",y);
        PlayerPrefs.SetFloat("simZ",z);
        PlayerPrefs.SetFloat("simRotZ",rotZ);
        RosSubscriberExample.instance.simRunning=true;
        RosPublisherExample.instance.pubStartSim(true);
        
        if (isUr3){
            SceneManager.LoadScene(7);
        }
        else {
            SceneManager.LoadScene(8);
        }
    }

    public void toogleUR(bool isUr10){
        isUr3 = !isUr10;
        if(isUr10){
            RosPublisherExample.instance.pubRobotName("ur10");
            textUR10.SetActive(true);
            settingsTextUR10.SetActive(true);
            textUR3.SetActive(false);
            settingsTextUR3.SetActive(false);
            setUr10MinMax();
        }
        else { 
            RosPublisherExample.instance.pubRobotName("ur3");
            textUR3.SetActive(true);
            settingsTextUR3.SetActive(true);
            textUR10.SetActive(false);
            settingsTextUR10.SetActive(false);
            setUr3MinMax();
        }
        uploadTextMinMax();
    }

    void setUr3MinMax(){
        posMaxXY = 0.6f;
        posMinXY = -0.6f;
        maxAddXY = 0.8f;
        posMaxZ = 1f;
        posMinZ = 0.8f;
        for (int i = 0; i < input.Count; i++) {
            input[i].text = basicUr3Values[i].ToString();
        }
    }
    void setUr10MinMax(){
        posMaxXY = 1.1f;
        posMinXY = -1.1f;
        maxAddXY = 1.4f;
        posMaxZ = 1.5f;
        posMinZ = 0.5f;
        for (int i = 0; i < input.Count; i++) {
            input[i].text = basicUr10Values[i].ToString();
        }
    }
    void uploadTextMinMax(){
        textPosMinX.text = posMinXY.ToString()+"<=";
        textPosMinY.text = posMinXY.ToString()+"<=";
        textPosMaxX.text = "<="+posMaxXY.ToString();
        textPosMaxY.text = "<="+posMaxXY.ToString();
        textPosMinZ.text = posMinZ.ToString()+"<=";
        textPosMaxZ.text = "<="+posMaxZ.ToString();
        textMaxAddXY.text = "<="+maxAddXY.ToString();
        goodX = true;
        goodY = true;
        goodZ = true;
        goodRot = true;
    }

    public void ReadX(string input){
        goodX = true;
        x = Read(input,posMinXY,posMaxXY);
        if(float.IsNaN(x)){
            goodX = false;
            crossX.SetActive(true);
        }
        else{
            rotMinMax();
            crossX.SetActive(false);
        }
    }
    public void ReadY(string input){
        goodY = true;
        y = Read(input,posMinXY,posMaxXY);
        if(float.IsNaN(y)){
            goodY = false;
            crossY.SetActive(true);
        }
        else{
            rotMinMax();
            crossY.SetActive(false);
        }
    }
    public void ReadZ(string input){
        goodZ = true;
        z = Read(input,posMinZ,posMaxZ);
        if(float.IsNaN(z)){
            goodZ = false;
            crossZ.SetActive(true);
        }
        else{
            rotMinMax();
            crossZ.SetActive(false);
        }
    }

    public void ReadRot(string input){
        goodRot = true;
        rotZ = Read(input,rotMin,rotMax);
        if(float.IsNaN(rotZ)){
            goodRot = false;
            crossRot.SetActive(true);
        }
        else{
            crossRot.SetActive(false);
        }
    }

    float Read(string inpStr, float min, float max){
        float input;
        if (float.TryParse(inpStr, out input)){
            checkAddXY();
            if (input >= min && input <= max ){
                //check if we can unblock the exit button
                canPlay();
                return input;
            }
        }
        //block the start button
        exitButton.interactable = false;
        return float.NaN;
    }

    //check if we can unblock the start button
    void canPlay(){
        if(goodX && goodY && goodZ && goodRot && goodAddXY){
            exitButton.interactable = true;
        }
    }

    void checkAddXY(){
        float add = Mathf.Abs(x) + Mathf.Abs(y);
        textAdd.text = add.ToString();
        if(add <= maxAddXY){
            crossAdd.SetActive(false);
            goodAddXY = true;
        }
        else{
            goodAddXY = false;
            crossAdd.SetActive(true);
            exitButton.interactable = false;
        }
    }

    void rotMinMax(){
        if (x > 0 && y > 0){
            print("x>0 et y >0");
            rotMin = 0f;
            rotMax = 90f;
        }
        else if (x < 0 && y > 0){
            print("x<0 et y >0");
            rotMin = 90f;
            rotMax = 180f;
        } 
        else if (x < 0 && y < 0){
            print("x<0 et y <0");
            rotMin = 90f;
            rotMax = 270f;
        } 
        else if (x > 0 && y < 0){
            print("x>0 et y <0");
            rotMin = -90f;
            rotMax = 0f;
            inverse = true;
        } 
        else if (x == 0 && y > 0){
            rotMin = 0f;
            rotMax = 180f;
        } 
        else if (x == 0 && y < 0){
            rotMin = 180f;
            rotMax = 360f;
        } 
        else if (x > 0 && y == 0){
            rotMin = -90f;
            rotMax = 90f;
        } 
        else if (x < 0 && y == 0){
            rotMin = 90f;
            rotMax = 270f;
        }
        // case x = 0 and y = 0
        else{
            rotMin = 0;
            rotMax = 360;
        }
        textMinRot.text = rotMin.ToString() + "<=";
        textMaxRot.text = "<=" + rotMax.ToString();
        uploadInputRot(rotMin, rotMax);
    }

    void uploadInputRot(float min,float max){
        if(x == 0f && y == 0f){
            input[3].text = "0";
        }
        else if (x == 0f || y == 0f ){
            input[3].text = (max-90f).ToString();
        }
        else if(Mathf.Abs(x)>=Mathf.Abs(y)){
            if(inverse){
                inverse = false;
                input[3].text = max.ToString();
            }
            input[3].text = min.ToString();
        }
        else {
            input[3].text = max.ToString();
            if(inverse){
                inverse = false;
                input[3].text = min.ToString();
            }
        }
    }
}
