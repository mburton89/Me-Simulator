using ReadyPlayerMe.Core;
using ReadyPlayerMe.Samples.QuickStart;
using UnityEngine;

public class QuickStartAnimationAssigner : MonoBehaviour
{
    [SerializeField] private ThirdPersonLoader thirdPersonLoader; // Drag the RPM Player's ThirdPersonLoader here
    [SerializeField] private RuntimeAnimatorController avatarAnimatorController; // Your Idle/Walk controller

    private void OnEnable()
    {
        thirdPersonLoader.OnLoadComplete += OnAvatarLoaded; // Subscribe to the built-in event
    }

    private void OnDisable()
    {
        thirdPersonLoader.OnLoadComplete -= OnAvatarLoaded;
    }

    private void OnAvatarLoaded()
    {
        // The loaded avatar is already set up in ThirdPersonLoader.SetupAvatar()
        // Just grab the current avatar's Animator and assign your controller
        var animator = thirdPersonLoader.GetComponentInChildren<Animator>(); // Or use the stored 'avatar' field if exposed
        if (animator != null && avatarAnimatorController != null)
        {
            animator.runtimeAnimatorController = avatarAnimatorController;
            Debug.Log("Custom animations assigned to loaded avatar!");
        }
    }

    // Your keypress controls (Alpha1/2)
    private void Update()
    {
        var animator = thirdPersonLoader.GetComponentInChildren<Animator>();
        if (animator == null) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            animator.Play("Idle", 0, 0f);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            animator.Play("Walking", 0, 0f);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            animator.Play("Running", 0, 0f);
    }
}