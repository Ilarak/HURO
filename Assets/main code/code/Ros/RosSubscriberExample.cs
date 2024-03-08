using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;

using RosList3float = RosMessageTypes.UnityRoboticsDemo.List3floatMsg;
using RosList6float= RosMessageTypes.UnityRoboticsDemo.List6floatMsg;
using RosFloat = RosMessageTypes.UnityRoboticsDemo.FloatMsg;
using RosBool = RosMessageTypes.UnityRoboticsDemo.BoolMsg;

using TMPro;
using System;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

public class RosSubscriberExample : MonoBehaviour
{
    public static RosSubscriberExample instance;
    GameObject hand;

    public static List<float> timestamp = new List<float>();
    public static List<List<float>> force = new List<List<float>>();
    public static List<List<float>> tcp_pos = new List<List<float>>();
    public static List<List<float>> tcp_vel = new List<List<float>>();
    public static List<List<float>> joint_pos = new List<List<float>>();
    public static List<List<float>> joint_vel = new List<List<float>>();
    //if we need the position of the robot depending on the first position :
    //public static List<List<float>> tcp_pos_sub = new List<List<float>>();


    public static float timestamp_UI;
    public static List<float> tcp_pos_UI = new List<float>();
    public static List<float> tcp_vel_UI = new List<float>();
    public static List<float> joint_pos_UI = new List<float>();
    public static List<float> joint_vel_UI = new List<float>();
    public static List<float> force_UI = new List<float>();

    public List<float> jointPosSim = new List<float>(); 

    public List<float> calibVal = new List<float>();

    public bool validPosSim = false;

    public bool recording = false;
    public bool exerciceRunning = false;
    public bool simRunning = false;
    public bool calib = false;

    public bool simPosControl = false;

    public static Vector3 posHand;

    void OnEnable(){
        ROSConnection.GetOrCreateInstance().Subscribe<RosList3float>("tcp_pos", Tcp_pos);
        ROSConnection.GetOrCreateInstance().Subscribe<RosList3float>("tcp_pos_sub", Tcp_pos_sub);
        ROSConnection.GetOrCreateInstance().Subscribe<RosList3float>("tcp_vel", Tcp_vel);
        ROSConnection.GetOrCreateInstance().Subscribe<RosList6float>("joint_pos", Joint_pos);
        ROSConnection.GetOrCreateInstance().Subscribe<RosList6float>("joint_vel", Joint_vel);
        ROSConnection.GetOrCreateInstance().Subscribe<RosFloat>("timestamp", Timestamp);
        ROSConnection.GetOrCreateInstance().Subscribe<RosList3float>("force_sensor", Force_sensor);
        ROSConnection.GetOrCreateInstance().Subscribe<RosList3float>("save_calib_val", Calib_val);
        ROSConnection.GetOrCreateInstance().Subscribe<RosList6float>("joint_pos_sim", Joint_pos_sim);
        ROSConnection.GetOrCreateInstance().Subscribe<RosBool>("pos_sim_valid", Pos_sim_valid);

        instance = this;
    }

    public IEnumerator findHand(){
        hand = GameObject.FindWithTag("hand");
        while(hand == null){
            hand = GameObject.FindWithTag("hand");
            yield return null;
        yield return null;
        }
    }

    void Tcp_pos(RosList3float tcpPosMessage){
        if (recording){
            tcp_pos_UI = new List<float>(tcpPosMessage.list);
        }
        if (exerciceRunning){
            tcp_pos.Add(new List<float>(tcpPosMessage.list));
        }
    }

    // tcp pos substract to the tcp origin pos
    void Tcp_pos_sub(RosList3float tcpPosSubMessage){
        if (exerciceRunning){
            // tcpPosSubMessage is in the tcp referenciel. We want it in the unity ref
            if(hand != null){
                posHand = new Vector3(-tcpPosSubMessage.list[1], tcpPosSubMessage.list[2], tcpPosSubMessage.list[0]);
                hand.transform.position = posHand;
            }
            //tcp_pos_sub.Add(new List<float>(tcpPosSubMessage.list));
        }
    }

    void Tcp_vel(RosList3float tcpVelMessage){
        if (recording){
            tcp_vel_UI = new List<float>(tcpVelMessage.list);
        }
        if (exerciceRunning){
            tcp_vel.Add(new List<float>(tcpVelMessage.list));
        }
    }

    void Joint_pos(RosList6float jointPosMessage){
        if (recording){
            joint_pos_UI = new List<float>(jointPosMessage.list);
        }
        if (exerciceRunning){
            joint_pos.Add(new List<float>(jointPosMessage.list));
        }
    }

    void Joint_vel(RosList6float jointVelMessage){
        if(recording){
            joint_vel_UI = new List<float>(jointVelMessage.list);
        }
        if (exerciceRunning){
            joint_vel.Add(new List<float>(jointVelMessage.list));
        }
    }

    void Timestamp(RosFloat timestampMessage){
        if(recording){
            timestamp_UI = timestampMessage.value;
        }
        if(exerciceRunning){
            timestamp.Add(timestampMessage.value);
        }
    }

    void Force_sensor(RosList3float forceSensorMessage){
        if(recording){
            force_UI = new List<float>(forceSensorMessage.list);
        }
        if(exerciceRunning){
            force.Add(new List<float>(forceSensorMessage.list));
        }
    }

    void Calib_val(RosList3float calibValMessage){
        if(calib){
            calibVal = new List<float>(calibValMessage.list);
        }
        
    }

    void Joint_pos_sim(RosList6float posSimMessage){
        if(simRunning){
            simPosControl = true;
            jointPosSim = new List<float>(posSimMessage.list);
        }
    }
    void Pos_sim_valid(RosBool validMessage){
        if(simRunning){
            print("VALIDE : " + validMessage.value);
            validPosSim = validMessage.value;
        }
    }
}
