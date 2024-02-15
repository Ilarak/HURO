using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.UnityRoboticsDemo;
using System.Collections.Generic;
public class RosPublisherExample : MonoBehaviour
{
    public static RosPublisherExample instance;
    ROSConnection ros;
    private string check_collisions_topic = "check_collisions";
    private string c_value_topic = "c_value";
    private string type_control_topic = "type_control";
    private string plane_of_work_topic = "plane_of_work";
    private string start_calib_topic = "start_calib";
    private string save_tcp_initial_pos_topic = "save_tcp_initial_pos";
    private string go_joint_position_topic = "go_joint_position";
    private string run_ex_topic = "run_ex";
    private string opposing_force_topic = "opposing_force_topic";
    public float publishMessageFrequency = 0.0005f;

    BoolMsg collisionMsg = new BoolMsg();
    FloatMsg cValueMsg = new FloatMsg();
    StringMsg typeControlMsg = new StringMsg();
    IntMsg planeOfWorkMsg = new IntMsg();
    BoolMsg startCalibMsg = new BoolMsg();
    BoolMsg saveTcpInitialPosMsg = new BoolMsg();
    List6floatMsg goJointPosMsg = new List6floatMsg();
    BoolMsg startExMsg = new BoolMsg();
    FloatMsg opposingForceMsg = new FloatMsg();

    public static float c, opposingForce;
    public static int axis;


    void OnEnable(){    
        instance = this;
    }
    void Start(){
        // start the ROS connection
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<BoolMsg>(check_collisions_topic);
        ros.RegisterPublisher<FloatMsg>(c_value_topic);
        ros.RegisterPublisher<StringMsg>(type_control_topic);
        ros.RegisterPublisher<IntMsg>(plane_of_work_topic);
        ros.RegisterPublisher<BoolMsg>(start_calib_topic);
        ros.RegisterPublisher<BoolMsg>(save_tcp_initial_pos_topic);
        ros.RegisterPublisher<List6floatMsg>(go_joint_position_topic);
        ros.RegisterPublisher<BoolMsg>(run_ex_topic);
        ros.RegisterPublisher<IntMsg>(opposing_force_topic);
    }
    //functions to publish data-------------------------------
    public void pubCollision(bool collision){
        collisionMsg.value = collision;
        ros.Publish(check_collisions_topic, collisionMsg);
    }

    public void pubCValue(float c_val){
        cValueMsg.value = c_val;
        c=c_val;
        ros.Publish(c_value_topic,cValueMsg);
    }

    public void pubTypeControl(string type){
        typeControlMsg.value = type;
        ros.Publish(type_control_topic,typeControlMsg);
    }

    public void pubPlaneOfWork(int plane){
        axis = plane;
        planeOfWorkMsg.value = plane;
        ros.Publish(plane_of_work_topic,planeOfWorkMsg);
    }

    public void pubStartCalib(bool startCalib){
        startCalibMsg.value = startCalib;
        ros.Publish(start_calib_topic,startCalibMsg);
    }

    public void pubSaveTCPInitialPos(bool value){
        saveTcpInitialPosMsg.value = value;
        ros.Publish(save_tcp_initial_pos_topic,saveTcpInitialPosMsg);
    }

    public void pubGoJointPosition(List<float> pos){
        goJointPosMsg.list = pos.ToArray();
        ros.Publish(go_joint_position_topic,goJointPosMsg);
    }

    public void pubStartEx(bool startEx){
        startExMsg.value = startEx;
        ros.Publish(run_ex_topic,startExMsg);
    }

    public void pubOpposingForce(float opposingForceVal){
        opposingForce = opposingForceVal;
        opposingForceMsg.value = opposingForceVal;
        ros.Publish(opposing_force_topic,opposingForceMsg);
    }


}
