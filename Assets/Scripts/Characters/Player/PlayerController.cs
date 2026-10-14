using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public CharacterController Controller { get; private set; }

    public PlayerState CurrentState {  get; private set; }
    [field: SerializeField] public PlayerWaddleState WaddleState { get; private set; } = new PlayerWaddleState();
    [field: SerializeField] public PlayerChargeState ChargeState { get; private set; } = new PlayerChargeState();
    [field: SerializeField] public PlayerSlideState SlideState { get; private set; } = new PlayerSlideState();

    public float CurrentSpeed { get; set; }
    public float VerticalSpeed {  get; set; }
    public Vector3 MoveDirection {  get; set; }
    public Vector3 LookDirection {  get; set; }

    private void Start()
    {
        Controller = GetComponent<CharacterController>();

        SetState(WaddleState);
    }

    private void Update()
    {
        if (CurrentState != null)
        {
            CurrentState.UpdateState(this);
            CurrentState.ChangeState(this);
        }

        ApplyMovement();
    }

    public void SetState(PlayerState newState)
    {
        if (CurrentState != null)
        {
            CurrentState.ExitState(this);
        }

        CurrentState = newState;

        if (CurrentState != null)
        {
            CurrentState.StartState(this);
        }
    }

    public void FaceDirection(float turnSpeed = 500f)
    {
        if (LookDirection == Vector3.zero) return;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(LookDirection), turnSpeed * Time.deltaTime);
    }

    private void ApplyMovement()
    {
        Vector3 velocity = CurrentSpeed * MoveDirection;
        velocity.y = VerticalSpeed;

        Controller.Move(velocity * Time.deltaTime);
    }
}
