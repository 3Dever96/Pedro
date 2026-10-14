using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// A centralized input manager that handles the Unity Input System's messages,
/// processing player actions and making them globally accessible.
/// </summary>
[RequireComponent(typeof(PlayerInput))]
public class InputHub : MonoBehaviour
{
    // ==========================================
    // PUBLIC PROPERTIES (Read-Only Externally)
    // ==========================================

    /// <summary> Current 2D movement vector from the controller stick or WASD keys. </summary>
    public Vector2 Move { get; private set; }

    /// <summary> True if the player is holding down the charge action button. </summary>
    public bool Charge { get; private set; }

    /// <summary> True if the player is holding down the throw action button. </summary>
    public bool Throw { get; private set; }

    /// <summary> True if the player is holding down the pause action button. </summary>
    public bool Pause { get; private set; }

    // ==========================================
    // SINGLETON & PRIVATE COMPONENTS
    // ==========================================

    /// <summary> Global static access point for other scripts to read current player inputs. </summary>
    public static InputHub Instance { get; private set; }

    private PlayerInput input;

    private void Awake()
    {
        // --- LOGIC BUG ALERT / REGULAR SINGLETON SETUP ---
        // TODO: Review the Singleton initialization logic here. 
        // Currently, it checks if Instance is NOT null and overwrites it. 
        // If you want a traditional Singleton, you typically want to assign 'Instance = this;' 
        // only when 'Instance == null'.
        if (Instance == null)
        {
            Instance = this;
        }

        // If this instance is not the active global instance, destroy it to avoid duplicates
        if (Instance != this)
        {
            Destroy(gameObject);
        }

        // Persist the input system across scene loads
        DontDestroyOnLoad(gameObject);

        // Cache the reference to the attached PlayerInput component
        input = GetComponent<PlayerInput>();
    }

    /// <summary>
    /// Event listener called automatically by the PlayerInput component whenever an input occurs.
    /// </summary>
    /// <param name="context">The state context of the triggered action.</param>
    public void OnAction(InputAction.CallbackContext context)
    {
        // Filter actions by their mapped names in the Input Action Asset
        switch (context.action.name)
        {
            case "Move":
                Move = context.ReadValue<Vector2>();
                break;

            // For buttons, check if the actuation value passes a deadzone threshold (0.5f)
            case "Charge":
                Charge = context.ReadValue<float>() > 0.5f;
                break;

            case "Throw":
                Throw = context.ReadValue<float>() > 0.5f;
                break;

            case "Pause":
                Pause = context.ReadValue<float>() > 0.5f;
                break;
        }
    }

    // ==========================================
    // EVENT SUBSCRIPTIONS
    // ==========================================

    private void OnEnable()
    {
        // Subscribe to the Input System's global action broadcast
        input.onActionTriggered += OnAction;
    }

    private void OnDisable()
    {
        // Clean up subscription to prevent memory leaks when disabled or destroyed
        input.onActionTriggered -= OnAction;
    }
}
