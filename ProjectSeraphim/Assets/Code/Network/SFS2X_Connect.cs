using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Requests;


public class SFS2X_Connect : MonoBehaviour {

	public string ConfigFile;

	public bool UseConfigFile = false;
	public string ServerIP = "86.26.57.201";
	public int ServerPort = 9933;
	public string ZoneName = "TestBed";
	public string UserName = "";
	public string RoomName = "Hub";

    public PlayerController playerCharacter;

	SmartFox sfs;


	void Start ()
	{
		sfs = new SmartFox ();
		sfs.ThreadSafeMode = true;
		ConfigFile = Application.dataPath + "/Scripts/Network/sfs-config.xml";

		//Callbacks
		sfs.AddEventListener(SFSEvent.CONNECTION, OnConnection);
		sfs.AddEventListener(SFSEvent.LOGIN, OnLogin);
		sfs.AddEventListener (SFSEvent.LOGIN_ERROR, OnLoginError);
		sfs.AddEventListener (SFSEvent.CONFIG_LOAD_SUCCESS, OnConfigLoad);
		sfs.AddEventListener (SFSEvent.CONFIG_LOAD_FAILURE, OnConfigFail);
		sfs.AddEventListener (SFSEvent.ROOM_JOIN, OnJoinRoom);
		sfs.AddEventListener (SFSEvent.ROOM_JOIN_ERROR, OnJoinRoomError);
		sfs.AddEventListener (SFSEvent.PUBLIC_MESSAGE, OnPublicMessage);
				

		//Connect to the server
		if (UseConfigFile)
		{
			sfs.LoadConfig(ConfigFile, false);
		}
		else
		{
			sfs.Connect (ServerIP, ServerPort);
		}
	}


	void OnConfigLoad(BaseEvent e)
	{
		Debug.Log ("Config Loaded");
		sfs.Connect (sfs.Config.Host, sfs.Config.Port);
	}


	void OnConfigFail(BaseEvent e)
	{
		Debug.Log (e.Params["errorCode"] + " " + e.Params["errorMessage"]);
	}


	void OnLogin(BaseEvent e)
	{
		Debug.Log ("Logged In " + e.Params ["user"]);
		sfs.Send (new JoinRoomRequest (RoomName));
	}


	void OnJoinRoom(BaseEvent e)
	{
		Debug.Log ("Joined Room: " + e.Params ["room"]);
		sfs.Send (new PublicMessageRequest ("Hello world!"));
    }


	void OnPublicMessage (BaseEvent e)
	{
		Room room = (Room)e.Params ["room"];
		User sender = (User)e.Params ["sender"];
		Debug.Log ("[" + room.Name + "]" + sender.Name + ": " + e.Params ["message"]);
	}


	void OnJoinRoomError(BaseEvent e)
	{
		Debug.Log ("JoinRoom Error (" + e.Params["errorCode"] + ")" + e.Params["errorMessage"]);
	}


	void OnLoginError(BaseEvent e)
	{
		Debug.Log ("Login Error (" + e.Params["errorCode"] + ")" + e.Params["errorMessage"]);
	}


	void OnConnection(BaseEvent e)
	{
		if((bool)e.Params["success"])
			{
				Debug.Log ("Successfully Connected");
				if (UseConfigFile)
				{
					ZoneName = sfs.Config.Zone;
					sfs.Send (new LoginRequest (UserName, "", ZoneName));
				}
			}
			else
			{
				Debug.Log("Connection Failed");
			}

	}


	void Update ()
	{
		sfs.ProcessEvents ();
	}


	void OnApplicationQuit()
	{
		if(sfs.IsConnected)
			sfs.Disconnect ();
	}
}
