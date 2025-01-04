using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public void FinishAttacking()
    {
        Player.State = "Idle";
    }

    public void Attack()
    {
        GameObject g = Instantiate(Player.Attack, (transform.position + transform.forward * 5) + new Vector3(0,5,0), Player.Attack.transform.rotation);
        g.GetComponent<Rigidbody>().velocity =  transform.forward * 70;
        GameObject.Destroy(g,5);
    }

    void Update()
    {
        if(Input.GetButton("Fire1") && Player.State == "Idle")
        {
            Player.State = "Attacking";
            string AttackNumber = Random.Range(1,5).ToString();
            
            Player.animator.Play("Attack" + AttackNumber);
        }
    }
}
