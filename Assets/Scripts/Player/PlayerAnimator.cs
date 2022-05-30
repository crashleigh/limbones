using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private IPlayerCOntroller player;
    [SerializeField] private Animator animator;

    private void Awake()
    {
        player = GetComponentInParent<IPlayerCOntroller>();

    }

    private void Update()
    {
        if (player == null) return;

        #region Grounded
        if (player.Grounded && player.Input.X != 0)
        {
            animator.SetTrigger("IsRunning");
        }
        else
        {
            animator.SetTrigger("IsIdle");
        }


        if (!player.Grounded)
        {
            animator.ResetTrigger("IsIdle");
            animator.ResetTrigger("IsRunning");
        }
        #endregion


        #region Climbing
        if (!player.IsClimbing)
        {
            animator.SetBool("IsClimbing", false);
            animator.SetBool("IsClimbingIdle", false);
            animator.SetBool("IsFalling", true);
        }

        if (player.Input.X == 0 && player.IsClimbing)
        {
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsFalling", false);
            animator.SetBool("IsClimbingIdle", true);
            animator.SetBool("IsClimbing", false);
        }
        else if (player.IsClimbing && player.Input.X > 0 || player.Input.X < 0)
        {
            animator.SetBool("IsClimbingIdle", false);
            animator.SetBool("IsClimbing", true);
        }
        #endregion

        if (player.IsWallSliding)
        {
            animator.SetBool("IsWallSliding", true);
        }
        if (!player.IsWallSliding)
        {
            animator.SetBool("IsWallSliding", false);
        }


        #region Jump&Fall

        if (player.Grounded)
        {
            animator.SetBool("IsWallSLiding", false);
            animator.SetBool("IsFalling", false);
            animator.SetBool("IsJumping", false);
        }
        if (player.IsClimbing)
        {
            animator.SetBool("IsFalling", false);
            animator.SetBool("IsJumping", false);
        }

        if (player.Input.JumpHeld && player.Grounded)
        {
            animator.SetBool("IsWallSLiding", false);
            animator.SetBool("IsJumping", false);
        }

        if (player.Input.JumpHeld)
        {
            animator.SetBool("IsWallSLiding", false);
            animator.SetBool("IsJumping", true);
        }
        else if (!player.Grounded && !player.IsClimbing && !player.Input.JumpHeld)
        {
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsFalling", true);
        }

        #endregion

        if (FindObjectOfType<PlayerController>().running)
        {
            animator.SetBool("IsDie", true);
        }
    }
}
