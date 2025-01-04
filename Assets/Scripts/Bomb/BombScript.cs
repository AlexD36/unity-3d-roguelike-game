using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BombScript : MonoBehaviour
{

    public void EndBomb()
    {
        Player.State = "Idle";
        Player.PlayerStaff.SetActive(true);
    }

    bool AbleToDropBomb = false;

    public void WaitForBombDrop()
    {
        AbleToDropBomb = true;
    }


    public IEnumerator BombCountdown(float seconds, GameObject bomb)
    {
        bool bombdropped = false;
        float minspeed = 1;
        float maxspeed = 20;

        float TTime = 0;

        for (float i = 0; i <= seconds*100; i++) {
            if(bomb == null)
            {
                bombdropped = true;
                yield break;
            }

            
            Material M1 = bomb.GetComponent<MeshRenderer>().materials[0];
            Material M2 = bomb.GetComponent<MeshRenderer>().materials[1];

            TTime += Time.deltaTime;

            float angle = TTime * Mathf.Lerp(minspeed, maxspeed, i / seconds / 100);
            float sinValue = Mathf.Sin(angle);

            M1.SetFloat("_FlashSpeed", sinValue);
            M2.SetFloat("_FlashSpeed", sinValue);

            if (bomb.transform.Find("Timer") == null) yield break;
            bomb.transform.Find("Timer").GetComponent<TextMeshPro>().fontSize = 24 + (sinValue * 5);


            if (i % 100 == 0)
            {
                bomb.transform.Find("Timer").GetComponent<TextMeshPro>().text = (seconds - (i / 100)).ToString();

               // bomb.transform.Find("Timer").GetComponent<TextMeshPro>().faceColor = Color.Lerp(new Color(0, 0, 0), new Color(1, 1, 1), (i / 100)/seconds);
            }
            yield return new WaitForSeconds(.01f);
        }
        GameObject.Destroy(Instantiate(Player.BombExplosion, bomb.transform.position, Quaternion.identity), 3);
        Player.Bombs.Remove(bomb);
        GameObject.Destroy(bomb);
        if(!bombdropped)
        {
            Player.State = "Idle";
            Player.animator.Play("Idle");
            Player.PlayerStaff.SetActive(true);
            AbleToDropBomb = false;
        }
    }

    void Update()
    {
        if(Input.GetButtonUp("Bomb") && Player.State == "Idle")
        {
            Player.State = "HoldingBomb";
            
            Player.PlayerStaff.SetActive(false);
            Player.animator.Play("HoldingBomb");
            Player.Bombs.AddLast(Instantiate(Player.Bomb,transform.position + new Vector3(0,7,0),Player.Bomb.transform.rotation,transform));
            StartCoroutine(BombCountdown(10, Player.Bombs.Last.Value));
            Invoke(nameof(WaitForBombDrop), .1f);
        }
        if (Input.GetButtonUp("Bomb") && Player.State == "HoldingBomb" && AbleToDropBomb)
        {
            Player.State = "Idle";
            Player.animator.Play("Idle");
            AbleToDropBomb = false;


            GameObject GO = Player.Bombs.Last.Value;
            Destroy(GO.GetComponent<BombExplosionScript>());
            GO.transform.parent = null;
            GO.transform.position = new Vector3(GO.transform.position.x, 0, GO.transform.position.z);
            GO.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            GO.GetComponent<Rigidbody>().freezeRotation = true;
            Invoke(nameof(EndBomb), .1f);
        }
        if (Input.GetButtonUp("Fire1") && Player.State == "HoldingBomb")
        {
            Player.State = "ThrowingBomb";
            Player.animator.Play("ThrowBomb");
            StopCoroutine(BombCountdown(10, Player.Bombs.Last.Value));

            AbleToDropBomb = false;
            GameObject GO = Player.Bombs.Last.Value;
            GameObject.Destroy(Player.Bombs.Last.Value.transform.Find("Timer").gameObject);
            GO.transform.parent = null;
            GO.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            GO.GetComponent<Rigidbody>().freezeRotation = true;
            GO.GetComponent<Rigidbody>().velocity = (transform.forward * 75) + new Vector3(0, 30, 0);
            Invoke(nameof(EndBomb), .1f);
        }
    }
}
