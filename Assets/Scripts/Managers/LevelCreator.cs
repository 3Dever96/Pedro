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

    [Header("Grid Configurations")]
    [Tooltip("The overall width (X) and length (Y) dimensions of the generated level maze.")]
    [SerializeField] private Vector2Int gridSize = new Vector2Int();

    [Tooltip("Coordinates explicitly protected from generating breakable block blockades (e.g., player spawn zones).")]
    [SerializeField] private List<Vector2Int> startingCells = new List<Vector2Int>();

    /// <summary>
    /// Master runtime database mapping every localized 2D grid coordinate to its underlying environment cell type.
    /// Note: Dictionary tracking fields marked with [SerializeField] will not natively show in the Unity Inspector without a custom drawer.
    /// </summary>
    [SerializeField] public Dictionary<Vector2Int, CellType> cellData = new Dictionary<Vector2Int, CellType>();

    private void Awake()
    {
        // -------------------------------------------------------------
        // PHASE 1: GENERATING DATA ARCHITECTURE
        // -------------------------------------------------------------

        // Loop row-by-row and column-by-column through the designated dimensional boundaries
        for (var x = 0; x < gridSize.x; x++)
        {
            for (var y = 0; y < gridSize.y; y++)
            {
                // Default fallback: Any unhandled space defaults to an impenetrable permanent solid boundary wall
                CellType newType = CellType.Solid;

                // Outer Boundary Guard: Check if the current coordinate sits on the very outer edges of the grid matrix
                if (x != 0 && y != 0 && x != gridSize.x - 1 && y != gridSize.y - 1)
                {
                    // Structural Filtering: Ensures paths or pillars follow a predictable patterned structure (resembling traditional Bomberman/Maze algorithms)
                    if (x % 2 != 0 || y % 2 != 0)
                    {
                        Vector2Int position = new Vector2Int(x, y);

                        // Safety Check: Verify this coordinate wasn't specifically blacklisted to preserve player movement space
                        if (!startingCells.Contains(position))
                        {
                            // Procedural Probability: 50% chance to drop an interactive obstacle or an empty walkway path
                            bool isBreakable = Random.value > 0.5f;

                            if (isBreakable)
                            {
                                newType = CellType.Breakable;
                            }
                            else
                            {
                                newType = CellType.Empty;
                            }
                        }
                        else
                        {
                            // Coordinate was protected inside startingCells list; forcefully override to a safe empty path
                            newType = CellType.Empty;
                        }
                    }
                }

                // Append the calculated grid identity metadata into our centralized tracking map
                cellData.Add(new Vector2Int(x, y), newType);
            }
        }

        // -------------------------------------------------------------
        // PHASE 2: INSTANTIATING PHYSICAL GEOMETRY
        // -------------------------------------------------------------

        // Iterate across the populated environment blueprint data to spawn objects into the game world
        foreach (KeyValuePair<Vector2Int, CellType> kvp in cellData)
        {
            if (kvp.Value == CellType.Breakable)
            {
                // Instantiates the breakable prefab. 
                // Adds a +0.5f positional offset to precisely center the object's origin over its 1x1 grid cell footprint.
                Instantiate(breakable, new Vector3(kvp.Key.x + 0.5f, 0f, kvp.Key.y + 0.5f), Quaternion.identity);
            }
        }
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
