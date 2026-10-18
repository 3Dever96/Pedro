using UnityEngine;

/// <summary>
/// Listens directly to GameManager system updates to update the text elements 
/// on the screen HUD without introducing heavy tracking overhead loops.
/// </summary>
public class TacoCounter : MonoBehaviour
{
    [Header("UI Text Displays")]
    [SerializeField] private TMPro.TMP_Text maxTacoCounter;
    [SerializeField] private TMPro.TMP_Text currentTacoCounter;

    private int maxTaco = 0;
    private int currentTaco = 0;

    /// <summary> Triggered via AddMaxTaco event listener. Updates total count string displays. </summary>
    private void OnTacoSpawn()
    {
        maxTaco++;
        maxTacoCounter.text = "/" + maxTaco.ToString();
    }

    /// <summary> Triggered via AddCurrentTaco event listener. Updates scoring progress text strings. </summary>
    private void OnTacoCollected()
    {
        currentTaco++;
        currentTacoCounter.text = currentTaco.ToString();
    }

    private void OnEnable()
    {
        // Subscribe callback tracking to decoupled engine channels
        GameManager.instance.AddMaxTaco.AddListener(OnTacoSpawn);
        GameManager.instance.AddCurrentTaco.AddListener(OnTacoCollected);
    }

    private void OnDisable()
    {
        // Always teardown layout listener hooks to prevent persistent memory leak errors
        GameManager.instance.AddMaxTaco.RemoveListener(OnTacoSpawn);
        GameManager.instance.AddCurrentTaco.RemoveListener(OnTacoCollected);
    }
}
