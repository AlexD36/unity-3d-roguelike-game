using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public CharacterController Controller;
    public Animator A;
    public Transform T;
    public Camera cam;

    public float Speed = .01f;
    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if(h != 0 || v != 0)
        {
            A.Play("Walk");     
        }
        else
        {
            A.Play("Idle");
        }

        Vector3 Move = new Vector3(h, 0, v) * Speed;
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
        Vector3 targetposition = transform.position + new Vector3(0,60,0);
        Vector3 newPosition = Vector3.MoveTowards(cam.transform.position, targetposition, 130f * Time.deltaTime);
        cam.transform.position = newPosition;
    }
}
