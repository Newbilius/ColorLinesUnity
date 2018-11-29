using System;
using UnityEngine;

public class BallBehavior : MonoBehaviour
{
    public int BallCode;

    PathMover pathMover;
    Animator animator;
    bool AnimationInProgress;
    private Action onTeleportAnimationComplete;
    public AudioClip jumpSound;
    public AudioClip moveSound;
    private Action onShowingAnimationComplete;

    void Start()
    {
        pathMover = new PathMover(gameObject, 8f);
        animator = GetComponent<Animator>();
    }

    public void MoveByPath(Vector3[] newPositions, Action onMoveComplete, Action<Vector3Combined> onStepComplete)
    {
        pathMover.MoveByPath(newPositions, onMoveComplete, onStepComplete);
        StartJumping();
        AudioManager.Instance.PlayLoopSound(moveSound);
    }

    public void DisappearComplete()
    {
        Destroy(gameObject);
    }

    public void StartTeleport(Action onComplete)
    {
        AudioManager.Instance.StopSound();
        AnimationInProgress = false;
        onTeleportAnimationComplete = onComplete;
        animator.Play("Teleportation");
    }

    public void StartShowingAnimation(Action onComplete)
    {
        onShowingAnimationComplete = onComplete;
        animator.Play("BallAppear");
    }

    public void AnimationComplete()
    {
        if (onShowingAnimationComplete != null)
        {
            onShowingAnimationComplete();
            onShowingAnimationComplete = null;
        }
        if (onTeleportAnimationComplete != null)
        {
            onTeleportAnimationComplete();
            onTeleportAnimationComplete = null;
        }
    }

    void Update()
    {
        pathMover.Update();
    }

    public void JumpSound()
    {
        if (!pathMover.IsMoving())
            AudioManager.Instance.PlaySound(jumpSound);
    }

    public void StopJumping()
    {
        if (AnimationInProgress)
        {
            AudioManager.Instance.StopSound();
            AnimationInProgress = false;
            animator.Play("Stand");
        }
    }

    public void StartJumping()
    {
        if (!AnimationInProgress)
        {
            AnimationInProgress = true;
            animator.Play("Jump");
        }
    }

    public void Die()
    {
        if (animator != null)
            animator.Play("BallDisappear");
        else
            Destroy(gameObject);
    }
}
