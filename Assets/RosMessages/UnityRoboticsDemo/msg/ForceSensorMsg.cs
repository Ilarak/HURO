//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.UnityRoboticsDemo
{
    [Serializable]
    public class ForceSensorMsg : Message
    {
        public const string k_RosMessageName = "unity_robotics_demo_msgs/ForceSensor";
        public override string RosMessageName => k_RosMessageName;

        public float[] force;

        public ForceSensorMsg()
        {
            this.force = new float[3];
        }

        public ForceSensorMsg(float[] force)
        {
            this.force = force;
        }

        public static ForceSensorMsg Deserialize(MessageDeserializer deserializer) => new ForceSensorMsg(deserializer);

        private ForceSensorMsg(MessageDeserializer deserializer)
        {
            deserializer.Read(out this.force, sizeof(float), 3);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.force);
        }

        public override string ToString()
        {
            return "ForceSensorMsg: " +
            "\nforce: " + System.String.Join(", ", force.ToList());
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
