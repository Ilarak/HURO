using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class singletonRos : MonoBehaviour{
    private static singletonRos instance;

    GameObject ObjBaseLink;
    GameObject ObjBase;

    void Awake(){
        if (instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
            StartCoroutine(dontDestroyBaseLink());
        }
        else {
            Destroy(gameObject);
        }
    }
    IEnumerator dontDestroyBaseLink(){
        ObjBaseLink = GameObject.Find("base_link_inertia");
        while (ObjBaseLink == null){
            ObjBaseLink = GameObject.Find("base_link_inertia");
            yield return null;
        }
        ObjBase = GameObject.Find("base");
        while (ObjBaseLink == null){
            ObjBaseLink = GameObject.Find("ObjBase");
            yield return null;
        }
        DontDestroyOnLoad(ObjBaseLink);
        DontDestroyOnLoad(ObjBase);
        yield return null;
    }
}
