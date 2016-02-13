﻿using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {


    public int health;
    public GameObject deathParticles;
    public Color hurtColor;
    [HideInInspector] public Transform playerRespawnPoint;

    private int _initialHealth;


    void Awake()
    {
        _initialHealth = health;
    }

    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;

        gameObject.GetComponent<Renderer>().material.color = hurtColor;


        if (health > 0)
            return;


        if (gameObject.CompareTag("Player"))
            RespawnPlayer();
        else
            Die();
    }

    void Die()
    {
        CameraController.Instance.ScreenShake(.1f);

        Instantiate(deathParticles, transform.position, Quaternion.Euler(90f, 0f, 0f));
        Destroy(gameObject);
    }

    void RespawnPlayer()
    {
        transform.position = playerRespawnPoint.position;

        health = _initialHealth;
    }
}
