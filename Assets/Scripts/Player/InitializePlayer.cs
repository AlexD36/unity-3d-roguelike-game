using UnityEngine;

public class InitializePlayer : MonoBehaviour
{
    public Camera cam;
    public Animator PlayerAnimator;

    void Start()
    {
        Player.Camera = cam;
        Player.animator = PlayerAnimator;

    }
}
