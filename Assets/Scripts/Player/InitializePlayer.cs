using UnityEngine;

public class InitializePlayer : MonoBehaviour
{
    public Camera cam;
    public Animator PlayerAnimator;
    public GameObject PlayerAttack;
    public GameObject PlayerAttackExplosion;

    void Start()
    {
        Player.Camera = cam;
        Player.animator = PlayerAnimator;
        Player.Attack = PlayerAttack;
        Player.AttackExplosion = PlayerAttackExplosion;

    }
}
