using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AnimationPicker : MonoBehaviour
{
    [SerializeField] private Animator animator;

    // List of animation state names that the picker can trigger
    [SerializeField] private List<string> animationStateNames = new List<string>();

    // List of UI buttons (must match order of animationStateNames)
    [SerializeField] private List<Button> buttons = new List<Button>();

    // Names of the movement states that block picker actions
    [SerializeField] private string walkStateName = "Walk";
    [SerializeField] private string runStateName = "Run";

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        // Ensure lists are the same length
        int count = Mathf.Min(animationStateNames.Count, buttons.Count);

        for (int i = 0; i < count; i++)
        {
            int index = i; // Capture for lambda
            buttons[i].onClick.AddListener(() => TryPlayAnimation(index));
        }
    }

    private void TryPlayAnimation(int index)
    {
        if (animator == null || index < 0 || index >= animationStateNames.Count)
        {
            Debug.LogWarning("Invalid animation index or Animator missing.");
            return;
        }

        // Check if we are currently in Walk or Run
        if (IsInState(walkStateName) || IsInState(runStateName))
        {
            // Optionally give feedback
            // Debug.Log("Cannot play picker animation while walking or running.");
            return; // Block the action
        }

        // Safe to play the selected animation
        string stateName = animationStateNames[index];
        animator.Play(stateName);
    }

    // Helper to check if the Animator is currently playing a specific state in the base layer
    private bool IsInState(string stateName)
    {
        if (animator == null) return false;

        // For states in sub-layers, use "LayerName.StateName" format
        // For base layer states, just the state name is usually enough
        return animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }

    private void Reset()
    {
        if (animator == null) animator = GetComponent<Animator>();
    }

#if UNITY_EDITOR
    // Optional: Visual feedback in the Inspector
    private void OnValidate()
    {
        if (buttons.Count != animationStateNames.Count)
        {
            Debug.LogWarning("AnimationPicker: Buttons list size does not match Animation State Names list size.");
        }
    }
#endif
}