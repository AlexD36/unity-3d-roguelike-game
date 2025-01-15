using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.VFX;

public class SlimeScript : Unit
{
    private float timeCounter = 0.0f;
    public float oscillationSpeed = 1.0f;
   public float minSpeed = 20;
    public float maxSpeed = 500;
    public float minScale = 1.5f;
    public float maxScale = 2.5f;

    bool HitPlayer = false;

    public void GivePlayerASec()
    {
        HitPlayer = false;
    }

    override public void FollowPathAction(Path path, int pathIndex)
    {
        if (HitPlayer) return;




        Quaternion targetRotation = Quaternion.LookRotation(path.lookPoints[pathIndex] - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * TurnSpeed);

        float sineValue = Mathf.Sin(timeCounter * oscillationSpeed); //-1 and 1
        float currentSpeed = Mathf.Lerp(minSpeed, maxSpeed, (sineValue + 1.0f) / 2.0f);
        speed = currentSpeed;

        float scale = Mathf.Lerp(minScale, maxScale, (sineValue + 1.0f) / 2.0f);
        transform.localScale = new Vector3(Mathf.Clamp(scale, minScale, minScale + .5f), minScale, scale);

        // Apply translation using Rigidbody
        Vector3 translation = transform.forward * currentSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + translation);
        rb.velocity = Vector3.zero;

        timeCounter += Time.deltaTime;

    }

    const float minGrowth = .1f;
    const float maxGrowth = .2f;
    float TrailSize = 1;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.name == "StickmanContainer")
        {
            HitPlayer = true;

            ImpactReceiver.AddImpactOnGameObject(Player.transform.gameObject, (Player.transform.position - transform.position) * Player.KnockBack);
            HeartScript.TakeDamage(2);

            Invoke(nameof(GivePlayerASec), .2f);
            
        }
        else if(collision.collider.name.Contains("circleman"))
        {
            if(transform.localScale.z > collision.transform.localScale.z)
            {
                minScale += collision.transform.GetComponent<SlimeScript>().minScale;
                maxScale += collision.transform.localScale.z;
                
                transform.GetChild(0).GetComponent<VisualEffect>().SetFloat("MaxSize", maxScale);
                GameObject.Destroy(collision.gameObject);
            }
            else
            {
                collision.gameObject.GetComponent<SlimeScript>().minScale += minScale;
                collision.gameObject.GetComponent<SlimeScript>().maxScale += transform.localScale.z;
                TrailSize += 1;
                collision.transform.GetChild(0).GetComponent<VisualEffect>().SetFloat("MaxSize", collision.gameObject.GetComponent<SlimeScript>().maxScale);
                GameObject.Destroy(gameObject);
            }
        }
    }
}
