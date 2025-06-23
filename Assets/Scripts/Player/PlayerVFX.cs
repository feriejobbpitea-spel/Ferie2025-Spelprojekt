using System;
using UnityEngine;

public class PlayerVFX : MonoBehaviour
{
    public Movement Movement;
    public ParticleSystem MoveVFX;
    public ParticleSystem JumpVFX;

    private void OnEnable()
    {
        Movement.OnJump += HandleJump;
    }

    private void OnDisable()
    {
        Movement.OnJump -= HandleJump;
    }

    private void HandleJump()
    {
        JumpVFX.Play();
    }

    private void Update()
    {
        if (Movement.IsMoving && Movement.IsGrounded)
        {
            if (!MoveVFX.isPlaying)
                MoveVFX.Play(true);
        }
        else
        {
            if (MoveVFX.isPlaying)
                MoveVFX.Stop(true);
        }
    }
}
