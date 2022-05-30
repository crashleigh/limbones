using UnityEngine;


public class PlayerInput : MonoBehaviour
{
    public FrameInput GatherInput()
    {
        return new FrameInput
        {
            JumpDown = Input.GetButtonDown("Jump"),
            JumpHeld = Input.GetButton("Jump"),
            DashDown = Input.GetButtonDown("Dash"),
            CrouchDown = Input.GetButtonDown("Crouch"),

            X = Input.GetAxisRaw("Horizontal"),
            Y = Input.GetAxisRaw("Vertical")
        };

    }
}
