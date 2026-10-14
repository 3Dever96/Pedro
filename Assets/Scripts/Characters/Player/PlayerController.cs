using UnityEngine;

/// <summary>
/// The main driver for the player entity. Manages the state machine context,
/// tracks movement metrics, and executes the final movement via a CharacterController.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // ==========================================
    // COMPONENTS & REFERENCES
    // ==========================================

    /// <summary> Reference to the attached Unity CharacterController component. </summary>
    public CharacterController Controller { get; private set; }

    // ==========================================
    // STATE MACHINE CONTEXT
    // ==========================================

    /// <summary> The state that is currently active and executing logic. </summary>
    public PlayerState CurrentState { get; private set; }

    /// <summary> State handling standard, tile-by-tile grid movement. </summary>
    [field: SerializeField] public PlayerWaddleState WaddleState { get; private set; } = new PlayerWaddleState();

    /// <summary> State handling the button-hold preparation for a slide. </summary>
    [field: SerializeField] public PlayerChargeState ChargeState { get; private set; } = new PlayerChargeState();

    /// <summary> State handling the high-speed physical propulsion phase. </summary>
    [field: SerializeField] public PlayerSlideState SlideState { get; private set; } = new PlayerSlideState();

    /// <summary> State handling post-slide recovery or brief cooldown. </summary>
    [field: SerializeField] public PlayerCoolState CoolState { get; private set; } = new PlayerCoolState();

    // ==========================================
    // MOVEMENT & ROTATION VARIABLES
    // ==========================================

    /// <summary> Current horizontal movement velocity scale. </summary>
    public float CurrentSpeed { get; set; }

    /// <summary> Current vertical velocity (used for gravity or vertical alignment). </summary>
    public float VerticalSpeed { get; set; }

    /// <summary> Normalized heading direction vector for horizontal movement. </summary>
    public Vector3 MoveDirection { get; set; }

    /// <summary> Target direction vector the player mesh should rotate toward. </summary>
    public Vector3 LookDirection { get; set; }

    // ==========================================
    // UNITY LIFECYCLE METHODS
    // ==========================================

    private void Start()
    {
        // Cache the required CharacterController reference
        Controller = GetComponent<CharacterController>();

        // Initialize the state machine by entering the default waddling state
        SetState(WaddleState);
    }

    private void Update()
    {
        // Execute the logic loop for the active state
        if (CurrentState != null)
        {
            CurrentState.UpdateState(this); // Process state actions (e.g. movement input)
            CurrentState.ChangeState(this); // Check for state transitions
        }

        // Apply velocities to the CharacterController after the state updates
        ApplyMovement();
    }

    // ==========================================
    // STATE SYSTEM MANAGEMENT
    // ==========================================

    /// <summary>
    /// Safely transitions the state machine from the current state into a new state.
    /// </summary>
    /// <param name="newState">The instantiated PlayerState class to transition into.</param>
    public void SetState(PlayerState newState)
    {
        // Teardown the old state if one exists
        if (CurrentState != null)
        {
            CurrentState.ExitState(this);
        }

        // Assign the new state context
        CurrentState = newState;

        // Initialize the new state if it is valid
        if (CurrentState != null)
        {
            CurrentState.StartState(this);
        }
    }

    // ==========================================
    // UTILITY METHODS
    // ==========================================

    /// <summary>
    /// Smoothly rotates the player's transform toward the current LookDirection vector.
    /// </summary>
    /// <param name="turnSpeed">Degrees per second the player can rotate.</param>
    public void FaceDirection(float turnSpeed = 1000f)
    {
        // Guard clause: Do nothing if there is no target heading direction
        if (LookDirection == Vector3.zero) return;

        // Interpolate the rotation smoothly toward the target direction over time
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.LookRotation(LookDirection),
            turnSpeed * Time.deltaTime
        );
    }

    /// <summary>
    /// Compiles current horizontal and vertical speed metrics into a velocity vector, 
    /// then moves the CharacterController relative to frame execution time.
    /// </summary>
    private void ApplyMovement()
    {
        // Calculate horizontal heading speed
        Vector3 velocity = CurrentSpeed * MoveDirection;

        // Inject gravity or vertical adjustments
        velocity.y = VerticalSpeed;

        // Execute frame-rate independent movement via CharacterController
        Controller.Move(velocity * Time.deltaTime);
    }
}
