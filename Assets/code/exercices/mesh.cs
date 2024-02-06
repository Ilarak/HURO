using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeMesh : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Mesh[] meshToChange;

    public GameObject fruit;
    void Start()
    {
        MeshFilter meshObj = fruit.GetComponent<MeshFilter>();
        meshObj.mesh = meshToChange[2];
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    /*void changeMeshFruit(GameObject fruit){
        Mesh meshObj = fruit.GetComponent<Mesh>();
        meshObj.mesh = meshToChange[2];
    }*/
}
