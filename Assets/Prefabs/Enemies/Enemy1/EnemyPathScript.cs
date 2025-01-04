using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using static UnityEngine.GraphicsBuffer;

public class EnemyPathScript : MonoBehaviour
{
    public float Distance = 5;
    public float RebuildTime = 1;
    public float Radius = 5f;
    public float Speed = 15;
    public int degreesvariance = 15;
    List<GameObject> Path = new List<GameObject>();
    public GameObject Point;
    public GameObject MarkerPoint;

    int pointlimit = 0;
    void DrawPoint(GameObject CurrentPoint)
    {
        ++pointlimit;
        if (pointlimit > 50) return;


        Vector3 direction = Player.transform.position - CurrentPoint.transform.position;

        if (Vector3.Distance(Player.transform.position, CurrentPoint.transform.position) <= 5) return;

        bool CheckingDirection = true;
        int failsafe = -1;
        int counter = 0;
         
        while (CheckingDirection)
        {
            failsafe++;
            if (failsafe > 72) return; //360/5 is 72, we only need to check in all directions, not more.


            int result = counter % 2 == 0 ?
                counter / 2 * degreesvariance : -counter / 2 * degreesvariance;
            counter++;

            Quaternion rotation = Quaternion.LookRotation(direction);
            rotation *= Quaternion.Euler(0f, result, 0f);
            Vector3 adjustedDirection = rotation * Vector3.forward;


            LayerMask layerMask = ~(1 << LayerMask.NameToLayer("Floor") | 1 << LayerMask.NameToLayer("Enemy"));

            RaycastHit RH;
            if (Physics.Raycast(CurrentPoint.transform.position, adjustedDirection, out RH, Distance, layerMask))
            {
                Debug.Log(RH.transform.name);
                if (RH.transform.name == "StickWizardTutorial")
                {
                    return;
                }
            }
            else
            {
                Collider[] hitobjs = Physics.OverlapSphere(CurrentPoint.transform.position + adjustedDirection.normalized * Distance, Radius, layerMask);


                if (hitobjs.Count() == 0)
                {
                    CheckingDirection = false;


                    Vector3 pos = CurrentPoint.transform.position + adjustedDirection.normalized * Distance;

                    Path.Add(Instantiate(Point, new Vector3(pos.x,2,pos.z), Point.transform.rotation));
                }
                else
                {
                   foreach(Collider coll in hitobjs)
                    {
                        if(coll.transform.name == "StickWizardTutorial")
                        {
                            return;
                        }
                    }
                }
               
            }
        }


        DrawPoint(Path.Last());
    }

    void DrawPath()
    {
        if (Path.Count > 0)
        {
            for (int i = 0; i < Path.Count; i++)
            {
                GameObject.Destroy(Path[i]);
            }
            Path.Clear();
        }
        pointlimit = 0;
        DrawPoint(gameObject);

    }

  
    bool PathDrawn = false;
    bool TouchingWall = false;
    float TouchingWallTimer = 0;
    Vector3 CollisionPoint = Vector3.zero;

    Vector3 MoveTowards = Vector3.zero;

    void Update()
    {


        if (!PathDrawn && Input.GetKey(KeyCode.Y))
        {
            PathDrawn = true;
            InvokeRepeating(nameof(DrawPath), 0, RebuildTime);
        }


        if (!PathDrawn) return;

        if (Path.Count > 0)
        {
            MoveTowards = Path[0].transform.position - transform.position;
            rb.velocity = MoveTowards.normalized * Speed;

            MoveTowards = new Vector3(MoveTowards.x,transform.position.y, MoveTowards.z);
            Quaternion targetRotation = Quaternion.LookRotation(MoveTowards.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation * Quaternion.Euler(-90,180,0), Time.deltaTime * 10);

            float distanceToTarget = Vector3.Distance(transform.position, Path[0].transform.position);

            if (distanceToTarget < 6f)
            {
                GameObject.Destroy(Path[0], 0.01f);
                Path.RemoveAt(0);
            }

        }
        else
        {
            MoveTowards = Player.transform.position - transform.position;
            rb.velocity = MoveTowards.normalized * Speed;

            MoveTowards = new Vector3(MoveTowards.x, transform.position.y, MoveTowards.z);
            Quaternion targetRotation = Quaternion.LookRotation(MoveTowards.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation * Quaternion.Euler(-90, 180, 0), Time.deltaTime * 10);

        }

    }





    //GameObject FindClosestObject()
    //{
    //    GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

    //    GameObject closestObject = null;
    //    float closestDistance = float.MaxValue;

    //    foreach (GameObject otherObject in allObjects)
    //    {
    //        if (otherObject != gameObject && 
    //            otherObject.activeInHierarchy &&
    //            otherObject.layer == LayerMask.NameToLayer("Obstacle"))
    //        {
    //            float distance = Vector3.Distance(gameObject.transform.position, otherObject.transform.position);

    //            if (distance < closestDistance)
    //            {
    //                closestDistance = distance;
    //                closestObject = otherObject;
    //            }
    //        }
    //    }

    //    return closestObject;
    //}


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    Rigidbody rb;


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name == "StickWizardTutorial")
        {
            HeartScript.TakeDamage(1);
        }
       
    }

  



    }
