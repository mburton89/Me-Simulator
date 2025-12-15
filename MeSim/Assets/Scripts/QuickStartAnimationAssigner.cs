using ReadyPlayerMe.Samples.QuickStart;
using UnityEngine;

public class QuickStartAnimationAssigner : MonoBehaviour
{
    [SerializeField] private ThirdPersonLoader thirdPersonLoader;
    [SerializeField] private RuntimeAnimatorController arAvatarController;

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
        if (thirdPersonLoader.transform.childCount == 0) return;

        GameObject loadedAvatar = thirdPersonLoader.transform.GetChild(0).gameObject;
        Animator animator = loadedAvatar.GetComponentInChildren<Animator>();

        if (animator != null && arAvatarController != null)
        {
            animator.runtimeAnimatorController = arAvatarController;
            animator.applyRootMotion = false;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            animator.Update(0f);
            animator.SetFloat("Speed", 0f);
        }
    }
}