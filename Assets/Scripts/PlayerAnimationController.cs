using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField]
    private GameObject penguin;
    private Animator penguinAnimator;
    [Space]
    [SerializeField]
    private bool dynamicTransformAnimation = true;
    private Animator dynamicPenguinTransformAnimator;

    private void Awake()
    {
        penguinAnimator = penguin.GetComponent<Animator>();
        dynamicPenguinTransformAnimator = GetComponent<Animator>();
    }
    public void StartIdle()
    {
        penguinAnimator.SetTrigger("Idle");
    }
    public void StartWaddle() 
    {
        penguinAnimator.SetTrigger("Waddle");
    }

    public void StartAttack()
    {
        penguinAnimator.SetTrigger("Attack");
    }

    public void StartDash()
    {
        penguinAnimator.SetTrigger("Dash");
    }

    public void SetSpeed(float s)
    {
        penguinAnimator.speed = s;
    }

    public void Jump()
    {
        if (dynamicTransformAnimation)
            dynamicPenguinTransformAnimator.SetTrigger("jump");
    }

    public void Land()
    {
        if (dynamicTransformAnimation)
            dynamicPenguinTransformAnimator.SetTrigger("land");
    }

}
