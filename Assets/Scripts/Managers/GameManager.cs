using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public UnityEvent AddMaxTaco;
    public UnityEvent AddCurrentTaco;

    private int maxTaco;
    private int currentTaco;

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
    }
}
