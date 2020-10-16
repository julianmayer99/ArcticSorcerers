using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
	[SerializeField] private float m_JumpForce = 400f;
	[Range(1, 20f)] [SerializeField] private float m_Speed = 10f;
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;

	[SerializeField] private float turnSmoothTime = .1f;
	private float turnSmoothVelocity;

	const float k_GroundedRadius = .2f;
	private bool m_Grounded;
	private Rigidbody m_Rigidbody;
	private Vector3 m_Velocity = Vector3.zero;

	private Vector2 moveDirection = Vector2.zero;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	private void Awake()
	{
		m_Rigidbody = GetComponent<Rigidbody>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();
	}

	private void Start()
	{
		DynamicMultiTargetCamera.instance.targets.Add(this.transform);
	}

	private void OnDestroy()
	{
		DynamicMultiTargetCamera.instance.targets.Remove(this.transform);
	}

	private void Update()
	{
		Move(moveDirection.x);
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

		float targetAngle = 0f;

		if (move > 0)
		{
			targetAngle = -90;
		}
		else if (move < 0)
		{
			targetAngle = 90;

		}
		float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
		transform.rotation = Quaternion.Euler(0f, angle, 0f);
	}

	public void Jump(bool jump)
	{
		Debug.Log("Jump called.");
		if (m_Grounded && jump)
		{
			m_Grounded = false;
			m_Rigidbody.AddForce(new Vector2(0f, m_JumpForce));
		}
	}

	public void OnMovePerformed(InputAction.CallbackContext moveContext)
	{
		moveDirection = moveContext.ReadValue<Vector2>();
	}

	public void OnJumpPerformed(InputAction.CallbackContext moveContext)
	{
		var jump = moveContext.ReadValue<float>();
		Debug.Log("OnJumpPerformed " + jump);
		Jump(jump >= 1);
	}
}
