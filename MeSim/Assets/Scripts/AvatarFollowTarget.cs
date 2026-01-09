using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class AvatarFollowTarget : MonoBehaviour
{
    [Header("RPM Player Root")]
    [SerializeField] private Transform rpmPlayerRoot;

    [Header("Target")]
    [SerializeField] private Transform cubeTarget;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float stopDistance = 0.8f;

    [Header("Animation Picker")]
    [Tooltip("List of custom animation state names (e.g., Wave, Dance, Sit)")]
    [SerializeField] private List<string> customAnimationStates = new List<string>();

    public List<Animation> customAnimations;

    [Tooltip("UI Buttons that trigger the custom animations (must match order above)")]
    [SerializeField] private List<Button> animationButtons = new List<Button>();

    [Header("Locomotion State Names")]
    [SerializeField] private string idleStateName = "Idle";
    [SerializeField] private string walkStateName = "Walk";
    [SerializeField] private string runStateName = "Run";

    private Animator avatarAnimator;
    private bool isPlayingCustomAnimation = false;

    private void Awake()
    {
        // Hook up buttons
        for (int i = 0; i < animationButtons.Count && i < customAnimationStates.Count; i++)
        {
            int index = i;
            animationButtons[i].onClick.RemoveAllListeners();
            animationButtons[i].onClick.AddListener(() => PlayCustomAnimation(index));
        }
    }

    private void Update()
    {
        if (rpmPlayerRoot == null || cubeTarget == null || rpmPlayerRoot.childCount == 0) return;

        if (avatarAnimator == null)
            avatarAnimator = rpmPlayerRoot.GetComponentInChildren<Animator>();

        if (avatarAnimator == null) return;

        Vector3 direction = cubeTarget.position - rpmPlayerRoot.position;
        float distance = direction.magnitude;

        AnimatorStateInfo stateInfo = avatarAnimator.GetCurrentAnimatorStateInfo(0);

        // If we're in a custom emote, block movement until we return to Idle
        if (isPlayingCustomAnimation)
        {
            // Still in emote → do nothing, block all locomotion
            return;
        }

        // Normal locomotion only runs when NOT in a custom emote
        if (distance <= stopDistance)
        {
            avatarAnimator.Play(idleStateName);
            return;
        }

        // Move toward the target
        Vector3 moveDirection = direction.normalized;
        rpmPlayerRoot.position += moveDirection * moveSpeed * Time.deltaTime;

        // Rotate to face direction
        Vector3 lookDirection = new Vector3(moveDirection.x, 0, moveDirection.z);
        if (lookDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            rpmPlayerRoot.rotation = Quaternion.Slerp(rpmPlayerRoot.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Play walk or run animation
        if (ToggleScaleButton.instance.isMaximized)
        {
            avatarAnimator.Play(walkStateName);
        }
        else
        {
            avatarAnimator.Play(runStateName);
        }
    }

    private void PlayCustomAnimation(int index)
    {
        if (avatarAnimator == null || isPlayingCustomAnimation) return;
        if (index < 0 || index >= customAnimationStates.Count) return;

        string targetState = customAnimationStates[index];

        // Optional: Block emotes while moving (remove if you want emotes during walk/run)
        AnimatorStateInfo currentState = avatarAnimator.GetCurrentAnimatorStateInfo(0);
        if (currentState.IsName(walkStateName) || currentState.IsName(runStateName))
        {
            return;
        }

        // Play the emote and mark that we're in override mode
        avatarAnimator.Play(targetState);

        StartCoroutine(WaitForCustomAnimation());
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (animationButtons.Count != customAnimationStates.Count)
        {
            Debug.LogWarning("AvatarFollowTarget: Number of buttons doesn't match number of custom animation states.");
        }
    }
#endif

    private IEnumerator WaitForCustomAnimation() 
    {
        isPlayingCustomAnimation = true;
        yield return new WaitForSeconds(2);
        isPlayingCustomAnimation = false;
    }
}