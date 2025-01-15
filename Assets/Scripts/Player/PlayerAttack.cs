using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerAttack : MonoBehaviour
{
    public VisualEffect AttackEffect;
    public Transform AttackSpawnSpot;
    public void FinishAttacking()
    {
        if (Input.GetButton("Fire1"))
            Player.animator.Play("Attack");
        else
        {
            AttackEffect.Stop();
            AttackEffect.Reinit();
            Player.animator.Play("Idle");
            Player.State = "Idle";
        }
    }

    public void Attack()
    {
        Player.Audio.PlayOneShot(Resources.Load<AudioClip>("Sounds/PlayerAttackSounds/"+ Random.Range(1,7).ToString()),.5f);


        GameObject g = Instantiate(Player.Attack, new Vector3(AttackSpawnSpot.position.x,10, AttackSpawnSpot.position.z), Player.Attack.transform.rotation);
        g.GetComponent<Rigidbody>().velocity =  transform.forward * 70;

        AttackEffect.Play();
        GameObject.Destroy(g,5);
    }

    void Update()
    {
        if(Input.GetButton("Fire1") && Player.State == "Idle")
        {
            Player.State = "Attacking";
            string AttackNumber = Random.Range(1,5).ToString();
            
            //Player.animator.Play("Attack" + AttackNumber);
            Player.animator.Play("Attack");
        }
    }
}
