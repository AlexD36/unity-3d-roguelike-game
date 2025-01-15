using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieScriptTutorial : Unit
{
    

    override public void FollowPathAction(Path path, int pathIndex)
    {
        if (HitPlayer) return;

        Quaternion targetRotation = Quaternion.LookRotation(path.lookPoints[pathIndex] - transform.position);
        Quaternion newTR = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, newTR, Time.deltaTime * TurnSpeed);

        Vector3 translation = transform.forward * speed * Time.deltaTime;
        rb.MovePosition(rb.position + translation);
        rb.velocity = Vector3.zero;

    }


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.name.Contains("StickWizard"))
        {
            HitPlayer = true;

            ImpactReceiver.AddImpactOnGameObject(Player.transform.gameObject, (Player.transform.position - transform.position) * Player.KnockBack);
            HeartScript.TakeDamage(1);


            Invoke(nameof(GivePlayerASecond), .2f);
        }
        if(collision.collider.name.Contains("Robot"))
        {
            collision.collider.transform.GetComponent<Rigidbody>().AddForce((collision.collider.transform.position - transform.position) * Player.EnemyKnockBack, ForceMode.Impulse);
            Unit U;
            collision.collider.TryGetComponent<Unit>(out U);
            U.HitPlayer = true;
            StartCoroutine(ResetHitPlayer(U));
        }
    }

    private IEnumerator ResetHitPlayer(Unit U)
    {
        yield return new WaitForSeconds(0.2f);
        U.HitPlayer = false;
    }
}
