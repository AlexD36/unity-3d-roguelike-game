using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController Controller;

    private void Start()
    {
        Player.animator.SetFloat("Speed", Player.Speed /20);
    }

    private void Update()
    {



        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if((h != 0 || v != 0) && (Player.State == "Idle" || Player.State == "HoldingBomb"))
        {
            Player.animator.SetFloat("Speed", 1);
        }
        else
        {
            Player.animator.SetFloat("Speed", 0);
        }



        float ymove = 0;
        if (transform.position.y != 0)
        {
             ymove = -transform.position.y;
        }


        Vector3 Move = new Vector3(h, ymove, v) * Player.Speed * Time.deltaTime;
        
        Controller.Move(Move);



        Plane playerplane = new Plane(Vector3.up, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float hitdist;
        if (playerplane.Raycast(ray, out hitdist))
        {
            Vector3 targetpoint = ray.GetPoint(hitdist);
            Quaternion targetrotation = Quaternion.LookRotation(targetpoint - transform.position);
            transform.rotation = (Quaternion.Slerp(transform.rotation, targetrotation, 50f * Time.deltaTime));
        }



    }


    private void LateUpdate()
    {
      
        Vector3 targetposition = transform.position + new Vector3(0, 109, 0);
        Vector3 newPosition = Vector3.MoveTowards(Player.Camera.transform.position, targetposition, 150f * Time.deltaTime);
        Player.Camera.transform.position = newPosition;
    }
}
