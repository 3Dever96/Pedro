using UnityEngine;

/// <summary>
/// Defines an interface for any interactive entity or item drops within the maze 
/// that can be picked up, harvested, or consumed by the player upon contact.
/// </summary>
public interface ICollectable
{
    /// <summary>
    /// Triggered automatically when an actor (typically the player) successfully 
    /// interacts with or passes over this item's collection boundary trigger.
    /// </summary>
    public void OnCollect()
    {
        // Default implementation block.
        // Can be left empty for optional behavior, or overridden by inheriting MonoBehaviours 
        // to update UI player scores, apply gameplay stat modifiers, or trigger pickup audio tracks.
    }
}
