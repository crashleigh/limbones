using System;
using UnityEngine;

public struct FrameInput
{
    public float X, Y;
    public bool JumpDown;
    public bool JumpHeld;
    public bool DashDown;
    public bool CrouchDown;
}


public interface IPlayerCOntroller
{
    public FrameInput Input { get; }
    public Vector2 RawMovement { get; }
    public bool Grounded { get; }
    public bool IsClimbing { get; }
    public bool IsWallSliding { get; }
    public bool CurrentlyDying { get; }


    public event Action<bool> OnGroundedChanged;
    public event Action OnJumping;
    public event Action<bool> OnDashingChanged;
    public event Action<bool> OnCrouchingChanged;

    /// <summary>
    /// Add force to the character
    /// </summary>
    /// <param name="force">Force to be applied to the controller</param>
    /// <param name="mode">The Force application mode</param>
    /// <param name="cancelMovement">Cancelt the current velocity of teh player to provide a reliable reaction</param>

    public void AddForce(Vector2 force, PlayerForce mode = PlayerForce.Burst, bool cancelMovement = true);
}

public interface IPlayerEffector
{
    public Vector2 EvaluateEffector();
}


public enum PlayerForce
{
    ///<summary>
    ///Added directly to ther players movement speedm to be controlled by the standard decelartion
    ///</summary>
    Burst,
    ///<summary>
    /// An additive force handled by the decay system
    ///</summary>
    Decay

}
