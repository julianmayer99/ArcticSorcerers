using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class JumpPad : MonoBehaviour
{
    public JumpPadMode mode = JumpPadMode.ApplyForceOnJump;
    public Vector3 extraForceOnExit = new Vector3(0f, 600f);

    private void OnCollisionEnter(Collision collision)
    {
        var player = collision.gameObject.GetComponent<PlayerController>();

        if (player == null)
            return;

        if (mode == JumpPadMode.ApplyForceOnContact)
        {
            AddForceOnJump(player);
            return;
        }

        player.OnJumpEvent.RemoveListener(AddForceOnJump); // Avoid double triggering
        player.OnJumpEvent.AddListener(AddForceOnJump);
    }

    void AddForceOnJump(PlayerController player)
    {
        // Is the player still standing on the platform?
        if (player.GetComponent<BoxCollider>().bounds.Intersects(GetComponent<Collider>().bounds))
        {
            player.m_Rigidbody.AddForce(extraForceOnExit);
        }

        player.OnJumpEvent.RemoveListener(AddForceOnJump);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        var player = collision.gameObject.GetComponent<PlayerController>();

        if (player == null)
            return;

        player.OnJumpEvent.RemoveListener(AddForceOnJump);
    }

    public enum JumpPadMode
    {
        ApplyForceOnContact,
        ApplyForceOnJump
    }
}
