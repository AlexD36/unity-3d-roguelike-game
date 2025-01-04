using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public static class Player
{

    public static string State = "Idle";
    public static bool Invincible = false;
    public static Transform transform;
    public static GameObject PlayerStaff;
    public static Animator animator;

    public static float Speed = .2f;
    public static float KnockBack = 100;

    public static float Health = 3f;
    public static float MaxHealth = 3f;

    public static GameObject Attack;
    public static GameObject AttackExplosion;

    public static LinkedList<GameObject> Bombs = new LinkedList<GameObject>();
    public static GameObject Bomb;
    public static GameObject BombExplosion;


    public static GameObject HeartPanel;
    public static GameObject DamagePanel;
    public static Sprite FullHeart;
    public static Sprite HalfHeart;
    public static Sprite EmptyHeart;
    public static RuntimeAnimatorController HeartAnimator;
    

    public static Camera Camera;

    public static Room CurrentRoom;

    public static CharacterController Controller;

}
