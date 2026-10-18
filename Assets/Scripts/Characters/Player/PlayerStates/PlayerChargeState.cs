using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Forces the player to stop moving while they hold down the charge button, tracking 
/// a charge meter and orienting their direction towards intended propulsion.
/// </summary>
[System.Serializable]
public class PlayerChargeState : PlayerState
{
    [SerializeField] private float slideSpeed;

    [SerializeField] private float maxCharge;
    [SerializeField] private float chargeSpeed;
    [SerializeField] private float currentCharge;

    [Header("UI References")]
    [SerializeField] private GameObject canvas;
    [SerializeField] private Slider chargeMeter;
    [SerializeField] private Image fillImage;
    [SerializeField] private Gradient fillColor;

    private Vector3 lookDirection;

    public override void StartState(PlayerController player)
    {
        // Immediately halt all forward and structural movement vectors
        player.CurrentSpeed = 0f;
        player.MoveDirection = Vector3.zero;

        currentCharge = 0f;

        // Initialize the charging visual slider HUD and align its perspective with the primary camera view
        if (canvas != null)
        {
            canvas.SetActive(true);
            chargeMeter.transform.forward = Camera.main.transform.forward;
            chargeMeter.value = 0f;
        }
    }

    public override void UpdateState(PlayerController player)
    {
        // Discretize directional user commands to allow aiming orthogonal slide vectors while charging
        float x = Mathf.Abs(InputHub.Instance.Move.x) > 0.5f ? Mathf.Sign(InputHub.Instance.Move.x) : 0f;
        float z = x == 0f && Mathf.Abs(InputHub.Instance.Move.y) > 0.5f ? Mathf.Sign(InputHub.Instance.Move.y) : 0f;

        Vector3 desiredPostion = player.transform.position + new Vector3(x, 0f, z);
        Vector3 direction = desiredPostion - player.transform.position;

        lookDirection = direction;

        // Rotate the player to look directly in the direction they intend to fire themselves into
        player.LookDirection = lookDirection != Vector3.zero ? lookDirection : player.LookDirection;
        player.FaceDirection();

        // Incrementally build up the charge scale safely within the 0 to max bounds
        currentCharge = Mathf.Clamp(currentCharge + chargeSpeed * Time.deltaTime, 0f, maxCharge);

        // Update the UI Slider metrics and evaluate the corresponding normalized color gradient shift
        if (canvas != null)
        {
            chargeMeter.value = currentCharge;
            fillImage.color = fillColor.Evaluate(currentCharge / maxCharge);
            chargeMeter.transform.forward = Camera.main.transform.forward; // Ensure UI faces camera consistently
        }
    }

    public override void ChangeState(PlayerController player)
    {
        // As soon as the player releases the charge input trigger, transition straight into active propulsion
        if (!InputHub.Instance.Charge)
        {
            player.SetState(player.SlideState);
        }
    }

    public override void ExitState(PlayerController player)
    {
        // Disable the HUD overlay canvas and calculate initial propulsion speed mapped proportionally to the charge scale
        if (canvas != null)
        {
            canvas.SetActive(false);
            player.CurrentSpeed = slideSpeed * (currentCharge / maxCharge);
            player.MoveDirection = player.transform.forward;
        }
    }
}
