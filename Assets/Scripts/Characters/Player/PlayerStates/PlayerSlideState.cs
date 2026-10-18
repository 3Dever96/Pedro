using TMPro;
using UnityEngine;

/// <summary>
/// Dictates high-speed forward momentum subject to physical friction deceleration 
/// or abrupt termination upon collision with solid terrain features.
/// Handles dynamic object destruction when sliding into breakable obstacles.
/// </summary>
[System.Serializable]
public class PlayerSlideState : PlayerState
{
    [Tooltip("The base rate at which slide velocity decays over time.")]
    [SerializeField] private float friction;

    /// <summary> The actively calculated friction coefficient, which can dynamically increase upon impact. </summary>
    private float currentFriction;

    public override void StartState(PlayerController player)
    {
        // Initialize the runtime friction value with the base configuration value
        currentFriction = friction;
    }

    public override void UpdateState(PlayerController player)
    {
        // Construct a forward detection ray offset slightly upwards to miss the floor plane geometry
        Ray ray = new Ray(player.transform.position + Vector3.up * 0.4f, player.transform.forward);
        RaycastHit hit;

        // Perform a short-range raycast directly ahead to check for impending wall impacts or obstacles
        if (Physics.Raycast(ray, out hit, 0.5f, LayerMask.GetMask("Solid")))
        {
            // Check if the collided solid object implements the breakable behavior interface
            IBreakable breakable = hit.collider.GetComponent<IBreakable>();

            if (breakable != null)
            {
                // Trigger the destruction/interaction routine on the target entity
                breakable.OnBreak();

                // Add a permanent friction penalty for this slide to simulate losing momentum from the smash
                currentFriction += 1.5f;
            }
            else
            {
                // Unbreakable obstacle encountered: Bring the sliding velocity to a dead stop immediately
                player.CurrentSpeed = 0f;
            }
        }
        else
        {
            // Smoothly drain away forward velocity over time using the actively scaling friction metric
            player.CurrentSpeed = Mathf.Clamp(player.CurrentSpeed - currentFriction * Time.deltaTime, 0f, 10f);
        }
    }

    public override void ChangeState(PlayerController player)
    {
        // Once friction or impact brings velocity to a total stop, transition into the grid docking phase
        if (player.CurrentSpeed <= 0f)
        {
            player.SetState(player.CoolState);
        }
    }

    public override void ExitState(PlayerController player)
    {
        // Teardown hooks not required for this state transition
    }
}
