using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

    GameManager gm;
    GameUI ui;

    public float maxHealth = 100;
    public float health;
    public bool requestUpdate = true;


    private void Start()
    {
        gm = GameObject.Find("_GameManager").GetComponent<GameManager>();
        ui = GameObject.Find("_UI").GetComponent<GameUI>();

        health = maxHealth;
        requestUpdate = true;
    }


    public void Hit(float _damage)
    {
        health -= _damage;
        requestUpdate = true;
    }

    public void UpdateMaxHealth(float _amount)
    {
        maxHealth += _amount;
        requestUpdate = true;
    }


    public void Heal(float _amount)
    {
        health += _amount;
        requestUpdate = true;
    }


    void FixedUpdate()
    {
        Mathf.Clamp(health, 0, maxHealth);
        ui.UpdateHealth(health, maxHealth);

        if (health <= 0)
            gm.Die();
    }
}