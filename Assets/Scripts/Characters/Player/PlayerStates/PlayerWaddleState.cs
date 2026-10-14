using UnityEngine;

[System.Serializable]
public class PlayerWaddleState : PlayerState
{
    [SerializeField] private float waddleSpeed;

    public override void StartState(PlayerController player)
    {
        player.VerticalSpeed = -5f;
    }

    public override void UpdateState(PlayerController player)
    {
        Vector3 direction = Vector3.right * InputHub.Instance.Move.x + Vector3.forward * InputHub.Instance.Move.y;
        direction.y = 0f;
        player.MoveDirection = direction.normalized;

        player.CurrentSpeed = InputHub.Instance.Move != Vector2.zero ? waddleSpeed : 0f;
        player.LookDirection = player.MoveDirection != Vector3.zero ? player.MoveDirection : player.LookDirection;

        player.FaceDirection();
    }

    public override void ChangeState(PlayerController player)
    {
        
    }

    public override void ExitState(PlayerController player)
    {
        
    }
}
