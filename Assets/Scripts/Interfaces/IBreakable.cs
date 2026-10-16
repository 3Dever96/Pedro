using UnityEngine;

/// <summary>
/// Defines an interface for any interactive obstacle or environmental hazard 
/// within the maze that can be shattered or destroyed by a player's slide attack.
/// </summary>
public interface IBreakable
{
    /// <summary>
    /// Triggered automatically when an entity with high enough velocity (like a sliding player) 
    /// collides with this object's physical boundaries.
    /// </summary>
    public void OnBreak()
    {
        // Default implementation block. 
        // Can be left empty for optional behavior, or overridden by inheriting MonoBehaviours 
        // to instantiate fracture particle effects, play smash audio, or trigger item drops.
    }
}
