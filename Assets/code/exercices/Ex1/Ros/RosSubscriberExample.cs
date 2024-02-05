using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosPosition = RosMessageTypes.UnityRoboticsDemo.UnityPositionMsg;
using TMPro;
using System;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

public class RosSubscriberExample : MonoBehaviour
{
    public GameObject sphere;

    public Rigidbody sphereRig;

    public static List<float> time = new List<float>();
    public static List<List<float>> force = new List<List<float>>();

    public static List<List<float>> pos_art = new List<List<float>>();

    public static List<List<float>> vel_art = new List<List<float>>();

    public static List<List<float>> pos = new List<List<float>>();

    public static List<List<float>> vel = new List<List<float>>();


    public dataSave save;


    void Start()
    {
        ROSConnection.GetOrCreateInstance().Subscribe<RosPosition>("robot_position", PositionChange);
    }

    void PositionChange(RosPosition positionMessage){
        //sphere.transform.position = new Vector3(positionMessage.pos_x, positionMessage.pos_y, positionMessage.pos_z);
        time.Add(positionMessage.times);
        force.Add(new List<float>(positionMessage.force));
        pos_art.Add(new List<float>(positionMessage.pos_art));
        vel_art.Add(new List<float>(positionMessage.vel_art));
        pos.Add(new List<float>(positionMessage.pos));        

        vel.Add(new List<float>(positionMessage.vel));
    }

    public void OnDisable(){
        //save.SaveCVS();
    }
}
