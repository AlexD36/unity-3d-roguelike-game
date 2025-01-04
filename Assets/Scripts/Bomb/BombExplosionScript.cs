using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BombExplosionScript : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.name != "StickWizardTutorial" && Player.State != "HoldingBomb")
        {
            GameObject.Destroy(Instantiate(Player.BombExplosion,transform.position, Quaternion.identity),3);


            Collider[] BombHitObjects = Physics.OverlapSphere(transform.position, 15);


            foreach (Collider b in BombHitObjects)
            {
                if (b.name.Contains("DrawX"))
                {
                    Level.SecretRoomExploded = true;

                    GameObject.Destroy(Instantiate(Level.SecretRoomExplosion, b.transform.position, Quaternion.identity), 5f);
                    Transform T = GameObject.Find("Rooms").transform.Find(Player.CurrentRoom.RoomNumber.ToString()).Find("Doors").transform;

                    GameObject SecretDoor = Instantiate(Level.SecretRoomDoor, b.transform.position, b.transform.rotation, T);

                    string direction = (b.transform.position.x > 10) ? "RightDoor" :
                                      (b.transform.position.x < -10) ? "LeftDoor" :
                                      (b.transform.position.z > 10) ? "TopDoor" :
                                      (b.transform.position.z < -10) ? "BottomDoor" :
                                      "Center";

                    SecretDoor.name = direction;



                    GameObject.Destroy(b.gameObject);
                }

            }


            Player.Bombs.Remove(gameObject);
            GameObject.Destroy(gameObject);
            
        }
        
    }
}
