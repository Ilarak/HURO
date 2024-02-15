//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.UnityRoboticsDemo
{
    [Serializable]
    public class TCPVelMsg : Message
    {
        public const string k_RosMessageName = "unity_robotics_demo_msgs/TCPVel";
        public override string RosMessageName => k_RosMessageName;

        public float[] vel;

        public TCPVelMsg()
        {
            this.vel = new float[3];
        }

        public TCPVelMsg(float[] vel)
        {
            this.vel = vel;
        }

        public static TCPVelMsg Deserialize(MessageDeserializer deserializer) => new TCPVelMsg(deserializer);

        private TCPVelMsg(MessageDeserializer deserializer)
        {
            deserializer.Read(out this.vel, sizeof(float), 3);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.vel);
        }

        public override string ToString()
        {
            return "TCPVelMsg: " +
            "\nvel: " + System.String.Join(", ", vel.ToList());
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod]
#endif
        public static void Register()
        {
            MessageRegistry.Register(k_RosMessageName, Deserialize);
        }
    }
}
