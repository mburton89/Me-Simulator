using ReadyPlayerMe.Samples.QuickStart;
using UnityEngine;

public class QuickStartAnimationAssigner : MonoBehaviour
{
    [SerializeField] private ThirdPersonLoader thirdPersonLoader;
    [SerializeField] private RuntimeAnimatorController arAvatarController; // Drag your new Blend Tree controller here

    private void OnEnable()
    {
        thirdPersonLoader.OnLoadComplete += OnAvatarLoaded;
    }

    private void OnDisable()
    {
        thirdPersonLoader.OnLoadComplete -= OnAvatarLoaded;
    }

    private void OnAvatarLoaded()
    {
        var animator = thirdPersonLoader.transform.GetComponentInChildren<Animator>();
        if (animator != null && arAvatarController != null)
        {
            animator.runtimeAnimatorController = arAvatarController;
            animator.Update(0f); // Force immediate update
            Debug.Log("AR Avatar Animator Controller assigned with Blend Tree!");
        }
    }
}