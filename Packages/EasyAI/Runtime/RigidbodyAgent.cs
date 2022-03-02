using UnityEngine;

/// <summary>
/// Agent which moves through a rigidbody.
/// </summary>
public class RigidbodyAgent : Agent
{
    /// <summary>
    /// This agent's rigidbody.
    /// </summary>
    private Rigidbody _rigidbody;

    protected override void Start()
    {
        // Get the rigidbody.
        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody == null)
        {
            _rigidbody = gameObject.AddComponent<Rigidbody>();
        }

        // Since rotation is all done with the root visuals transform, freeze rigidbody rotation.
        if (_rigidbody != null)
        {
            _rigidbody.freezeRotation = true;
        }
    }
        
    public override void Move()
    {
        if (_rigidbody == null)
        {
            return;
        }
        
        CalculateMoveVelocity(Time.fixedDeltaTime);
        _rigidbody.velocity = new Vector3(MoveVelocity.x, _rigidbody.velocity.y, MoveVelocity.y);
    }
}