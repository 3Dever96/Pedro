using UnityEngine;

public class TacoCounter : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text maxTacoCounter;
    [SerializeField] private TMPro.TMP_Text currentTacoCounter;

    private int maxTaco = 0;
    private int currentTaco = 0;

    private void OnTacoSpawn()
    {
        maxTaco++;

        maxTacoCounter.text = "/" + maxTaco.ToString();
    }

    private void OnTacoCollected()
    {
        currentTaco++;

        currentTacoCounter.text = currentTaco.ToString();
    }

    private void OnEnable()
    {
        GameManager.instance.AddMaxTaco.AddListener(OnTacoSpawn);
        GameManager.instance.AddCurrentTaco.AddListener(OnTacoCollected);
    }

    private void OnDisable()
    {
        GameManager.instance.AddMaxTaco.RemoveListener(OnTacoSpawn);
        GameManager.instance.AddCurrentTaco.RemoveListener(OnTacoCollected);
    }
}
