using UnityEngine;

/// <summary>
/// Agent which moves through a character controller.
/// </summary>
public class CharacterAgent : TransformAgent
{
    /// <summary>
    /// This agent's character controller.
    /// </summary>
    protected CharacterController CharacterController;

    /// <summary>
    /// Used to manually apply gravity.
    /// </summary>
    private float _velocityY;

    protected override void Start()
    {
        base.Start();
            
        // Get the character controller.
        CharacterController = GetComponent<CharacterController>();
        if (CharacterController == null)
        {
            CharacterController = gameObject.AddComponent<CharacterController>();
        }
    }

    /// <summary>
    /// Character controller movement.
    /// </summary>
    public override void Move()
    {
        if (CharacterController == null)
        {
            return;
        }
        
        // Reset gravity if grounded.
        if (CharacterController.isGrounded)
        {
            _velocityY = 0;
        }
        
        // Apply gravity.
        _velocityY += Physics.gravity.y * Time.deltaTime;

        CalculateMoveVelocity(Time.deltaTime);
        Vector2 scaled = MoveVelocity * Time.deltaTime;
        CharacterController.Move(new Vector3(scaled.x, _velocityY, scaled.y));
    }
}