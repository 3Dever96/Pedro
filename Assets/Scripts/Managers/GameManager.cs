using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public UnityEvent AddMaxTaco;
    public UnityEvent AddCurrentTaco;
    public UnityEvent GoalAvailable;
    public UnityEvent GoalReached;

    private int maxTaco;
    private int currentTaco;

    public GameState state;

    private void Awake()
    {
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

    public void OnTacoSpawn()
    {
        maxTaco++;
        AddMaxTaco?.Invoke();
    }

    public void OnTacoCollected()
    {
        currentTaco++;
        AddCurrentTaco?.Invoke();

        if (currentTaco == maxTaco)
        {
            GoalAvailable?.Invoke();
        }
    }

    public void OnGoalReached()
    {
        state = GameState.Goal;
        GoalReached?.Invoke();
    }
}

public enum GameState
{
    Play,
    Paused,
    Goal,
    GameOver
}
