using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Procedurally generates a grid-based maze layer at runtime, defining structural boundaries,
/// generating random breakable obstacles, and spawning the physical game pieces.
/// </summary>
public class LevelCreator : MonoBehaviour
{
    [Header("Generation Prefabs")]
    [Tooltip("The prefab instantiated for tiles that can be shattered by a player slide.")]
    [SerializeField] private GameObject breakable;
    [Tooltip("Reference to the scene Exit Goal doorway asset model.")]
    [SerializeField] private GameObject goal;

    [Header("Grid Configurations")]
    [Tooltip("The overall width (X) and length (Y) dimensions of the generated level maze.")]
    [SerializeField] private Vector2Int gridSize = new Vector2Int();

    [Tooltip("Coordinates explicitly protected from generating breakable block blockades (e.g., player spawn zones).")]
    [SerializeField] private List<Vector2Int> startingCells = new List<Vector2Int>();

    /// <summary> Master runtime database mapping coordinates to identities. </summary>
    [SerializeField] public Dictionary<Vector2Int, CellType> cellData = new Dictionary<Vector2Int, CellType>();

    private void Awake()
    {
        // -------------------------------------------------------------
        // PHASE 1: GENERATING DATA ARCHITECTURE
        // -------------------------------------------------------------
        for (var x = 0; x < gridSize.x; x++)
        {
            for (var y = 0; y < gridSize.y; y++)
            {
                CellType newType = CellType.Solid;

                if (x != 0 && y != 0 && x != gridSize.x - 1 && y != gridSize.y - 1)
                {
                    if (x % 2 != 0 || y % 2 != 0)
                    {
                        Vector2Int position = new Vector2Int(x, y);

                        if (!startingCells.Contains(position))
                        {
                            bool isBreakable = Random.value > 0.5f;
                            newType = isBreakable ? CellType.Breakable : CellType.Empty;
                        }
                        else
                        {
                            newType = CellType.Empty;
                        }
                    }
                }
                cellData.Add(new Vector2Int(x, y), newType);
            }
        }

        // -------------------------------------------------------------
        // PHASE 2: INSTANTIATING PHYSICAL GEOMETRY
        // -------------------------------------------------------------
        foreach (KeyValuePair<Vector2Int, CellType> kvp in cellData)
        {
            if (kvp.Value == CellType.Breakable)
            {
                // Instantiate obstacles while nesting them inside the level generator's transform context for order
                Instantiate(breakable, new Vector3(kvp.Key.x + 0.5f, 0f, kvp.Key.y + 0.5f), Quaternion.identity, transform);
            }
        }
    }

    /// <summary> Activated via manager triggers to expose the escape route once all targets are cleared. </summary>
    public void SpawnGoal()
    {
        goal.SetActive(true);
    }

    private void OnEnable()
    {
        GameManager.instance.GoalAvailable.AddListener(SpawnGoal);
    }

    private void OnDisable()
    {
        GameManager.instance.GoalAvailable.RemoveListener(SpawnGoal);
    }
}


/// <summary>
/// Defines the unique structural or environmental identity classifications assigned to individual coordinates on the world map grid.
/// </summary>
public enum CellType
{
    /// <summary> A clear navigable pathway that actors can freely move through. </summary>
    Empty,

    /// <summary> An permanent, indestructible boundary wall or structural asset pillar. </summary>
    Solid,

    /// <summary> A destructible obstacle block that collapses upon receiving physical slide damage. </summary>
    Breakable,

    /// <summary> A grid footprint actively occupied or claimed by a hostile enemy entity. </summary>
    Enemy
}
