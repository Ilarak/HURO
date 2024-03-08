using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class singletonPlayer : MonoBehaviour{
    private static singletonPlayer instance;

    public static bool isDestroying = false;
    public static bool firstInstance = true;

    void Awake(){
        if (instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            firstInstance = false;
            //the game object is destryoing
            isDestroying = true;
            Destroy(gameObject);
        }
    }
}
