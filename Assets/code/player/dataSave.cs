using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

//this class must always be keeping
public class dataSave : MonoBehaviour
{
    public Player player;

    private string currentEx;

    string sessionSaveStr;

    //value of the session
    float averageForce,distance,time;
    int weigthForce;

    int nbExSession=0;

    void Start(){
        player.nbSession+=1;
        
        // if it is the first session, initialize the save of the resumeSession Save
        if (player.nbSession==1){
            sessionSaveStr = ";time;distance;average force";
        }
        else {
            sessionSaveStr=readCSV()+"\nsession"+player.nbSession+" :";
        }
    }

    public string readCSV(){
        //path to read the csv
        string path = "/"+player.playerName;
        //Check the folder to save
        #if UNITY_EDITOR
            var folder = Application.streamingAssetsPath+path;
        #else
            var folder = Application.persistentDataPath+path;
        #endif

        if (Directory.Exists(folder)){
            string data_string ="";

            StreamReader strReader = new StreamReader(folder+"/resumeSession.csv");
            bool firstLigne = true;
            while(true){
                string reader = strReader.ReadLine();
                //end of the file
                if(reader == null){
                    strReader.Close();
                    return data_string;
                }
                // if it is the first ligne, dont put a heading
                if (firstLigne){
                    firstLigne = false;
                    data_string +=reader;
                }
                else {
                    data_string +="\n"+ reader;
                }
            }
        }
        return "";
        
    }
    public void initializePlayer(Player playerIni){
        player = playerIni;
    }

    public void SaveEx(){
        // recover the current exercice name
        currentEx = PlayerPrefs.GetString("currentEx");
        nbExSession += 1;
        string fileNameEx ="";
        if (currentEx=="ex1"){
            player.nbEx1+=1;
            fileNameEx= currentEx+"_"+player.nbEx1.ToString();
        }  

        sessionSaveStr+="\n"+fileNameEx;

        saveCSV(fileNameEx);
        saveCSV("global_data");
    }

    public void SaveSession(){
        saveCSV("resumeSession");
    }

    public void saveCSV(string fileName){
        string str ="";
        if (fileName =="global_data"){
            str = exGlobalString();
        }
        else if (fileName == "resumeSession"){
            str = sessionString();
        }
        else if (currentEx == "ex1"){
            str = ex1String();
        }
        
        string filePath = fineName(fileName);

        using(StreamWriter writer = new StreamWriter(filePath, false)){
            writer.Write(str);
        }
    }

    public string exGlobalString(){
        string str = "time";
        float timeEx=0;
        foreach(var var in RosSubscriberExample.timestamp){
            timeEx+=var;
            str += ";"+var.ToString();
        }
        str+="\nforce";
        float averageForceEx=0;
        foreach (var var in RosSubscriberExample.force){
            averageForceEx += calculateNorm(var);
            str += ";"+var[0].ToString()+","+var[1].ToString()+","+var[2].ToString();
        }
        str+="\nposHand";
        float distanceEx=0;
        foreach (var var in RosSubscriberExample.tcp_pos){
            distanceEx +=calculateNorm(var);
            str += ";"+var[0].ToString()+","+var[1].ToString()+","+var[2].ToString();
        }
        str+="\nvelHand";
        foreach (var var in RosSubscriberExample.tcp_vel){
            float normeVel= calculateNorm(var);
            if (normeVel>player.maxVel){
                player.maxVel = normeVel;
            }
            str += ";"+var[0].ToString()+","+var[1].ToString()+","+var[2].ToString();
        }
        str += "\nposArt";
        foreach(var var in RosSubscriberExample.joint_pos){
            str += ";"+var[0].ToString()+","+var[1].ToString()+","+var[2].ToString()+","+var[3].ToString()+","+var[4].ToString()+","+var[5].ToString();
        }
        str+="\nvelArt";
        foreach (var var in RosSubscriberExample.joint_vel){
            str += ";"+var[0].ToString()+","+var[1].ToString()+","+var[2].ToString()+","+var[3].ToString()+","+var[4].ToString()+","+var[5].ToString();
        }

        str+="\nc;"+RosPublisherExample.c;
        str+="\nopposingForce;"+RosPublisherExample.opposingForce;
        str+="\nopposingForce;";
        if (RosPublisherExample.axis == 1){str+="x-z";}
        else if (RosPublisherExample.axis == 2){str+="y-z";}
        else if (RosPublisherExample.axis == 3){str+="x-y";}
        else {str+="3D";}

        //adds the average values to the session save data
        weigthForce += RosSubscriberExample.force.Count;
        if (RosSubscriberExample.force.Count!=0){
            sessionSaveStr += ";"+timeEx+";"+distanceEx+";"+averageForceEx/RosSubscriberExample.force.Count;
        }
        else {
            sessionSaveStr += ";"+timeEx+";"+distanceEx+";0";
        }
        averageForce+=averageForceEx;
        distance+=distanceEx;
        time+=timeEx;
        
        //the exercice is finished, reset the lists
        rosDataToZero();
        return str;
    }

    public void rosDataToZero(){
        RosSubscriberExample.timestamp.Clear();
        RosSubscriberExample.force.Clear();
        RosSubscriberExample.tcp_pos.Clear();
        RosSubscriberExample.tcp_vel.Clear();
        RosSubscriberExample.joint_pos.Clear();
        RosSubscriberExample.joint_vel.Clear(); 
    }

    public string sessionString(){
        float av = 0;
        if (weigthForce+player.weigthForce != 0 ){
            av = (player.averageForce*player.weigthForce + averageForce)/(weigthForce+player.weigthForce);
        }
        sessionSaveStr+="\nresume;"+time+";"+distance+";"+av+"\n";

        // we add 2 averages
        player.averageForce = av;
        player.timeTot+=time;
        player.totDistance +=distance;
        player.weigthForce += weigthForce; // we save the weigth of the force allowing to add later two average

        return sessionSaveStr;
    }
    

    public string ex1String(){
        string str = "score;"+ mainEx1.gamePoint + "\ntime between 2 objectives";
        foreach(var var in mainEx1.timeBetween2Scores){
            str +=";"+var;
        }
        return str;
    } 

    public string fineName(string fileName){
        //path to the resume session save
        string path;
        if(fileName=="resumeSession"){
            path = "/"+player.playerName+"/";
        }
        //path to the exercices save
        else{
            path = "/"+player.playerName+"/session"+player.nbSession.ToString()+"/Ex"+nbExSession+"_"+currentEx;
        }
        //Check the folder to save
        #if UNITY_EDITOR
            var folder = Application.streamingAssetsPath+path;
        #else
            var folder = Application.persistentDataPath+path;
        #endif
        if (!Directory.Exists(folder)){
            Directory.CreateDirectory(folder);
        }

        string filePath = Path.Combine(folder, fileName+".csv");
        
        //prevent from re writting if its not the unique file resumeSession
        if (fileName!="resumeSession"){
            int i = 1;
            while (File.Exists(filePath)){
                if (i!=1){
                    fileName = fileName.Substring(0,fileName.Length-1);
                }
                fileName+=i.ToString();
                filePath = Path.Combine(folder, fileName+".csv");
                i++;
            }
        }
        return filePath;
    }

    public float calculateNorm(List<float> list){
        return (float)Math.Sqrt(list[0]*list[0]+list[1]*list[1]+list[2]*list[2]);
    }

}