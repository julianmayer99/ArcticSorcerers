﻿using Assets.Scripts;
using Assets.Scripts.Gamemodes;
using Assets.Scripts.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour, IPlayer
{
	//component acces
	private PlayerAnimationController ac;

	//Status of the Player
	public enum Status { Idle, Waddle, Attack, Dash, Dead }
	public Status currentStatus = Status.Idle; //currentStatus keeps track of the Status the player is currently in.

	public GameObject particleJumpPreFab;
	public Vector3 jumpParticleOffset;
	
	public GameObject dashParticlesPreFab;
	public Vector3 dashParticleOffset;

	public GameObject hardLandParticlesPreFab;

	[Header("Player Stats")]
	[SerializeField] private float m_JumpForce = 400f;
	[Range(1, 20f)] [SerializeField] private float m_Speed = 10f;
	public PlayerConfiguration config;
	public PlayerStats playerStats = new PlayerStats();
	public GamemodeSpecificPlayerAttributes gamemodeExtraInfo = new GamemodeSpecificPlayerAttributes();
	public float shotKnockBack = 5f;
	public float shotKnockUpwardsForce = 5f;

	[Header("Movement")]
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;
	[Range(0, 3.3f)] [SerializeField] private float m_IdleSmoothing = .05f;
	[Range(0, 3.3f)] [SerializeField] private float m_AimSmoothing = .05f;
	[Range(0, 3.3f)] [SerializeField] private float m_DashSmoothing = .0f;
	[SerializeField] private float turnSmoothTime = .1f;
	private float turnSmoothVelocity;
	private bool m_Grounded;
	public Rigidbody m_Rigidbody;
	private Vector3 m_Velocity = Vector3.zero;
	private float m_threshhold = 0.2f;
	private float m_animMaxSpeed = 3f;
	private int jumpsLeft = 1;
	private float lastVerticalVelocity = 0f;
	private float lastlastVerticalVelocity = 0f;
	[HideInInspector] public bool playerControlsEnabled = true;
	[HideInInspector] public InteractableObject selectedInteractable;

	//Dashing
	public float dashSpeed = 20f;
	public float dashTime = 0.12f;
	private float dashTimeCounter;

	private float dashCooldown = 0.1f;
	private float dashCooldownCounter;
	private float dashCooldownAirFactor = 0f;

	private Vector2 dashVector = new Vector2();
	public float dashYFac = 1f;
	public float dashExitSpeedFac = 0.3f;


	//Dead
	private float deathAnimationTimeCounter;
	public float deathAnimationTime = 1f;

	[Header("Aiming")] 
	public Transform aimingIndicator;
	private bool isAiming = false;
	private bool wasAiming = false;
	[Range(0, 120f)] public float aimingRange = 70f;
	public PlayerWeapon weapon;
	public int shootCoolDown = 5;
	private int shootCoolDownCounter = 5;
	private float aimingMoveDamp = 0.5f;

	[Header("UI")]
	[Space]
	[HideInInspector] public FloatingPlayerGuiHandler playerUI;
	public GameObject playerUiPreFab;
	public SkinnedMeshRenderer scarfMeshRenderer;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;
	public PlayerControllerInteractionManager.PlayerControllerEvent OnJumpEvent;
	public UnityEvent OnPlayerWasShot;
	public UnityEvent OnPlayerDied;

	[HideInInspector] public UnityEvent OnBackActionTriggered;
	[HideInInspector] public UnityEvent OnShowScoreboardActionTriggered;
	[HideInInspector] public UnityEvent OnCancelActionTriggered;

	private bool listenersAreSetUp = false;

	

	private void Awake()
	{
		m_Rigidbody = GetComponent<Rigidbody>();
		ac = GetComponent<PlayerAnimationController>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();
		if (OnPlayerWasShot == null)
			OnPlayerWasShot = new UnityEvent();
		if (OnPlayerDied == null)
			OnPlayerDied = new UnityEvent();
		if (OnBackActionTriggered == null)
			OnBackActionTriggered = new UnityEvent();
		if (OnShowScoreboardActionTriggered == null)
			OnShowScoreboardActionTriggered = new UnityEvent();
		if (OnCancelActionTriggered == null)
			OnCancelActionTriggered = new UnityEvent();

		shootCoolDownCounter = shootCoolDown;
	}

	private void Start()
	{
		if (listenersAreSetUp)
			return;

		config.Input.OnJumpButtonDown.AddListener(OnJumpPerformed);
		config.Input.OnBackButtonUp.AddListener(OnBackActionPerformed);
		config.Input.OnPauseButtonDown.AddListener(OnCancelActionPerformed);
		config.Input.OnScoreboardButtonDown.AddListener(OnShowScoreboardActionPerformed);
		config.Input.OnShootButtonDown.AddListener(OnShootButtonDownPerformed);
		config.Input.OnShootButtonUp.AddListener(OnShootButtonUpPerformed);
		config.Input.OnDashButtonDown.AddListener(OnDashButtonDownPerformed);

		listenersAreSetUp = true;
	}

	public void Initialize(PlayerConfiguration config)
	{
		this.config = config;
	}

	private void OnEnable()
	{
		DynamicMultiTargetCamera.instance.targets.Add(transform);

		CheckRespawnOrEnableUI();

		if (GameSettings.gameHasStarted)
		{
			InvokeRepeating(nameof(RecalculateDistanceCovered), 1f, 1f);
		}
	}

	public void CheckRespawnOrEnableUI()
	{
		if (playerUI == null)
		{
			playerUI = Instantiate(playerUiPreFab, FindObjectOfType<Canvas>().transform).GetComponent<FloatingPlayerGuiHandler>();
			playerUI.SetUpFloatingGui(this, FindObjectOfType<DynamicMultiTargetCamera>().GetComponent<Camera>());
		}
		playerUI.gameObject.SetActive(true);
	}

	private void OnDisable()
	{
		DynamicMultiTargetCamera.instance.targets.Remove(transform);

		if (playerUI != null)
		{
			playerUI.gameObject.SetActive(false);
		}

		CancelInvoke(nameof(RecalculateDistanceCovered));
	}

	private void OnDestroy()
	{
		config.Input.OnJumpButtonDown.RemoveListener(OnJumpPerformed);
		config.Input.OnBackButtonUp.RemoveListener(OnBackActionPerformed);
		config.Input.OnPauseButtonDown.RemoveListener(OnCancelActionPerformed);
		config.Input.OnScoreboardButtonDown.RemoveListener(OnShowScoreboardActionPerformed);
		config.Input.OnShootButtonDown.RemoveListener(OnShootButtonDownPerformed);
		config.Input.OnShootButtonUp.RemoveListener(OnShootButtonUpPerformed);
		config.Input.OnDashButtonDown.RemoveListener(OnDashButtonDownPerformed);
	}

	public void OnColorChanged()
	{
		scarfMeshRenderer.material = config.Color.material;
		scarfMeshRenderer.materials = new Material[] { config.Color.material };
		scarfMeshRenderer.sharedMaterials = new Material[] { config.Color.material };
		playerUI.UpdatePlayerColor();
	}

	private Vector3 distanceMeasurementLastPosition;
	private void RecalculateDistanceCovered()
	{
		if (distanceMeasurementLastPosition == null)
			distanceMeasurementLastPosition = transform.position;

		float newDistance = Vector3.Distance(transform.position, distanceMeasurementLastPosition);

		distanceMeasurementLastPosition = transform.position;
		playerStats.distanceCovered += newDistance;
	}

	private void Update()
	{
		config.Input.RefreshInput();
	}

	private void FixedUpdate()
	{
		if (!m_Grounded)
		{
			lastlastVerticalVelocity = lastVerticalVelocity;
			lastVerticalVelocity = m_Rigidbody.velocity.y;
		}

		if (shootCoolDownCounter > 0)
			shootCoolDownCounter--;

		switch (currentStatus)
        {
			default:
				IdleStatus();
				break;

			case Status.Waddle:
				WaddleStatus();
				break;

			case Status.Attack:
				AttackStatus();
				break;

			case Status.Dash:
				DashStatus();
				break;

			case Status.Dead:
				DeadStatus();
				break;
		}

		if (dashCooldownCounter > 0)
		{
			if (!m_Grounded)
			{
				dashCooldownCounter -= Time.deltaTime * dashCooldownAirFactor;
			}
			else
			{
				dashCooldownCounter -= Time.deltaTime;
			}
		}
		
	}

	private bool m_WasGrounded;

	private void OnTriggerEnter(Collider other)
	{
		if (currentStatus == Status.Dead) return;

		if (!m_WasGrounded)
		{
			OnLandEvent.Invoke();
			ac.Land();
		}

		if (lastlastVerticalVelocity < -17 || lastVerticalVelocity < -17)
		{
			AudioManager.instance.Play(AudioManager.audioSFXLand);
			config.Input.QueueGamepadVibration(PlayerInputMethod.Rumble.SmallShortPulse);
			Instantiate(hardLandParticlesPreFab, transform.position + jumpParticleOffset, Quaternion.identity);
		} else if (lastlastVerticalVelocity < -8 || lastVerticalVelocity < -8)
		{
			config.Input.QueueGamepadVibration(PlayerInputMethod.Rumble.VerySmallShortPulse);
		}

		m_Grounded = true;
		jumpsLeft = 1;
		m_WasGrounded = m_Grounded;

		var otherPlayer = other.GetComponent<PlayerController>();
		if (otherPlayer == null)
			return;

		if (otherPlayer.transform.position.y > transform.position.y)
			return; // Der andere Spieler ist über dem Spieler

		Debug.Log(otherPlayer.config.info.name + " jumped on the head of " + config.info.name);
		if (otherPlayer.config.Team.teamId != this.config.Team.teamId || GameSettings.TeamDamage)
		{
			otherPlayer.OnPlayerHasBeenShot(this, transform.position);
			otherPlayer.config.Input.QueueGamepadVibration(PlayerInputMethod.Rumble.StrongPulse);
			m_Rigidbody.AddForce(new Vector3(0f, 300f, 0f));
		}
	}

	public void Move(float move, float movementSmoothing)
	{
		Vector3 targetVelocity = new Vector2(move * m_Speed, m_Rigidbody.velocity.y);
		m_Rigidbody.velocity = Vector3.SmoothDamp(m_Rigidbody.velocity, targetVelocity, ref m_Velocity, movementSmoothing);

		AdjustPlayerRotation(config.Input.Move);
	}
	public void Move2(Vector2 move, float speed , float movementSmoothing, float yfac = 1f)
	{
		move.y *= yfac;
		move.Normalize();
		Vector3 targetVelocity = new Vector2(move.x * speed, move.y * speed);
		m_Rigidbody.velocity = Vector3.SmoothDamp(m_Rigidbody.velocity, targetVelocity, ref m_Velocity, movementSmoothing);

		AdjustPlayerRotation(config.Input.Move);
	}

	public void Jump(bool jump)
	{
		if (!jump)
			return;

		if (jumpsLeft < 1)
		{
			return;

			// TODO: check if player is standing on the ground
			/*
			if (m_Rigidbody.velocity.y > .001f)
				return;

			if (Physics.Conta( GetComponent<BoxCollider>().)
			{

			}
			*/
		}
		m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, 0f);

		if (!m_Grounded) jumpsLeft--;
		//else dashCooldown = 0f;
		m_Grounded = false;
		AudioManager.instance.Play(AudioManager.audioSFXJump);
		config.Input.QueueGamepadVibration(PlayerInputMethod.Rumble.VerySmallShortPulse);
		m_Rigidbody.AddForce(new Vector2(0f, m_JumpForce));
		OnJumpEvent.Invoke(this);
		Instantiate(particleJumpPreFab, transform.position + jumpParticleOffset, Quaternion.identity);
		playerStats.jumps++;
		ac.Jump();
		m_WasGrounded = m_Grounded;
	}

	public void OnJumpPerformed()
	{
		if (currentStatus == Status.Dead)
			return;

		if (selectedInteractable == null && playerControlsEnabled)
			Jump(true);
		else if (selectedInteractable != null && selectedInteractable.isButtonDownEvent)
		{
			// => On button down
			selectedInteractable.OnPlayerInteracted.Invoke(this);
			Debug.Log("Button down Event called for interactable");
		}
	}

	public void OnBackActionPerformed()
	{
		OnBackActionTriggered.Invoke();
	}

	public void OnShootButtonDownPerformed()
	{
		if (currentStatus == Status.Dead)
			return;

		if (!playerControlsEnabled)
			return;

		//Button up
		if (currentStatus != Status.Attack && shootCoolDownCounter <= 0 && playerStats.ammunitionLeft > 0)
		{
			//Exit current State
			if (currentStatus == Status.Waddle)
			{
				WaddleExit();
			}
			if (currentStatus == Status.Waddle)
			{
				IdleExit();
			}
			if (currentStatus == Status.Dash)
            {
				return;
            }

			AttackInit();
		}
	}
	
	public void OnShootButtonUpPerformed()
	{
		if (currentStatus == Status.Dead)
			return;

		if (!playerControlsEnabled)
			return;

		//Button down
		if (currentStatus == Status.Attack)
		{
			AttackShoot();
			AttackExit();
			IdleInit();
		}
	}

	public void OnShowScoreboardActionPerformed()
	{
		OnShowScoreboardActionTriggered.Invoke();
		PlayerControllerInteractionManager.Instance.OnScoreboardPressed.Invoke();
	}

	public void OnCancelActionPerformed()
	{
		OnCancelActionTriggered.Invoke();
		PlayerControllerInteractionManager.Instance.OnCancelButtonPressed.Invoke();
	}

	public void OnDashButtonDownPerformed()
    {
		if (!playerControlsEnabled || dashCooldownCounter > 0f)
			return;

		if (currentStatus == Status.Dead)
			return;

		//Button down
		if (currentStatus == Status.Attack)
		{
			AttackExit();
		}
		DashInit();
	}

	public void AttackInit()
    {
		//Initializing the Attack Status
		currentStatus = Status.Attack;
		ac.StartAttack();
	}

	public void AttackStatus()
    {
		//Durig the Attack Status
		Aim(config.Input.AimDirection);
		Move(0f, m_AimSmoothing);
	}

	public void AttackShoot()
	{
		InstantlyAdjustPlayerRotation();
		weapon.Shoot();
		config.Input.QueueGamepadVibration(PlayerInputMethod.Rumble.StrongPulse);
		playerStats.shotsFired++;
		ChangeAmmunnitionReserve(-1);
		shootCoolDownCounter = shootCoolDown;

	}

	public void AttackExit()
	{
		//Leaving the Attack Status
		// isAiming = false;
		if (playerUI.IsAiming)
			playerUI.IsAiming = false;
	}

	public void IdleInit()
	{
		//Initializing the Idle Status
		currentStatus = Status.Idle;
		ac.StartIdle();
	}

	public void IdleStatus()
    {
		//During Idle
		Move(0f,m_IdleSmoothing);

		//StartWaddle
		if (Mathf.Abs(config.Input.Move) > m_threshhold)
        {
			WaddleInit();
        }
    }

	public void IdleExit()
    {
		//Leaving Idles
    }
	public void WaddleInit()
    {
		//Initializing the Waddle Status
		currentStatus = Status.Waddle;
		ac.StartWaddle();
	}

	public void WaddleStatus()
    {
		if (!playerControlsEnabled)
			return;

		//During Waddle

		//Movement
		float move = config.Input.Move;
		Move(move, m_MovementSmoothing);

		//Animation
		ac.SetSpeed(Mathf.Abs(move) * m_animMaxSpeed);

		//StartIdle
		if (Mathf.Abs(move) < m_threshhold)
		{
			IdleInit();
			WaddleExit();
		}
	}

	public void WaddleExit()
    {
		//ResetSpeed
		ac.SetSpeed(1f);
	}

	public void DashInit()
    {
		currentStatus = Status.Dash;
		config.Input.QueueGamepadVibration(PlayerInputMethod.Rumble.FadeOut);
		ac.StartDash();
		AudioManager.instance.Play(AudioManager.audioSFXDash);

		//Set direction
		dashVector = config.Input.AimDirection;
			

		//Counters
		dashTimeCounter = dashTime;

		Instantiate(dashParticlesPreFab, transform.position + dashParticleOffset, Quaternion.identity);

		//Apply Speed
		//m_Rigidbody.velocity = new Vector3(config.Input.AimDirection.x, config.Input.AimDirection.y, 0f);
	}
	public void DashStatus()
    {
		Move2(dashVector, dashSpeed, m_DashSmoothing, dashYFac);
		dashTimeCounter -= Time.deltaTime;
		m_Grounded = false;

		if (dashTimeCounter <= 0)
        {
			DashExit();
			IdleInit();
        }
    }

    public void DashExit()
    {
		dashCooldownCounter = dashCooldown;
		//m_Rigidbody.velocity = m_Rigidbody.velocity * dashExitSpeedFac;
	}

	public void DeadInit()
    {
		currentStatus = Status.Dead;
		ac.StartDead();

		deathAnimationTimeCounter = deathAnimationTime;
    }
	void DeadStatus()
    {
		deathAnimationTimeCounter -= Time.deltaTime;

		if (deathAnimationTimeCounter <= 0)
        {
			DeadExit();
        }
    }
	void DeadExit()
    {
		WaddleInit();
	}

	public void ChangeAmmunnitionReserve(int add)
	{
		playerStats.ammunitionLeft += add;
		if (playerStats.ammunitionLeft > 5)
			playerStats.ammunitionLeft = 5;

		playerUI.UpdateAmmunitionReserveCount(playerStats.ammunitionLeft);
	}

	public void OnPlayerHasBeenShot(PlayerController fromPlayer, Vector3 shotPoint)
	{
		var forceDirection = (transform.position - shotPoint).normalized;
		if (forceDirection.y < shotKnockUpwardsForce)
			forceDirection.y = shotKnockUpwardsForce;

		m_Rigidbody.AddForce(forceDirection * shotKnockBack);

		PlayerControllerInteractionManager.Instance.OnPlayerHitOtherPlayer.Invoke(fromPlayer, this);

		playerStats.health -= 100;
		if (playerStats.health <= 0)
		{
			PlayerControllerInteractionManager.Instance.OnPlayerKilledOtherPlayer.Invoke(fromPlayer, this);
		}
	}

	public void Aim(Vector2 direction)
	{
		// aimingIndicator.gameObject.SetActive(true);

		aimingIndicator.transform.eulerAngles = new Vector3(
			aimingRange * direction.y,
			aimingIndicator.transform.eulerAngles.y,
			aimingIndicator.transform.eulerAngles.z
			);

		playerUI.aimIndicator.eulerAngles = direction.x >= 0
			? new Vector3(
			playerUI.aimIndicator.eulerAngles.x,
			playerUI.aimIndicator.eulerAngles.y,
			(aimingRange * direction.y) - 90
			)
			: new Vector3(
			playerUI.aimIndicator.eulerAngles.x,
			playerUI.aimIndicator.eulerAngles.y,
			(-aimingRange * direction.y) + 90
			);


		if (!playerUI.IsAiming)
			playerUI.IsAiming = true;
	}

	internal void OnTeamChanged()
	{
		if (GameSettings.gameMode.IsTeamBased && config.Team.teamId == 1)
			ac.SetColorBlack();
		else
			ac.SetColorBlue();
	}

	float adjustRotTarget = 0f;
	float adjustRotTargetOld = 0f;

	private void AdjustPlayerRotation(float directionX)
	{
		//adjustRotTarget = transform.eulerAngles.y;

		if (directionX > 0)
		{
			facingRight = false;
			adjustRotTarget = -90;
		}
		else if (directionX < 0)
		{
			facingRight = true;
			adjustRotTarget = 90;
		}

		if (adjustRotTargetOld != adjustRotTarget)
		{
			float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, 0, ref turnSmoothVelocity, turnSmoothTime);
			transform.rotation = Quaternion.Euler(0f, angle, 0f);
		}
        else
		{
			float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, adjustRotTarget, ref turnSmoothVelocity, turnSmoothTime);
			transform.rotation = Quaternion.Euler(0f, angle, 0f);
		}

		adjustRotTargetOld = adjustRotTarget;
	}

	private bool facingRight = true;

	public Transform Transform => transform;

	private void InstantlyAdjustPlayerRotation()
	{
		float targetAngle = transform.eulerAngles.y < 0 || transform.eulerAngles.y > 180
			? -90
			: 90;

		transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
	}

	public void VisualizeTakingDamage()
	{
		// TODO: Spieler kurz rot werden lassen oder so
	}

	public void OnPlayerStartedObjective()
	{
		PlayerControllerInteractionManager.Instance.OnPlayerStartedObjective.Invoke(this);
	}

	public void OnPlayerScoredObjective()
	{
		PlayerControllerInteractionManager.Instance.OnPlayerScoredObjective.Invoke(this);
	}
}
