using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMesh : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Mesh[] meshToChange;
    private int random = 0;
    public bool isStrawberry = false;
    
    public void changeMeshFruit(GameObject fruit){
        if (fruit.tag == "strawberry"){
            random = meshToChange.Length-1;
            isStrawberry = true;
        }
        else{
            MeshFilter meshObj = fruit.GetComponent<MeshFilter>();
            //Choose a random fruit mesh
            random = Random.Range(1,meshToChange.Length-1);
            meshObj.mesh = meshToChange[random];
            isStrawberry = false;
        }
        
    }

    public void changeBasketMesh(GameObject fruit){
        MeshFilter meshObj = fruit.GetComponent<MeshFilter>();
        meshObj.mesh = meshToChange[random];
    }
}
