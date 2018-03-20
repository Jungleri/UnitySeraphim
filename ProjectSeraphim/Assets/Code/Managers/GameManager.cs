using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Variables;
using Sfs2X.Requests;
using Sfs2X.Logging;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{


    private Dictionary<SFSUser, GameObject> remotePlayers = new Dictionary<SFSUser, GameObject>();

    PlayerController playerController;
    PlayerController playerMotor;
    Health playerHealth;
	private GameObject localPlayer;
	public GameObject playerPrefab;

    private SmartFox sfs;


    void Start()
    {
        if (!SmartFoxConnection.IsInitialized)
        {
            SceneManager.LoadScene(0);
            return;
        }

        sfs = SmartFoxConnection.Connection;

        sfs.AddEventListener(SFSEvent.OBJECT_MESSAGE, OnObjectMessage);
        sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        sfs.AddEventListener(SFSEvent.USER_VARIABLES_UPDATE, OnUserVariableUpdate);
        sfs.AddEventListener(SFSEvent.USER_EXIT_ROOM, OnUserExitRoom);
        sfs.AddEventListener(SFSEvent.USER_ENTER_ROOM, OnUserEnterRoom);

        SpawnLocalPlayer();
    }


	void FixedUpdate()
	{
		if (sfs != null)
		{   //If there is a sfs instance, process events.
			sfs.ProcessEvents ();

            if (localPlayer != null && (playerHealth.requestUpdate))
            {   //If we have a local player and variables have not been sent to the server, do so.
                List<UserVariable> userVariables = new List<UserVariable>();

                if (playerMotor)
                {   //If the player has moved, add those variables.
                    userVariables.Add(new SFSUserVariable("x", (double)localPlayer.transform.position.x));
                    userVariables.Add(new SFSUserVariable("y", (double)localPlayer.transform.position.y));
                    userVariables.Add(new SFSUserVariable("z", (double)localPlayer.transform.position.z));
                    userVariables.Add(new SFSUserVariable("rot", (double)localPlayer.transform.rotation.eulerAngles.y));
                    //Clean the motor requestUpdate.
                }

                if (playerHealth.requestUpdate)
                {   //If the player's health has changed, add variables.
                    userVariables.Add(new SFSUserVariable("hp", (double)playerHealth.health));
                    userVariables.Add(new SFSUserVariable("maxhp", (double)playerHealth.maxHealth));
                    //Clean the health requestUpdate.
                    playerHealth.requestUpdate = false;
                }

                //Send the user variables.
                sfs.Send(new SetUserVariablesRequest(userVariables));
            }
		}
	}


    void OnApplicationQuit()
    {   //When the game closes, remove the player from the server.
        RemoveLocalPlayer();
    }


    public void OnObjectMessage(BaseEvent e)
    {
        ISFSObject dataObj = (SFSObject)e.Params["message"];
        SFSUser sender = (SFSUser)e.Params["sender"];

        if (dataObj.ContainsKey("cmd"))
        {
            switch (dataObj.GetUtfString("cmd"))
            {
                case "rm":
                    Debug.Log("Removing " + sender.Id);
                    RemoveRemotePlayer(sender);
                    break;
            }
        }
    }


    public void OnConnectionLost(BaseEvent e)
    {   //If we lose connection, clear all listeners and return to the main menu.
        sfs.RemoveAllEventListeners();
        SceneManager.LoadScene("Connection");
    }


    public void OnUserExitRoom(BaseEvent e)
    {   //When another player leaves the room, remove them from our user list and scene.
        SFSUser user = (SFSUser)e.Params["user"];
        RemoveRemotePlayer(user);
    }


    public void OnUserEnterRoom(BaseEvent e)
    {
        if (localPlayer != null)
        {   //Initialize the local player.
            List<UserVariable> userVariables = new List<UserVariable>();
            userVariables.Add(new SFSUserVariable("x", (double)localPlayer.transform.position.x));
            userVariables.Add(new SFSUserVariable("y", (double)localPlayer.transform.position.y));
            userVariables.Add(new SFSUserVariable("z", (double)localPlayer.transform.position.z));
            userVariables.Add(new SFSUserVariable("rot", (double)localPlayer.transform.rotation.eulerAngles.y));
            userVariables.Add(new SFSUserVariable("hp", (double)playerHealth.health));
            userVariables.Add(new SFSUserVariable("maxhp", (double)playerHealth.maxHealth));
            //Send the local player's new variables.
            sfs.Send(new SetUserVariablesRequest(userVariables));
        }
    }


    public void OnUserVariableUpdate(BaseEvent e)
	{
		ArrayList newVars = (ArrayList)e.Params ["changedVars"];    //Contains the updated variables passed in.
        SFSUser user = (SFSUser)e.Params["user"];   //Contains the user who has sent the update.

        if (user == sfs.MySelf)
            //Incase the player is myself, ignore it and return.
			return;

        if (!remotePlayers.ContainsKey(user))
        {
            Vector3 pos = new Vector3(0, 0.9f, 0);
            if (user.ContainsVariable("x") && user.ContainsVariable("y") && user.ContainsVariable("z"))
            {
                pos.x = (float)user.GetVariable("x").GetDoubleValue();
                pos.y = (float)user.GetVariable("y").GetDoubleValue();
                pos.z = (float)user.GetVariable("z").GetDoubleValue();
            }

            float rotAngle = 0;
            if (user.ContainsVariable("rot"))
            {
                rotAngle = (float)user.GetVariable("rot").GetDoubleValue();
            }
            SpawnRemotePlayer(user, pos, Quaternion.Euler(0, rotAngle, 0));
        }


        if(newVars.Contains("x") && newVars.Contains("y") && newVars.Contains("z") && newVars.Contains("rot"))
        {
            remotePlayers[user].GetComponent<RemoteInterpolate>().SetTransform(
                new Vector3((float)user.GetVariable("x").GetDoubleValue(), (float)user.GetVariable("y").GetDoubleValue(), (float)user.GetVariable("z").GetDoubleValue()),
                Quaternion.Euler(0, (float)user.GetVariable("rot").GetDoubleValue(), 0),
                true);
        }
        if(newVars.Contains("hp"))
        {
            remotePlayers[user].GetComponent<Health>().health = (float)user.GetVariable("hp").GetDoubleValue();
            remotePlayers[user].GetComponent<Health>().maxHealth = (float)user.GetVariable("maxhp").GetDoubleValue();
        }
    }


    public void Disconnect()
    {
        sfs.Disconnect();
    }


    private void SpawnLocalPlayer()
    {
        Vector3 pos;
        Quaternion rot;

        if (localPlayer != null)
        {
            pos = localPlayer.transform.position;
            rot = localPlayer.transform.rotation;
            Camera.main.transform.parent = null;
            Destroy(localPlayer);
        }
        else
        {
            pos = new Vector3(0, 2.8f, 0);
            rot = Quaternion.identity;
        }

        localPlayer = GameObject.Instantiate(playerPrefab) as GameObject;
        localPlayer.transform.position = pos;
        localPlayer.transform.rotation = rot;

        localPlayer.AddComponent<PlayerController > ();
        playerController = localPlayer.GetComponent<PlayerController>();
        playerMotor = localPlayer.GetComponent<PlayerController>();
        playerHealth = localPlayer.GetComponent<Health>();
        Camera.main.transform.parent = localPlayer.transform;
        Camera.main.transform.position = new Vector3(0, localPlayer.transform.position.y + 0.7f, 0);
    }


    private void SpawnRemotePlayer(SFSUser _user, Vector3 _pos, Quaternion _rot)
    {

        if (remotePlayers.ContainsKey(_user) && remotePlayers[_user] != null)
        {
            Destroy(remotePlayers[_user]);
            remotePlayers.Remove(_user);
        }

        GameObject remotePlayer = GameObject.Instantiate(playerPrefab);
        remotePlayer.AddComponent<RemoteInterpolate>();
        remotePlayer.GetComponent<RemoteInterpolate>().SetTransform(_pos, _rot, false);


        remotePlayers.Add(_user, remotePlayer);
    }
    

    private void RemoveLocalPlayer()
    {
        SFSObject obj = new SFSObject();
        obj.PutUtfString("cmd", "rm");
        sfs.Send(new ObjectMessageRequest(obj, sfs.LastJoinedRoom));
    }

    
    private void RemoveRemotePlayer(SFSUser _user)
        {
        if(_user == sfs.MySelf)
        {
            return;
        }
        if (remotePlayers.ContainsKey(_user))
        {
            Destroy(remotePlayers[_user]);
            remotePlayers.Remove(_user);
        }
    }

    public void Die()
    {

    }
}