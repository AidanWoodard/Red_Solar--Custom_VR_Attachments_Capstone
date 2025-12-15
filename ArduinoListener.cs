using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArduinoListener : MonoBehaviour
{
    private SerialController serialController;
    private PlayerController playerController;
    private bool warned = false;

    void Start()
    {
        // Object that receives messages via serial
        serialController = GameObject.Find("SerialController").GetComponent<SerialController>();
        playerController = GameObject.Find("PLAYER (XR Origin)").GetComponent<PlayerController>();
    }

    void Update()
    {
        // latest message
        string newMsg = serialController.ReadSerialMessage();

        if (newMsg == null && !warned)
        {
            Debug.LogWarning("No messages from listener");
            warned = true;
            return;
        }
        Debug.Log(newMsg);
        // handle input from arduino
        if (newMsg == "ON")
        {
            playerController.handsConnected = true;
            warned = false;     // for if no events
        }
        else if (newMsg == "OFF")
        {
            playerController.handsConnected = false;
            warned = false;     // for if no events
        }
    }
}
