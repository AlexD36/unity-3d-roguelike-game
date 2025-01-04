using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    Grid g;

    public bool DrawPath = false;

    const float minPathUpdateTime = .2f;
    const float pathUpdateMoveThreshold = .5f;


    [Header("0: Chase Player\n" +
        "1: Random Path")]
    public int MonsterMovementType = 0;


    public Transform target;
    public float speed = 5f;
    public float turnDst = 5f;
    public float TurnSpeed = 3;
    Path path;
    [HideInInspector] public Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        g = GameObject.Find("A*").GetComponent<Grid>();

        switch (MonsterMovementType)
        {
            case 0: StartCoroutine(UpdatePath()); break;
            case 1: StartCoroutine(UpdatePathRandom()); break;
        }
    }

    public void OnPathFound(Vector3[] waypoints,bool pathSuccessful)
    {
        if (!pathSuccessful) return;

        path = new Path(waypoints, transform.position, turnDst);
        StopCoroutine("FollowPath");
        StartCoroutine("FollowPath");

    }

    IEnumerator UpdatePathRandom()
    {
        if (Time.timeSinceLevelLoad < .3f)
        {
            yield return new WaitForSeconds(.3f);
        }

        while (true)
        {
            
            yield return new WaitForSeconds(.2f); 
            Node curN = g.NodeFromWorldPoint(transform.position);
            int max = g.grid.GetLength(0)-1;

            int newX = Mathf.Clamp(curN.gridX + Random.Range(-25, 25), 3, max-3);
            int newY = Mathf.Clamp(curN.gridY + Random.Range(-25, 25), 3, max - 3);

            int iterationNumber = 0;
            while (!g.grid[newX, newY].Walkable)
            {
                iterationNumber++;
                if (iterationNumber > 10) break;

                 newX = Mathf.Clamp(curN.gridX + Random.Range(-25, 25), 3, max - 3);
                 newY = Mathf.Clamp(curN.gridY + Random.Range(-25, 25), 3, max - 3);
                curN = g.grid[newX, newY];
            }

            Vector3 RPos = g.grid[newX, newY].WorldPosition;

            PathRequestManager.RequestPath(new PathRequest(transform.position, RPos, OnPathFound));

        }
    }


    IEnumerator UpdatePath()
    {
        if(Time.timeSinceLevelLoad < .3f)
        {
            yield return new WaitForSeconds(.3f);
        }
        PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));

        float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
        Vector3 targetPosOld = target.position;


        float counter = 0;

        while (true)
        {
            yield return new WaitForSeconds(minPathUpdateTime);
            ++counter;
            if ((target.position - targetPosOld).sqrMagnitude > sqrMoveThreshold || counter > 10)
            {
                PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));
                targetPosOld = target.position;
                counter = 0;
            }
        }
    }

    public virtual void FollowPathAction(Path path, int pathIndex)
    {
        Quaternion targetRotation = Quaternion.LookRotation(path.lookPoints[pathIndex] - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * TurnSpeed);
        transform.Translate(Vector3.forward * Time.deltaTime * speed, Space.Self);
    }

    IEnumerator FollowPath()
    {
        bool followingPath = true;
        int pathIndex = 0;

        while (followingPath)
        {
            Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);
            while (path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
            {
                if(pathIndex == path.finishLineIndex)
                {
                    followingPath = false;
                    break;
                }
                else
                {
                    pathIndex++;
                }
            }

            if(followingPath)
            {
                FollowPathAction(path, pathIndex);
                
            }
            yield return null;
        }
    }


    public void OnDrawGizmos()
    {
        if(path != null && DrawPath)
        {
            path.DrawWithGizmos();
        }
        
    }
}
