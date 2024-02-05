using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheck : MonoBehaviour
{
    // every frame of the collision OnTriggerStay
    // end of the collision OnTriggerExit 
    // first collison 

    public static bool collision = false;
    public static bool reachSphere = false;
    void OnTriggerEnter(Collider other){
        if (other.gameObject.tag == "wall"){
            print("collision avec le mur");
            collision = true;
        }
        if (other.gameObject.tag == "VisibleBall"){
            print("collision");
            reachSphere = true;
        }
    }
    void OnTriggerStay(Collider other){
        if (other.gameObject.tag == "wall"){
            collision = true;
        }
    }
    void OnTriggerExit(Collider other){
        if (other.gameObject.tag == "wall"){
            collision = false;
        }
    }
    public bool IsInCollision(){
        return collision;
    }
}
