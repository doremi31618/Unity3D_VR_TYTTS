using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OscJack;
public class OSC_sender : MonoBehaviour
{
    [Header("IP information")]
    public string ip_address="127.0.0.1";
    public int UDP_port_number = 9000;
    [Header("OSC information")]
    public string osc_address="/test";
    public string content="osc_content";

    
    IEnumerator Start(){
        var client  = new OscClient(ip_address, UDP_port_number);
        for(var i=0; i<10; i++){
            yield return new WaitForSeconds(0.5f);
            client.Send(osc_address,
                        i*10.0f);
            client.Send(osc_address,
                        content);
        }
        
        client.Dispose();
    }
}
