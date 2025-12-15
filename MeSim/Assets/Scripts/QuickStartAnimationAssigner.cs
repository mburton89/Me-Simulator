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
        // PreviewAvatar gets destroyed automatically, replaced by loaded one
        Transform avatarRoot = thirdPersonLoader.transform;
        if (avatarRoot.childCount == 0) return;

        GameObject currentAvatar = avatarRoot.GetChild(0).gameObject;

        Animator animator = currentAvatar.GetComponent<Animator>();
        if (animator == null)
        {
            animator = currentAvatar.AddComponent<Animator>();
        }

        animator.runtimeAnimatorController = arAvatarController;
        animator.applyRootMotion = false;
        animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        animator.Update(0f);
        animator.SetFloat("Speed", 0f); // Starts Idle

        Debug.Log("Animations ready on loaded avatar!");
    }
}