﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerController : MonoBehaviour {


    // Shooting
    public Rigidbody bulletPrefab;
    public Transform firePoint;

    // Audio
    public AudioClip[] playerSoundEffects;
    private AudioSource _playerSounds;
    // 0 Dash 
    // 1 Shoot
    // 2 Taunt

    // Components
    private Rigidbody _playerControls;
    private MoeAI _moeScript;

    // Animations
    private Animator _playerAnimator;
    private int _dashAnim;
    private int _shootAnim;

    // Attributes
    private float moveSpeed;
    private float moveSpeedModifier;
    private float horz;
    private float vert;

    // Abilities
    [HideInInspector] public bool tauntDisabled;
    private bool canTaunt;
    private bool _canRoll;
    private bool _canShoot;
    private bool _rotationDisabled;



    void Awake()
    {
        _moeScript = GameObject.FindGameObjectWithTag("Moe").GetComponent<MoeAI>();
        _playerControls = GetComponent<Rigidbody>();
        _playerSounds = GetComponent<AudioSource>();

        _playerAnimator = GetComponent<Animator>();
        _dashAnim = Animator.StringToHash("Dash");
        _shootAnim = Animator.StringToHash("Shoot");

        // Player metrics
        moveSpeed = 5f;
        moveSpeedModifier = 1f;

        _canRoll = true;
        _canShoot = true;
        canTaunt = true;
        tauntDisabled = false;
    }

    void FixedUpdate()
    {
        // -- left thumbstick controls -- //
        horz = Input.GetAxisRaw("LeftHorz");
        vert = Input.GetAxisRaw("LeftVert");
        // -- right thumbstick controls -- //
        float rightHorz = Input.GetAxis("RightHorz");
        float rightVert = Input.GetAxis("RightVert");


        // move player with left stick
        if (horz != 0f || vert != 0f)
            MovePlayer(horz, vert);

        // if player doesn't aim with right stick, aim with left
        if (rightHorz == 0 && rightVert == 0)
            RotatePlayer(horz, vert);
        else
            RotatePlayer(rightHorz, rightVert);

        // dive roll
        if (Input.GetAxis("Dash") > 0)
            StartCoroutine(DiveRoll(horz, vert));
    }

    void Update()
    {      
        // player shooting
        if (Input.GetAxis("Shoot") > 0f)
            StartCoroutine(ShootPea());


        if (Input.GetButtonDown("Taunt") && canTaunt && !tauntDisabled)
            StartCoroutine(Taunt());

        //Restart Level
        if (Input.GetKeyDown(KeyCode.Q))
            SceneManager.LoadScene(0);

        //Exit Game
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    void MovePlayer(float hAxis, float vAxis)
    {
        Vector3 movement = new Vector3(hAxis, 0f, vAxis);

        _playerControls.MovePosition(transform.position + movement * moveSpeed * Time.deltaTime);
    }

    void RotatePlayer(float hAxis, float vAxis)
    {
        if (_rotationDisabled)
            return;

        float angle = Mathf.Atan2(hAxis, vAxis) * Mathf.Rad2Deg;

        if (angle == 0f)
            return;

        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    IEnumerator Taunt()
    {
        canTaunt = false;

        _playerSounds.clip = playerSoundEffects[2];
        _playerSounds.Play();

        _moeScript.ChangeState(MoeAI.aiState.charging);

        yield return new WaitForSeconds(1f);

        canTaunt = true;
    }

    IEnumerator DiveRoll(float rHAxis, float rVAxis)
    {
        if (!_canRoll)
            yield break;

        _canRoll = false;
        StartCoroutine(DisableRotation());

        _playerAnimator.SetTrigger(_dashAnim);
        
        yield return new WaitForSeconds(2f);

        _canRoll = true;
    }

    IEnumerator DisableRotation()
    {
        _rotationDisabled = true;
        yield return new WaitForSeconds(.8f);
        _rotationDisabled = false;
    }

    IEnumerator DashAnimEvent()
    {
        _playerSounds.clip = playerSoundEffects[0];
        _playerSounds.Play();
       
        Vector3 dashTarget = transform.position + new Vector3(transform.forward.x * 6.5f, transform.position.y, transform.forward.z * 6.5f);
        float dashUnstick = 1.0f;

        while(Vector3.Distance(transform.position, dashTarget) > 2f)
        {
            if (dashUnstick <= 0f)
                break;

            transform.position = Vector3.Lerp(transform.position, dashTarget,  Time.deltaTime * 3f);
            dashUnstick -= Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator ShootPea()
    {
        if (!_canShoot)
            yield break;

        _canShoot = false;

        _playerAnimator.SetTrigger(_shootAnim);

        yield return new WaitForSeconds(1f);

        _canShoot = true;
    }

    void ShootAnimEvent()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        _playerSounds.clip = playerSoundEffects[1];
        _playerSounds.Play();
    }

	public float MoveSpeedModifier {
		get {
			return moveSpeedModifier;
		}
		set {
			moveSpeedModifier = value;
		}
	}
}
