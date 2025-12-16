using UnityEngine;

public class AvatarFollowTarget : MonoBehaviour
{
    [Header("RPM Player Root")]
    [SerializeField] private Transform rpmPlayerRoot; // Drag the RPM Player (parent) here

    [Header("Target")]
    [SerializeField] private Transform cubeTarget;

    [Header("Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float stopDistance = 0.8f;

    private GameObject currentAvatar;
    private Animator avatarAnimator;

    private void Update()
    {
        if (rpmPlayerRoot == null || cubeTarget == null) return;

        if (rpmPlayerRoot.childCount == 0) return;

        currentAvatar = rpmPlayerRoot.GetChild(0).gameObject;
        avatarAnimator = GetComponentInChildren<Animator>();

        if (avatarAnimator == null) return;

        // Use the root's position for calculation
        Vector3 direction = cubeTarget.position - rpmPlayerRoot.position;
        float distance = direction.magnitude;

        if (distance <= stopDistance)
        {
            avatarAnimator.Play("Idle");
            return;
        }

        // MOVE THE PARENT (RPM Player root)
        Vector3 moveDirection = direction.normalized;
        rpmPlayerRoot.position += moveDirection * moveSpeed * Time.deltaTime;

        // ROTATE THE PARENT to face the target
        Vector3 lookDirection = new Vector3(moveDirection.x, 0, moveDirection.z);
        if (lookDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            rpmPlayerRoot.rotation = Quaternion.Slerp(rpmPlayerRoot.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        avatarAnimator.Play("Walk");
    }
}