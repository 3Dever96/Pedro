using UnityEngine;

/// <summary>
/// Abstract base class representing a generic state within the Player State Machine.
/// Every state must override these four lifecycle hooks.
/// </summary>
[System.Serializable]
public abstract class PlayerState
{
    /// <summary> Called once when the state machine transitions into this state. </summary>
    public abstract void StartState(PlayerController player);

    /// <summary> Called every frame from Update() while this state is active. Handles mechanics and movement physics. </summary>
    public abstract void UpdateState(PlayerController player);

    /// <summary> Called every frame from Update() to evaluate if a transition conditions to a new state is met. </summary>
    public abstract void ChangeState(PlayerController player);

    /// <summary> Called once when the state machine exits this state, before entering the next one. </summary>
    public abstract void ExitState(PlayerController player);
}
