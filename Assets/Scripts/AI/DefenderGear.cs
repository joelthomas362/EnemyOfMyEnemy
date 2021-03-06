﻿using UnityEngine;
using System.Collections;

public class DefenderGear : MonoBehaviour {


    public GameObject shield;
    private DefenderEnemy defender;
    private Moe _moeScript;


    void Awake()
    {
        defender = transform.parent.gameObject.GetComponent<DefenderEnemy>();
        _moeScript = GameObject.FindGameObjectWithTag("Moe").GetComponent<Moe>();
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Bullet"))
            DestroyShield();
    }

    void DestroyShield()
    {
        // make a target for Moe //
        defender.gameObject.tag = "Enemy";

        // enable Moe's abilities
        _moeScript._actionAvailable = true;
        _moeScript._isFollowing = true;

        // defender vulnerable //
        defender._shieldActive = false;
        defender._healthScript.enabled = true;
        Destroy(gameObject);
        Destroy(shield);
    }
}
