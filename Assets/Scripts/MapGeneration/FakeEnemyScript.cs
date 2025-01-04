using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeEnemyScript : MonoBehaviour
{
    private void OnMouseUp()
    {
        Level.EnemyCount--;

        if(Level.EnemyCount == 0 )
        {
           Transform Doors = GameObject.Find(Player.CurrentRoom.RoomNumber.ToString()).transform.Find("Doors");
            
            Player.CurrentRoom.Cleared = true;

            Animator A;
            Doors.Find("LeftDoor").TryGetComponent<Animator>(out A);
            A.enabled = true;
            Doors.Find("RightDoor").TryGetComponent<Animator>(out A);
            A.enabled = true;
            Doors.Find("TopDoor").TryGetComponent<Animator>(out A);
            A.enabled = true;
            Doors.Find("BottomDoor").TryGetComponent<Animator>(out A);
            A.enabled = true;

            transform.parent.gameObject.SetActive(false);
        }
    }
}
