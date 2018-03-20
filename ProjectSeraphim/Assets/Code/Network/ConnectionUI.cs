using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Sfs2X;
using Sfs2X.Logging;
using Sfs2X.Util;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Requests;

public class ConnectionUI : MonoBehaviour
{

    private SmartFox sfs;

    public InputField nameField;
    public Button loginButton;

    
    public string serverIP = "127.0.0.1";
    public int serverPort = 9933;
    public string serverZone = "TestBed";
    public string zoneRoom = "Hub";


    void Update()
    {
        if (sfs != null)
        {
            sfs.ProcessEvents();
        }
    }


    private void Awake()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }


    public void OnLoginButtonClick()
    {
        EnableLoginUI(false);

        ConfigData config = new ConfigData();
        config.Host = serverIP;
        config.Port = serverPort;
        config.Zone = serverZone;

        sfs = new SmartFox();

        sfs.ThreadSafeMode = true;

        sfs.AddEventListener(SFSEvent.CONNECTION, OnConnection);
        sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        sfs.AddEventListener(SFSEvent.LOGIN, OnLogin);
        sfs.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
        sfs.AddEventListener(SFSEvent.ROOM_JOIN, OnRoomJoin);
        sfs.AddEventListener(SFSEvent.ROOM_JOIN_ERROR, OnRoomJoinError);

        sfs.Connect(config);
    }


    private void ResetSFS()
    {
        sfs.RemoveAllEventListeners();
        EnableLoginUI(true);
    }


    private void EnableLoginUI(bool _enabled)
    {
        nameField.interactable = _enabled;
        loginButton.interactable = _enabled;
    }


    private void OnConnection(BaseEvent e)
    {
        if ((bool)e.Params["success"])
        {
            SmartFoxConnection.Connection = sfs;
            sfs.Send(new Sfs2X.Requests.LoginRequest(nameField.text));
        }
    }


    private void OnConnectionLost(BaseEvent e)
    {
        ResetSFS();
        string reason = (string)e.Params["reason"];
        Debug.Log("Connection was lost; reason is: " + reason);
    }


    private void OnLogin(BaseEvent e)
    {
        sfs.Send(new JoinRoomRequest(zoneRoom));
    }


    private void OnLoginError(BaseEvent e)
    {
        sfs.Disconnect();
        ResetSFS();
        Debug.Log("Login failed: " + (string)e.Params["errorMessage"]);
    }


    private void OnRoomJoin(BaseEvent e)
    {
        ResetSFS();
        SceneManager.LoadScene(1);
    }


    private void OnRoomJoinError(BaseEvent e)
    {
        Debug.Log("Room join failed: " + (string)e.Params["errorMessage"]);
    }
}