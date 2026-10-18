using UnityEngine;

/// <summary>
/// A concrete implementation of a breakable obstacle (a crate).
/// Manages procedural loot generation and records objective additions to the central manager.
/// </summary>
public class Crate : MonoBehaviour, IBreakable
{
    [Header("Loot Configuration")]
    [Tooltip("An array of prefabs (powerups, items, coins) that this crate could potentially drop upon breaking.")]
    [SerializeField] private GameObject[] possibleContents;

    /// <summary> The specific item chosen to drop from this crate instance. Remains null if the crate contains nothing. </summary>
    private GameObject contents;

    /// <summary>
    /// Evaluates random item generation conditions. Automatically hooks up metric registrations 
    /// if an objective pickup (Taco) is rolled inside slot index 0.
    /// </summary>
    public void OnSpawn()
    {
        // 60% probability check to determine if this crate holds items or is empty (1.0f - 0.4f)
        bool hasContents = Random.value > 0.4f;

        if (hasContents && possibleContents != null && possibleContents.Length > 0)
        {
            // Pick a completely random index within the bounds of the drop table array
            int index = Random.Range(0, possibleContents.Length);
            contents = possibleContents[index];

            // CRITICAL DESIGN ASSUMPTION: Index 0 must ALWAYS represent your primary Taco prefab!
            if (contents == possibleContents[0])
            {
                GameManager.instance.OnTacoSpawn();
            }
        }
    }

    /// <summary>
    /// Interface method implementation from IBreakable. Spawns items safely centered 
    /// on the grid path and automatically places them under a dedicated organization transform container.
    /// </summary>
    public void OnBreak()
    {
        // Locate the target parent layout node to keep the hierarchy scene clean at runtime
        Transform container = GameObject.Find("Collectables").transform;

        // If an item was successfully selected during initialization, manifest it in the game world
        if (contents != null)
        {
            // Center the pickup footprint by adding a +0.5f offset, keeping layout positions structurally uniform
            Instantiate(contents, transform.position + new Vector3(0.5f, 0f, 0.5f), transform.rotation, container);
        }

        // Disable the crate object, taking it out of physical play and hiding it from view
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        // Re-evaluate contents whenever enabled (supports layout generation pools)
        OnSpawn();
    }
}
