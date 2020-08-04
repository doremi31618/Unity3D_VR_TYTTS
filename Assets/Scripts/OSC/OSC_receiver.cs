using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OscJack;

public class OSC_receiver : MonoBehaviour
{
    public int UDP_port_number = 9000;
    public string osc_address="/test";

    IEnumerator Start(){
        var server = new OscServer(UDP_port_number);
        
        server.MessageDispatcher.AddCallback(
            "/test", // OSC address
            (string address, OscDataHandle data) => {
                Debug.Log(string.Format("({0}, {1})",
                    data.GetElementAsFloat(0),
                    data.GetElementAsString(0)));
            }
        );

        print("on call");
        yield return new WaitForSeconds(10);
        server.Dispose();
    }
}
