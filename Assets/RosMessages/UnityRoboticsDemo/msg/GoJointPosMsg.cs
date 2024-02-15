//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.UnityRoboticsDemo
{
    [Serializable]
    public class GoJointPosMsg : Message
    {
        public const string k_RosMessageName = "unity_robotics_demo_msgs/GoJointPos";
        public override string RosMessageName => k_RosMessageName;

        public float[] pos;

        public GoJointPosMsg()
        {
            this.pos = new float[6];
        }

        public GoJointPosMsg(float[] pos)
        {
            this.pos = pos;
        }

        public static GoJointPosMsg Deserialize(MessageDeserializer deserializer) => new GoJointPosMsg(deserializer);

        private GoJointPosMsg(MessageDeserializer deserializer)
        {
            deserializer.Read(out this.pos, sizeof(float), 6);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.pos);
        }

        public override string ToString()
        {
            return "GoJointPosMsg: " +
            "\npos: " + System.String.Join(", ", pos.ToList());
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
