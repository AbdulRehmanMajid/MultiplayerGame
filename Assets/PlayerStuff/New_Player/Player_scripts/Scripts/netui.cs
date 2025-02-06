using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode.Transports.UTP;
using System.IO;
using TMPro;

public class netui : MonoBehaviour
{
    
    [SerializeField]private Button Hostbtn;
    [SerializeField]private Button Clientbtn;
    [SerializeField]private Button Serverbtn;
    

    [SerializeField]private GameObject ui;

    [SerializeField]private int packetdelay;
    [SerializeField]private int jitter;
    [SerializeField]private int dropRate;
    public TMP_InputField ip_field;

    public string ip;


    public UnityTransport transport;

    public void readinput(string s)
    {
        transport.SetConnectionData(s,8009);

    }

    public void Start(){
         if(Application.platform != RuntimePlatform.Android)
        {
         string path = Application.dataPath + "/server.txt";
         string path_fps = Application.dataPath + "/fps.txt";
        if(!File.Exists(path))
        {
            File.WriteAllText(path,"off");
        
        }
        if(!File.Exists(path_fps))
        {
            File.WriteAllText(path_fps,"60");
        
        }
        
        string active = File.ReadAllText(path);
        string fps_limit = File.ReadAllText(path_fps);
        int fps_limit_int = int.Parse(fps_limit);
        Application.targetFrameRate = fps_limit_int;
        Debug.LogError(active);
        if(active == "on")
        {
            transport.SetConnectionData("192.168.18.13",8009);
            NetworkManager.Singleton.StartServer();

        }
        else if(active == "off")
        {
            Debug.LogError("Dedicated Server Off");
        }
        string pathe = Application.dataPath + "/ip.txt";
        if(!File.Exists(pathe))
        {
            File.WriteAllText(pathe,"192.168.18.13");
        
        }
        
        string con_data =File.ReadAllText(pathe);
        transport.SetConnectionData(con_data,8009);
        }
         
    }
    public void change_ip()
    {
        ip = ip_field.text;
        transport.SetConnectionData(ip,8009);

    }
   

    

    private void Awake(){
        
        transport.SetDebugSimulatorParameters(packetdelay,jitter,dropRate);
        Hostbtn.onClick.AddListener(() => {

            NetworkManager.Singleton.StartHost();
           
            ui.SetActive(false);
        });
        Clientbtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
            ui.SetActive(false);
            
        });
        Serverbtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartServer();
            ui.SetActive(false);
            
        });

 



    


}
void Update()
{
if(Application.platform == RuntimePlatform.Android)
{
    
    QualitySettings.vSyncCount = 0;
Application.targetFrameRate = 120;
}

}
}
