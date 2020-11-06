using Assets.Scripts.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
	//Status of the Player
	private enum status { Idle, Waddle, Attack}


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

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;
	public UnityEvent OnPlayerWasShot;
	public UnityEvent OnPlayerDied;

	private void Awake()
	{
		m_Rigidbody = GetComponent<Rigidbody>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();
		if (OnPlayerWasShot == null)
			OnPlayerWasShot = new UnityEvent();
		if (OnPlayerDied == null)
			OnPlayerDied = new UnityEvent();

		shootCoolDownCounter = shootCoolDown;
	}

	private void Start()
	{
		config = FindObjectOfType<GameManager>().RegisterPlayerAndGetConfiguration(this);
		playerUI = Instantiate(playerUiPreFab, FindObjectOfType<Canvas>().transform).GetComponent<FloatingPlayerGuiHandler>();

		playerUI.SetUpFloatingGui(this, FindObjectOfType<GameManager>().dynamicCamera.GetComponent<Camera>());
		InvokeRepeating(nameof(RecalculateDistanceCovered), 1f, 1f);
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

	private void OnDestroy()
	{
		DynamicMultiTargetCamera.instance.targets.Remove(this.transform);
	}

	private void FixedUpdate()
	{
		if (shootCoolDownCounter > 0)
			shootCoolDownCounter--;
	}

	private void Update()
	{
		if (isAiming)
		{
			Aim(moveDirection);
			Move(0f);
		}
		else
		{
			Move(moveDirection.x);
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
		Debug.Log("Jump called.");
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
		var jump = context.ReadValue<float>();
		Debug.Log("OnJumpPerformed " + jump);
		Jump(jump >= 1);
	}

	private bool wasAiming = false;
	public void OnShootPerformed(InputAction.CallbackContext context)
	{
		if (shootCoolDownCounter > 0 || playerStats.ammunitionLeft < 1)
		{
			isAiming = false;
			wasAiming = isAiming;
			return;
		}

		var shootButtonDown = context.ReadValue<float>() >= 1;

		if (!shootButtonDown && wasAiming != shootButtonDown)
		{
			InstantlyAdjustPlayerRotation();
			weapon.Shoot();
			ChangeAmmunnitionReserve(-1);
			shootCoolDownCounter = shootCoolDown;
		}

		isAiming = shootButtonDown;
		wasAiming = isAiming;
	}

	public void ChangeAmmunnitionReserve(int add)
	{
		playerStats.ammunitionLeft += add;
		playerUI.UpdateAmmunitionReserveCount(playerStats.ammunitionLeft);
	}

	public void OnPlayerHasBeenShot(PlayerController fromPlayer, Vector3 shotPoint)
	{
		var forceDirection = (transform.position - shotPoint).normalized;
		if (forceDirection.y < shotKnockUpwardsForce)
			forceDirection.y = shotKnockUpwardsForce;

		m_Rigidbody.AddForce(forceDirection * shotKnockBack);

		OnPlayerWasShot.Invoke();
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
}
