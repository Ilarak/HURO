using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ControllerRobot : MonoBehaviour
{
    public Transform shoulder;
    
    public Transform upper;
    
    public Transform forearm;
    
    public Transform wrist_1;
    
    public Transform wrist_2;

    public Transform wrist_3;

    public SliderControl sliderControl;

    public MoveHand moveHand;

    public List<float> list = new List<float>();

    bool simPosControl = false;
    bool simAngularControl = false;

    void Awake(){
        for(int i = 0 ; i < 6; i++){
            list.Add(0.0f);
        }
    }

    void Update(){
        if (RosSubscriberExample.instance.simPosControl){
            RosSubscriberExample.instance.simPosControl = false;
            list = new List<float>(RosSubscriberExample.instance.jointPosSim);
            simPosControl = true;
        }
        else if (RosPublisherExample.instance.simAngularControl){
            RosPublisherExample.instance.simAngularControl = false;
            if(RosSubscriberExample.instance.validPosSim){
                list = new List<float>(sliderControl.pos);
                simAngularControl = true;
            }
        }
        updateRobot(list);
    }

    public void updateRobot(List<float> jointPosSim){
        float angle = Mathf.Repeat(jointPosSim[0],Mathf.PI*2f);
        shoulder.localEulerAngles = new Vector3(0,-angle,0);
        shoulder.localEulerAngles = new Vector3(0,-jointPosSim[0]*Mathf.Rad2Deg,0);
        upper.localEulerAngles = new Vector3(-jointPosSim[1]*Mathf.Rad2Deg,0,-90);
        forearm.localEulerAngles = new Vector3(0,-jointPosSim[2]*Mathf.Rad2Deg,0);
        wrist_1.localEulerAngles = new Vector3(0,-jointPosSim[3]*Mathf.Rad2Deg,0);
        wrist_2.localEulerAngles = new Vector3(-jointPosSim[4]*Mathf.Rad2Deg,0,-90);
        wrist_3.localEulerAngles = new Vector3(-jointPosSim[5]*Mathf.Rad2Deg,0,90);


        if(simPosControl){
            List<float> list = new List<float>();
            list.Add(shoulder.localEulerAngles.y);
            list.Add(upper.localEulerAngles.x);
            list.Add(forearm.localEulerAngles.y);
            list.Add(wrist_1.localEulerAngles.y);
            list.Add(wrist_2.localEulerAngles.x);
            list.Add(wrist_3.localEulerAngles.x);
            sliderControl.editSlider(list);
            simPosControl = false;
        }

        if(simAngularControl){
            moveHand.sliderMoveHand();
            simAngularControl = false;
        }
    } 
}
