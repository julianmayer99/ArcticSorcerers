using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField]
    private GameObject penguin;
    private Animator penguinAnimator;

    private void Awake()
    {
        penguinAnimator = penguin.GetComponent<Animator>();
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

}
