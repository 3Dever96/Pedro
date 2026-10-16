using UnityEngine;

/// <summary>
/// A concrete implementation of a breakable obstacle (a crate).
/// When smashed by the player, it randomly determines whether to spawn loot/content from an array of possibilities.
/// </summary>
public class Crate : MonoBehaviour, IBreakable
{
    [Header("Loot Configuration")]
    [Tooltip("An array of prefabs (powerups, items, coins) that this crate could potentially drop upon breaking.")]
    [SerializeField] private GameObject[] possibleContents;

    /// <summary> The specific item chosen to drop from this crate instance. Remains null if the crate contains nothing. </summary>
    private GameObject contents;

    /// <summary>
    /// Evaluates random item generation conditions. Has a 50% chance to assign a random drop asset from the configuration library.
    /// </summary>
    public void OnSpawn()
    {
        // 50% probability check to determine if this crate holds items or is empty
        bool hasContents = Random.value > 0.5f;

        if (hasContents && possibleContents != null && possibleContents.Length > 0)
        {
            // Pick a completely random index within the bounds of the drop table array
            int index = Random.Range(0, possibleContents.Length);

            // Assign the selected asset to be held as contents
            contents = possibleContents[index];
        }
    }

    /// <summary>
    /// Interface method implementation from IBreakable. Called instantly when a sliding player collides with this crate.
    /// Spawns the held item drop and deactivates the crate object.
    /// </summary>
    public void OnBreak()
    {
        // If an item was successfully selected during initialization, manifest it in the game world
        if (contents != null)
        {
            Instantiate(contents, transform.position + new Vector3(0.5f, 0f, 0.5f), transform.rotation);
        }

        // Disable the crate object, taking it out of physical play and hiding it from view
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        OnSpawn();
    }
}
