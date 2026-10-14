using UnityEngine;

[System.Serializable]
public class PlayerWaddleState : PlayerState
{
    [SerializeField] private float waddleSpeed;
    private Vector3 targetPosition;
    private Vector3 lookDirection;

    public override void StartState(PlayerController player)
    {
        player.VerticalSpeed = -5f;
        targetPosition = player.transform.position;
    }

    public override void UpdateState(PlayerController player)
    {
        if (Vector3.Distance(targetPosition, player.transform.position) < 0.01f)
        {
            player.MoveDirection = Vector3.zero;
            player.CurrentSpeed = 0f;

            if (player.transform.position != targetPosition)
            {
                player.Controller.enabled = false;
                player.transform.position = targetPosition;
                player.Controller.enabled = true;
            }

            float x = Mathf.Abs(InputHub.Instance.Move.x) > 0.5f ? Mathf.Sign(InputHub.Instance.Move.x) : 0f;
            float z = x == 0f && Mathf.Abs(InputHub.Instance.Move.y) > 0.5f ? Mathf.Sign(InputHub.Instance.Move.y) : 0f;

            Vector3 desiredPostion = player.transform.position + new Vector3(x, 0f, z);
            Vector3 direction = desiredPostion - player.transform.position;

            lookDirection = direction;

            Ray ray = new Ray(player.transform.position + Vector3.up * 0.4f, direction);

            if (!Physics.Raycast(ray, 1.5f, LayerMask.GetMask("Solid")))
            {
                targetPosition = desiredPostion;
                player.MoveDirection = (targetPosition - player.transform.position).normalized;
            }
        }
        else
        {
            player.CurrentSpeed = waddleSpeed;
        }

        player.LookDirection = lookDirection != Vector3.zero ? lookDirection : player.LookDirection;
        player.FaceDirection();
    }

    public override void ChangeState(PlayerController player)
    {
        
    }

    public override void ExitState(PlayerController player)
    {
        
    }
}
