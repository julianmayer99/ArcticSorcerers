using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField]
    private GameObject penguin;
    [SerializeField]
    private GameObject body;
    private Animator penguinAnimator;
    [Space]
    [SerializeField]
    private bool dynamicTransformAnimation = true;
    private Animator dynamicPenguinTransformAnimator;

    [Header("Materials")]
    [Space]
    public Material headBlack;
    public Material bodyBlack;

    [Space]
    public Material headBlue;
    public Material bodyBlue;

    [Space]
    public Material headGray;
    public Material bodyGray;

    private void Awake()
    {
        penguinAnimator = penguin.GetComponent<Animator>();
        dynamicPenguinTransformAnimator = GetComponent<Animator>();
    }

    public void SetColorBlack()
    {
        Debug.Log("BLACK");
        Material[] mats = body.GetComponent<Renderer>().sharedMaterials;
        mats[0] = bodyBlack;
        mats[1] = headBlack;
        body.GetComponent<Renderer>().sharedMaterials = mats;
    }

    public void SetColorBlue()
    {
        Debug.Log("BLUE");
        Material[] mats = body.GetComponent<Renderer>().sharedMaterials;
        mats[0] = bodyBlue;
        mats[1] = headBlue;
        body.GetComponent<Renderer>().sharedMaterials = mats;
    }

    public void SetColorGray()
    {
        Debug.Log("BLUE");
        Material[] mats = body.GetComponent<Renderer>().sharedMaterials;
        mats[0] = bodyGray;
        mats[1] = headGray;
        body.GetComponent<Renderer>().sharedMaterials = mats;
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

    public void StartDead()
    {
        penguinAnimator.SetTrigger("Dead");
        penguinAnimator.speed = 2f;
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
