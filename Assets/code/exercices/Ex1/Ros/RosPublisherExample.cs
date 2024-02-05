using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.UnityRoboticsDemo;

/// <summary>
///
/// </summary>
public class RosPublisherExample : MonoBehaviour
{
    ROSConnection ros;
    public string topicName = "in_collision";

    // The game object
    //public GameObject sphere;
    // Publish the sphere's position and rotation every N seconds
    public float publishMessageFrequency = 0.0005f;

    // Used to determine how much time has elapsed since the last message was published
    private float timeElapsed;

    //public CollisionCheck collisionCheck;
    UnityCollisionMsg collisionMsg = new UnityCollisionMsg();

    void Start()
    {
        // start the ROS connection*/
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<UnityCollisionMsg>(topicName);
    }

    private void Update()
    {
        collisionMsg.collision = CollisionCheck.collision;
        ros.Publish(topicName, collisionMsg);
    }
}
