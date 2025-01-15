using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitalizePlayer : MonoBehaviour
{
    public Camera cam;
    public AudioSource AS;

    public Animator PlayerAnimator;
    public GameObject PlayerAttack;
    public GameObject PlayerAttackExplosion;
    public Transform PlayerTransform;
    public CharacterController PlayerController;
    public GameObject SecretRoomExplosion;
    public GameObject XMark;
    public GameObject SecretRoomDoor;
    public GameObject Bomb;
    public GameObject BombExplosion;
    public GameObject PlayerStaff;
    public Sprite FullHeart;
    public Sprite HalfHeart;
    public Sprite EmptyHeart;
    public GameObject HealthPanel;
    public GameObject DamagePanel;
    public RuntimeAnimatorController HeartAnimatorController;

    void Start()
    {
        Application.targetFrameRate = 120;


        Player.Audio = AS;
        Player.Camera = cam;
        Player.animator = PlayerAnimator;
        Player.Attack = PlayerAttack;
        Player.AttackExplosion = PlayerAttackExplosion;
        Player.transform= PlayerTransform;
        Player.Controller= PlayerController;
        Player.Bomb = Bomb;
        Player.BombExplosion = BombExplosion;
        Player.PlayerStaff = PlayerStaff;
        Player.FullHeart = FullHeart;
        Player.EmptyHeart = EmptyHeart;
        Player.HalfHeart= HalfHeart;
        Player.HeartPanel = HealthPanel;
        Player.DamagePanel = DamagePanel;
        Player.HeartAnimator = HeartAnimatorController;

        Level.SecretRoomExplosion = SecretRoomExplosion;
        Level.SecretRoomDoor = SecretRoomDoor;
        Level.XMark = XMark;
 
    }


}
