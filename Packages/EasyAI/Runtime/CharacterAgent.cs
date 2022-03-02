using UnityEngine;

/// <summary>
/// Agent which moves through a character controller.
/// </summary>
public class CharacterAgent : TransformAgent
{
    /// <summary>
    /// This agent's character controller.
    /// </summary>
    private CharacterController _characterController;

    /// <summary>
    /// Used to manually apply gravity.
    /// </summary>
    private float _velocityY;

    protected override void Start()
    {
        base.Start();
            
        // Get the character controller.
        _characterController = GetComponent<CharacterController>();
        if (_characterController == null)
        {
            _characterController = gameObject.AddComponent<CharacterController>();
        }
    }

    /// <summary>
    /// Character controller movement.
    /// </summary>
    public override void Move()
    {
        if (_characterController == null)
        {
            return;
        }
        
        // Reset gravity if grounded.
        if (_characterController.isGrounded)
        {
            _velocityY = 0;
        }
        
        // Apply gravity.
        _velocityY += Physics.gravity.y * Time.deltaTime;

        CalculateMoveVelocity(Time.deltaTime);
        _characterController.Move(new Vector3(MoveVelocity.x, _velocityY, MoveVelocity.y));
    }
}