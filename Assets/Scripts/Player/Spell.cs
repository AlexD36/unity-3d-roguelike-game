using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    

    private void OnCollisionEnter(Collision collision)
    {
        GameObject.Destroy(Instantiate(Player.AttackExplosion,transform.position, Quaternion.identity),3);


        GameObject.Destroy(gameObject);
    }
}
