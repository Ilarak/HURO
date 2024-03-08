using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosHouse : MonoBehaviour
{
    public GameObject house;
    public GameObject environment;
    public SliderControl sliderControl;
    // Start is called before the first frame update
    void OnEnable(){
        float z = PlayerPrefs.GetFloat("simX");
        float x = PlayerPrefs.GetFloat("simY");
        float y = -PlayerPrefs.GetFloat("simZ");
        float rotY = PlayerPrefs.GetFloat("simRotZ");
        Vector3 pos = new Vector3(x,y,z);
        Quaternion rot = Quaternion.Euler(0,rotY,0);
        
        environment.transform.position = pos;
        house.transform.rotation = rot;

        sliderControl.heigthStartBanana(z);
        
    }

    public void changeHeigtHouse(float heigth){
        Vector3 pos = new Vector3(environment.transform.position.x,-heigth,environment.transform.position.z);
        environment.transform.position = pos;
    }
}
