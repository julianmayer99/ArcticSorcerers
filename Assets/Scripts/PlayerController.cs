using Assets.Scripts.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
	//component acces
	private PlayerAnimationController ac;

	//Status of the Player
	private enum Status { Idle, Waddle, Attack}
	private Status currentStatus = Status.Idle; //currentStatus keeps track of the Status the player is currently in.

	[Header("Player Stats")]
	[SerializeField] private float m_JumpForce = 400f;
	[Range(1, 20f)] [SerializeField] private float m_Speed = 10f;
	public PlayerConfiguration config;
	public float shotKnockBack = 5f;
	public float shotKnockUpwardsForce = 5f;
	public PlayerStats playerStats = new PlayerStats();

	[Header("Movement")]
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;
	[SerializeField] private float turnSmoothTime = .1f;
	private float turnSmoothVelocity;
	private bool m_Grounded;
	private Rigidbody m_Rigidbody;
	private Vector3 m_Velocity = Vector3.zero;
	private Vector2 moveDirection = Vector2.zero;
	private float m_threshhold = 0.2f;
	private float m_animMaxSpeed = 3f;
	[HideInInspector] public InteractableObject selectedInteractable;

	[Header("Aiming")]
	public Transform aimingIndicator;
	private bool isAiming = false;
	[Range(0, 120f)] public float aimingRange = 70f;
	public PlayerWeapon weapon;
	public int shootCoolDown = 5;
	private int shootCoolDownCounter = 5;

	[Header("UI")]
	[Space]
	[HideInInspector] public FloatingPlayerGuiHandler playerUI;
	public GameObject playerUiPreFab;
	public SkinnedMeshRenderer scarfMeshRenderer;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;
	public UnityEvent OnPlayerWasShot;
	public UnityEvent OnPlayerDied;

	[HideInInspector] public UnityEvent OnBackActionTriggered;
	[HideInInspector] public UnityEvent OnShowScoreboardActionTriggered;

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


		shootCoolDownCounter = shootCoolDown;
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


	private void FixedUpdate()
	{
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

        }
	}

	private bool m_WasGrounded;

	private void OnTriggerEnter(Collider other)
	{
		if (!m_WasGrounded)
		{
			OnLandEvent.Invoke();
		}

		m_Grounded = true;
		m_WasGrounded = m_Grounded;
	}

	public void Move(float move)
	{
		Vector3 targetVelocity = new Vector2(move * m_Speed, m_Rigidbody.velocity.y);
		m_Rigidbody.velocity = Vector3.SmoothDamp(m_Rigidbody.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

		AdjustPlayerRotation(moveDirection.x);
	}

	public void Jump(bool jump)
	{
		if (m_Grounded && jump)
		{
			m_Grounded = false;
			m_Rigidbody.AddForce(new Vector2(0f, m_JumpForce));
			playerStats.jumps++;
		}
	}

	public void OnMovePerformed(InputAction.CallbackContext context)
	{
		moveDirection = context.ReadValue<Vector2>();
	}

	public void OnJumpPerformed(InputAction.CallbackContext context)
	{
		if (!context.performed)
		{
			return;
		}

		var jump = context.ReadValue<float>() >= 1;
		if (selectedInteractable == null)
			Jump(jump);
		else if (jump && selectedInteractable.isButtonDownEvent)
		{
			// => On button down
			selectedInteractable.OnPlayerInteracted.Invoke(this);
			Debug.Log("Button down Event called for interactable");
		}
		else if (!jump && !selectedInteractable.isButtonDownEvent)
		{
			// => On button up
			selectedInteractable.OnPlayerInteracted.Invoke(this);
			Debug.Log("Button up Event called for interactable");
		}
	}

	public void OnBackActionPerformed(InputAction.CallbackContext context)
	{
		var buttonDown = context.ReadValue<float>() >= 1;

		if (!buttonDown) // => OnButtonUp Event
			OnBackActionTriggered.Invoke();
	}

	public void OnShootPerformed(InputAction.CallbackContext context)
	{
		//Context value
		//var shootButtonDown = context.ReadValue<float>() = 1;
		if (context.ReadValue<float>() == 1)
        {
			//Button pressed
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
				AttackInit();
            }
        }
        else if (context.ReadValue<float>() == 0)
		{
			//Button released
			if (currentStatus == Status.Attack)
			{
				AttackShoot();
				AttackExit();
				IdleInit();
			}
        }
	}

	public void OnShowScoreboardActionPerformed(InputAction.CallbackContext context)
	{
		var buttonDown = context.ReadValue<float>() >= 1;

		if (buttonDown) // => OnButtonDown Event
			OnShowScoreboardActionTriggered.Invoke();
	}

	public void AttackInit()
    {
		//Initializing the Attack Status
		currentStatus = Status.Attack;
		ac.StartAttack();

		isAiming = true;
	}

	public void AttackStatus()
    {
		//Durig the Attack Status
		Aim(moveDirection);
		Move(0f);
	}

	public void AttackShoot()
	{
		InstantlyAdjustPlayerRotation();
		weapon.Shoot();
		ChangeAmmunnitionReserve(-1);
		shootCoolDownCounter = shootCoolDown;
	}

	public void AttackExit()
	{
		//Leaving the Attack Status
		isAiming = false;
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
		Move(0f);

		//StartWaddle
		if (Mathf.Abs(moveDirection.x) > m_threshhold)
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
		//During Waddle

		//Movement
		Move(moveDirection.x);

		//Animation
		ac.SetSpeed(Mathf.Abs(moveDirection.x) * m_animMaxSpeed);

		//StartIdle
		if (Mathf.Abs(moveDirection.x) < m_threshhold)
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


	public void ChangeAmmunnitionReserve(int add)
	{
		playerStats.ammunitionLeft += add;
		//playerUI.UpdateAmmunitionReserveCount(playerStats.ammunitionLeft); //AUSKOMMENTIERT WEIL ERROR
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
		aimingIndicator.gameObject.SetActive(true);

		aimingIndicator.transform.eulerAngles = new Vector3(
			aimingRange * moveDirection.y,
			aimingIndicator.transform.eulerAngles.y,
			aimingIndicator.transform.eulerAngles.z
			);
	}

	private void AdjustPlayerRotation(float directionX)
	{
		float targetAngle = transform.eulerAngles.y;

		if (directionX > 0)
		{
			targetAngle = -90;
		}
		else if (directionX < 0)
		{
			targetAngle = 90;
		}

		float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
		transform.rotation = Quaternion.Euler(0f, angle, 0f);
	}

	private void InstantlyAdjustPlayerRotation()
	{
		float targetAngle = transform.eulerAngles.y < 0 || transform.eulerAngles.y > 180
			? -90
			: 90;

		transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
	}

	private void OnDrawGizmos()
	{
		if (isAiming)
		{
			Gizmos.DrawLine(weapon.shootPoint.position, weapon.shootPoint.position + weapon.shootPoint.transform.forward);
		}
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
