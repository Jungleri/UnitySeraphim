using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Variables;
using Sfs2X.Requests;
using Sfs2X.Logging;

public class GameUI : MonoBehaviour
{ 

    SFSUser localPlayer;
    [SerializeField]
    Image healthBar;
    

    private string myName = "Jungleri";
    private float displayHealth;
    private float myHealth;
    private float maxHealth = 100;

    [SerializeField]
    Canvas gameUI;
    [SerializeField]
    Canvas pauseUI;

    bool paused;



    private void Start()
    {
        myHealth = maxHealth;
    }


    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        paused = false;
        pauseUI.enabled = false;
    }


    private void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            TogglePause();
        }
    }


    private void FixedUpdate()
    {
        DisplayHealth();
    }


    private void DisplayHealth()
    {
        if (displayHealth != myHealth)
        {
            float targetHealth = myHealth / maxHealth;
            displayHealth = Mathf.Lerp(displayHealth, targetHealth, Time.deltaTime * 6);
            healthBar.fillAmount = displayHealth;
        }
    }


    public void UpdateName(string _name)
    {
        myName = _name;
    }

    
    public void UpdateHealth(float _health, float _maxHealth)
    {
        myHealth = _health;
        maxHealth = _maxHealth;
    }


    void TogglePause()
    {
        if(!paused)
        {
            gameUI.enabled = false;
            pauseUI.enabled = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
           
            paused = true;
        }
        else if(paused)
        {
            pauseUI.enabled = false;
            gameUI.enabled = true;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            paused = false;
        }
    }


    public void QuitGame()
    {
        Application.Quit();
    }
}