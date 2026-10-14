using UnityEngine;

/// <summary>
/// Executed post-slide. Smoothly guides and decelerates the player back to the 
/// center of the nearest whole tile on the grid, masking any physical positioning errors.
/// </summary>
[System.Serializable]
public class PlayerCoolState : PlayerState
{
    [SerializeField] private float waddleSpeed;
    private Vector3 tilePosition;

    public override void StartState(PlayerController player)
    {
        // Calculate the absolute grid coordinates of the nearest whole tile center when the slide finishes
        float x = Mathf.RoundToInt(player.transform.position.x);
        float z = Mathf.RoundToInt(player.transform.position.z);

        tilePosition = new Vector3(x, 0f, z);
    }

    public override void UpdateState(PlayerController player)
    {
        // Create a steering direction vector directly pointing from current position to the clean tile center
        player.MoveDirection = tilePosition - player.transform.position;
        player.LookDirection = player.MoveDirection;

        player.FaceDirection();

        // Apply fallback crawling speed to bridge the gap smoothly
        player.CurrentSpeed = waddleSpeed;
    }

    public override void ChangeState(PlayerController player)
    {
        // Once close enough to the exact tile node, exit cooldown loop and restore manual exploration
        if (Vector3.Distance(tilePosition, player.transform.position) < 0.1f)
        {
            player.SetState(player.WaddleState);
        }
    }

    public override void ExitState(PlayerController player)
    {
        // Toggle the CharacterController component safely to hard-snap the final position to absolute integers
        player.Controller.enabled = false;
        player.transform.position = tilePosition;
        player.Controller.enabled = true;

        // Correct visual mesh facing angle to snap exactly back into a clean grid direction
        player.LookDirection = GetProperDirection(player.transform.forward);
        player.FaceDirection(5000f);
    }

    /// <summary>
    /// Evaluates a free-form forward vector and snaps it strictly to the nearest clean 90-degree world direction vector.
    /// </summary>
    private Vector3 GetProperDirection(Vector3 forward)
    {
        float forwardAngle = Vector3.Angle(forward, Vector3.forward);
        float rightAngle = Vector3.Angle(forward, Vector3.right);

        // Check if closer to Forward or Backward vectors
        if (forwardAngle < 45f)
        {
            return Vector3.forward;
        }
        else if (forwardAngle > 135f)
        {
            return -Vector3.forward;
        }
        else
        {
            // Evaluate if orientation aligns best to Right or Left vectors
            if (rightAngle < 45f)
            {
                return Vector3.right;
            }
            else if (rightAngle > 135f)
            {
                return -Vector3.right;
            }
        }

        return Vector3.zero;
    }
}
