using UnityEngine;

/// <summary>
/// Triggers the level victory transition phase once an authorized player controller 
/// establishes intersection contact with this physical zone.
/// </summary>
public class Goal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Verify if the hitting actor handles player mechanics
        PlayerController player = other.GetComponent<PlayerController>();

        if (player != null)
        {
            // Report to the master manager that the map has been beaten
            GameManager.instance.OnGoalReached();
        }
    }
}
