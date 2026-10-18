using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Centralized gameplay coordinator handling game state loops, progression stats,
/// level completion requirements, and event notification streams.
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary> Global static access point for persistent engine-level state tracking. </summary>
    public static GameManager instance;

    [Header("Progression Signals")]
    public UnityEvent AddMaxTaco;
    public UnityEvent AddCurrentTaco;
    public UnityEvent GoalAvailable;
    public UnityEvent GoalReached;

    private int maxTaco;
    private int currentTaco;

    /// <summary> The structural runtime operational state of the gameplay loops. </summary>
    public GameState state;

    private void Awake()
    {
        // Enforce traditional strict Singleton lifecycle constraints
        if (instance == null)
        {
            instance = this;
        }

        if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Registers a newly manifested target pickup in the world blueprint database, broadcasting updates to UI nodes.
    /// </summary>
    public void OnTacoSpawn()
    {
        maxTaco++;
        AddMaxTaco?.Invoke();
    }

    /// <summary>
    /// Tallies a collected objective pickup item. Instantly fires off state checks 
    /// to reveal the exit vector once all targets are acquired.
    /// </summary>
    public void OnTacoCollected()
    {
        currentTaco++;
        AddCurrentTaco?.Invoke();

        // Check if the current total matches the maximum spawned objective requirement
        if (currentTaco == maxTaco)
        {
            GoalAvailable?.Invoke();
        }
    }

    /// <summary>
    /// Shuts down active gameplay configurations and processes level victory triggers.
    /// </summary>
    public void OnGoalReached()
    {
        state = GameState.Goal;
        GoalReached?.Invoke();
    }
}

/// <summary>
/// Dictates the overarching system state properties of the active gameplay framework execution.
/// </summary>
public enum GameState
{
    Play,
    Paused,
    Goal,
    GameOver
}
