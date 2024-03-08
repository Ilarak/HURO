using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveHand : MonoBehaviour{
    public Rigidbody end_effector;

    public GameObject Hand;

    public GameObject positionToGoSlider;

    public float stepPos, stepRot;

    public List<KeyCode> lastStuckKeyPress = new List<KeyCode>();

    Vector3 lastTransPos = new Vector3(0,0,0);
    Quaternion lastTransRot = Quaternion.Euler(0, 0, 0);

    Vector3 posIni,posIniHand;
    Quaternion rotIni,rotIniHand;

    public SliderControl sliderControl;

    bool pub = true;

    public GameObject wrist3;

    void Awake(){
        //Initialize the first position of the object to 0
        // use to calibrate the zero position
        posIni = end_effector.position;
        rotIni = end_effector.rotation;

        stepPos = 0.0005f;
        stepRot = 20f;

        // same position between the ref object of the wrist and the hand
        positionToGoSlider.transform.position = Hand.transform.position;
        positionToGoSlider.transform.rotation = Hand.transform.rotation;

        //same position between the axe of the end effector and the wrist
        //end_effector.transform.position = wrist3.transform.position;
    }

    void Start(){
        StartCoroutine(posInitial());
    }

    IEnumerator posInitial(){
        while (posIni == end_effector.position){
            pub = true;
            yield return null;
        }     
        // use to reset the normal position
        posIniHand = Hand.transform.position;
        rotIniHand = Hand.transform.rotation;
        sliderControl.posIni();
    }
    public void changeHeigtRobot(){
        pub = true;
    }

    void Update(){
        // if the hand is in a valid position
        if (RosSubscriberExample.instance.validPosSim){
            //clear the list that allow not to move in a specific way if the robot cant go to the location
            lastStuckKeyPress.Clear();
            sliderControl.lastSliderValUse.Clear();
            //if a key is press, move the hand
            positionMove(stepPos);
            rotationMove(stepRot);
        }
        else {
            //if a key is press, move the hand
            positionMove(stepPos*20);
            rotationMove(stepRot*20);
        }
        if(pub){
            publishPosition();
            pub = false;
        }
        
    }
    void publishPosition(){
        Vector3 position = end_effector.position;
        Quaternion rotation = end_effector.rotation;
        RosPublisherExample.instance.pubPosRobotSim(position,rotation,posIni);
    }

    void positionMove(float stepPos){
        if (Input.GetKey(KeyCode.W) && !lastStuckKeyPress.Contains(KeyCode.W)){
            lastStuckKeyPress.Add(KeyCode.W);
            lastTransPos = new Vector3(0,0,-stepPos);
        }
        else if (Input.GetKey(KeyCode.S)&& !lastStuckKeyPress.Contains(KeyCode.S)){
            lastStuckKeyPress.Add(KeyCode.S);
            lastTransPos = new Vector3(0,0,stepPos);
        }
        else if (Input.GetKey(KeyCode.A)&& !lastStuckKeyPress.Contains(KeyCode.A)){
            lastStuckKeyPress.Add(KeyCode.A);
            lastTransPos = new Vector3(stepPos,0,0);
        }
        else if (Input.GetKey(KeyCode.D)&& !lastStuckKeyPress.Contains(KeyCode.D)){
            lastStuckKeyPress.Add(KeyCode.D);
            lastTransPos = new Vector3(-stepPos,0,0);
        }
        else if (Input.GetKey(KeyCode.LeftShift)&& !lastStuckKeyPress.Contains(KeyCode.LeftShift)){
            lastStuckKeyPress.Add(KeyCode.LeftShift);
            lastTransPos = new Vector3(0,-stepPos,0);
        }
        else if (Input.GetKey(KeyCode.Space)&& !lastStuckKeyPress.Contains(KeyCode.Space)){
            lastStuckKeyPress.Add(KeyCode.Space);
            lastTransPos = new Vector3(0,stepPos,0);
        }
        else{
            return;
        }
        pub = true;
        Hand.transform.position += lastTransPos;
    }

    void rotationMove(float stepRot){
        if (Input.GetKey(KeyCode.Keypad8)&& !lastStuckKeyPress.Contains(KeyCode.Keypad8)){
            lastStuckKeyPress.Add(KeyCode.Keypad8);
            lastTransRot = Quaternion.Euler(0, 0, stepRot * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.Keypad5) && !lastStuckKeyPress.Contains(KeyCode.Keypad5)){
            lastStuckKeyPress.Add(KeyCode.Keypad5);
            lastTransRot = Quaternion.Euler(0, 0, -stepRot * Time.deltaTime);   
        }
        else if (Input.GetKey(KeyCode.Keypad4)&& !lastStuckKeyPress.Contains(KeyCode.Keypad4)){
            lastStuckKeyPress.Add(KeyCode.Keypad4);
            lastTransRot = Quaternion.Euler(-stepRot * Time.deltaTime, 0, 0);
        }
        else if (Input.GetKey(KeyCode.Keypad6)&& !lastStuckKeyPress.Contains(KeyCode.Keypad6)){
            lastStuckKeyPress.Add(KeyCode.Keypad6);
            lastTransRot = Quaternion.Euler(stepRot * Time.deltaTime, 0, 0);
        }
        else if (Input.GetKey(KeyCode.Keypad7)&& !lastStuckKeyPress.Contains(KeyCode.Keypad7)){
            lastStuckKeyPress.Add(KeyCode.Keypad7);
            lastTransRot = Quaternion.Euler(0, -stepRot * Time.deltaTime, 0);
        }
        else if (Input.GetKey(KeyCode.Keypad9)&& !lastStuckKeyPress.Contains(KeyCode.Keypad9)){
            lastStuckKeyPress.Add(KeyCode.Keypad9);
            lastTransRot = Quaternion.Euler(0, stepRot * Time.deltaTime, 0);
        }
        else{
            return;
        }
        pub = true;
        Hand.transform.rotation *= lastTransRot;
    }

    public void goStartPos(){
        lastStuckKeyPress.Clear();
        Hand.transform.position = posIniHand;
        Hand.transform.rotation = rotIniHand;
        pub = true;
        StartCoroutine(CoroutineGoStartPos());
    }

    IEnumerator CoroutineGoStartPos(){
        yield return new WaitForSeconds(0.1f);
        pub = true;
    }

    public void sliderMoveHand(){
        Hand.transform.position = positionToGoSlider.transform.position;
        Hand.transform.rotation = positionToGoSlider.transform.rotation;
    }
}
