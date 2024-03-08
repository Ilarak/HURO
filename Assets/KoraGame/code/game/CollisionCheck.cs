using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheck : MonoBehaviour
{
    // every frame of the collision OnTriggerStay
    // end of the collision OnTriggerExit 
    // first collison 
    RosPublisherExample publisher;
    void Start() {
        GameObject ObjRosControllers = GameObject.Find("RosControllers");
        GameObject ObjRosPublisher = ObjRosControllers.transform.Find("RosPublisher").gameObject;
        publisher = ObjRosPublisher.GetComponent<RosPublisherExample>();
    }

    public static bool reachSphere = false;
    void OnTriggerEnter(Collider other){
        if (other.gameObject.tag == "wall"){
            print("collision with the wall");
            publisher.pubCollision(true);
        }
        if (other.gameObject.tag == "objToReach"){
            reachSphere = true;
        }
    }
    void OnTriggerStay(Collider other){
        if (other.gameObject.tag == "wall"){
            print("collision with the wall");
            publisher.pubCollision(true);
        }
    }
    void OnTriggerExit(Collider other){
        if (other.gameObject.tag == "wall"){
            publisher.pubCollision(false);
        }
    }
}
