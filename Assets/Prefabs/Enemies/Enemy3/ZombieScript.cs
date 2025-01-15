using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ZombieScript : Unit
{
    private float timeCounter = 0.0f;



    public void GivePlayerASec()
    {
        HitPlayer = false;
    }

    override public void FollowPathAction(Path path, int pathIndex)
    {
        if (HitPlayer) return;

        Quaternion targetRotation = Quaternion.LookRotation(path.lookPoints[pathIndex] - transform.position);

        Quaternion newTR = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, newTR, Time.deltaTime * TurnSpeed);

        Vector3 translation = transform.forward * speed * Time.deltaTime;
        rb.MovePosition(rb.position + translation);
        rb.velocity = Vector3.zero;

        timeCounter += Time.deltaTime;

    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name == "StickmanContainer")
        {
            HitPlayer = true;

            ImpactReceiver.AddImpactOnGameObject(Player.transform.gameObject, (Player.transform.position - transform.position) * Player.KnockBack);
            HeartScript.TakeDamage(1);

            Invoke(nameof(GivePlayerASec), .2f);

        }
       
    }
}
