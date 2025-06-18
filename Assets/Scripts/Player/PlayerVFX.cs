using UnityEngine;

public class PlayerVFX : MonoBehaviour
{
    public Movement Movement;
    public ParticleSystem MoveVFX;

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
