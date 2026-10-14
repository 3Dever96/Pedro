using UnityEngine;

/// <summary>
/// Handles standard, tile-by-tile grid movement when the player is exploring the maze normally.
/// </summary>
[System.Serializable]
public class PlayerWaddleState : PlayerState
{
    [SerializeField] private float waddleSpeed;
    private Vector3 targetPosition;
    private Vector3 lookDirection;

    public override void StartState(PlayerController player)
    {
        // Apply a gentle constant downward force to ensure the controller remains grounded
        player.VerticalSpeed = -5f;

        // Pin the initial movement destination to the player's exact starting point
        targetPosition = player.transform.position;
    }

    public override void UpdateState(PlayerController player)
    {
        // Check if the player has arrived at (or is extremely close to) the center of their target tile
        if (Vector3.Distance(targetPosition, player.transform.position) < 0.01f)
        {
            player.MoveDirection = Vector3.zero;
            player.CurrentSpeed = 0f;

            // Absolute position correction to clean up minute floating point drift
            if (player.transform.position != targetPosition)
            {
                player.Controller.enabled = false;
                player.transform.position = targetPosition;
                player.Controller.enabled = true;
            }

            // Read Input System directions and discretize them strictly to -1, 0, or 1
            float x = Mathf.Abs(InputHub.Instance.Move.x) > 0.5f ? Mathf.Sign(InputHub.Instance.Move.x) : 0f;

            // Prioritize X axis movement; if moving on X, completely disregard Z movement to enforce strict orthogonal paths
            float z = x == 0f && Mathf.Abs(InputHub.Instance.Move.y) > 0.5f ? Mathf.Sign(InputHub.Instance.Move.y) : 0f;

            // Calculate the absolute world position of the next target tile
            Vector3 desiredPostion = player.transform.position + new Vector3(x, 0f, z);
            Vector3 direction = desiredPostion - player.transform.position;

            lookDirection = direction;

            // Cast a ray forward at waist height to verify the path to the next tile is unobstructed
            Ray ray = new Ray(player.transform.position + Vector3.up * 0.4f, direction);

            if (!Physics.Raycast(ray, 1.5f, LayerMask.GetMask("Solid")))
            {
                targetPosition = desiredPostion;
                player.MoveDirection = (targetPosition - player.transform.position).normalized;
            }
        }
        else
        {
            // Maintain continuous movement towards the designated tile center if not yet arrived
            player.CurrentSpeed = waddleSpeed;
        }

        // Apply turning rotations smoothly based on computed movement directions
        player.LookDirection = lookDirection != Vector3.zero ? lookDirection : player.LookDirection;
        player.FaceDirection();
    }

    public override void ChangeState(PlayerController player)
    {
        // Only allow state transitions if the player has safely halted completely on a clean grid intersection
        if (player.transform.position == targetPosition)
        {
            // Transition immediately to charging state if the charge input action is held down
            if (InputHub.Instance.Charge)
            {
                player.SetState(player.ChargeState);
            }
        }
    }

    public override void ExitState(PlayerController player)
    {
        // Cleanup hooks not required for this state transition
    }
}
