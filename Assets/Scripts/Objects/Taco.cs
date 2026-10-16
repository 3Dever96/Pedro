using UnityEngine;

/// <summary>
/// A concrete implementation of a collectable item (a Taco).
/// Utilizes a brief state-based cooldown timer upon spawning to prevent accidental immediate pickups.
/// </summary>
public class Taco : MonoBehaviour, ICollectable
{
    /// <summary> The current lifecycle status of the item, dictating whether it can actively be harvested. </summary>
    private CollectableState state = CollectableState.Spawn;

    /// <summary> Time duration in seconds before the spawned item settles and becomes active for collection. </summary>
    private float restTime = 0.5f;

    private void Update()
    {
        // Countdown the activation delay timer over frame-execution intervals
        if (restTime > 0f)
        {
            restTime -= Time.deltaTime;
        }
        else
        {
            // Transition the state to allow player collection once the countdown completes
            state = CollectableState.Rest;
        }
    }

    /// <summary>
    /// Interface method implementation from ICollectable. Handles final disposal 
    /// and collection processing (such as adding score or health modifications).
    /// </summary>
    public void OnCollect()
    {
        // TODO: Integrate score increments, sound fx triggers, or stat updates here!

        // Deactivate the object to safely remove it from the active game space
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Collection Guard: Only process intersection math if the item has finished settling
        if (state == CollectableState.Rest)
        {
            // Validate if the entering physical actor is indeed the Player agent context
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                // Execute the collection routine
                OnCollect();
            }
        }
    }
}

/// <summary>
/// Defines the interactive lifecycle status of a collectable item in the game world.
/// </summary>
public enum CollectableState
{
    /// <summary> Freshly generated entity phase; locked out from interaction equations to ensure structural safety. </summary>
    Spawn,

    /// <summary> Settled, static state; fully open for player collection interaction. </summary>
    Rest
}
