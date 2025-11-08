using UnityEngine;

public class SettingsAnimator : MonoBehaviour
{
    public Animator animator;

    public void OpenSettings()
    {
        animator.ResetTrigger("Close");
        animator.SetTrigger("Open");
    }

    public void CloseSettings()
    {
        animator.ResetTrigger("Open");
        animator.SetTrigger("Close");
    }
}
