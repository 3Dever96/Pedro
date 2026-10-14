using TMPro;
using UnityEngine;

/// <summary>
/// Dictates high-speed forward momentum subject to physical friction deceleration 
/// or abrupt termination upon collision with solid terrain features.
/// </summary>
[System.Serializable]
public class PlayerSlideState : PlayerState
{
    [SerializeField] private float friction;

    public override void StartState(PlayerController player)
    {
        // Slide values and directional vectors are derived dynamically from PlayerChargeState.ExitState
    }

    public override void UpdateState(PlayerController player)
    {
        // Construct a forward detection ray offset slightly upwards to miss the floor plane geometry
        Ray ray = new Ray(player.transform.position + Vector3.up * 0.4f, player.transform.forward);

        // If a solid object blocks the path 1.5 units ahead, stop instantly
        if (Physics.Raycast(ray, 0.5f, LayerMask.GetMask("Solid")))
        {
            player.CurrentSpeed = 0f;
        }
        else
        {
            // Linearly bleed away horizontal velocity over time using the friction parameter
            player.CurrentSpeed = Mathf.Clamp(player.CurrentSpeed - friction * Time.deltaTime, 0f, 10f);
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
