using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ImpactReceiver : MonoBehaviour
{
    public static Dictionary<GameObject, List<Vector3>> forcesOnGameObjects = new Dictionary<GameObject, List<Vector3>>();

    void FixedUpdate()
    {
        if (Player.State == "Dead")
        {
            forcesOnGameObjects.Clear();
            return;
        }

        List<GameObject> gameObjectsToRemove = new List<GameObject>();

        foreach (KeyValuePair<GameObject, List<Vector3>> gameObjectToBeMoved in forcesOnGameObjects.ToList())
        {
            GameObject target = gameObjectToBeMoved.Key;

            if (target.name == "StickmanContainer")
            {
                if (Physics.OverlapSphere(target.transform.position, 10, LayerMask.NameToLayer("Default")).Count() > 0)
                {
                    forcesOnGameObjects.Clear();
                }
            }
            if (target == null)
            {
                gameObjectsToRemove.Add(gameObjectToBeMoved.Key);
                continue;
            }

            List<Vector3> impacts = gameObjectToBeMoved.Value;
            Vector3 finalImpact = Vector3.zero;
            foreach (Vector3 impact in impacts) finalImpact += impact;

            if (finalImpact.magnitude > .2f)
            {
                
                target.GetComponent<CharacterController>().Move(new Vector3(finalImpact.x, 0, finalImpact.z) * Time.deltaTime);
            }

            for (int i = 0; i < impacts.Count; i++)
            {
                if (forcesOnGameObjects.Count != 0)
                    forcesOnGameObjects[gameObjectToBeMoved.Key][i] = Vector3.Lerp(impacts[i], Vector3.zero, 5 * Time.deltaTime);
            }
        }
        foreach (GameObject gameObjectToRemove in gameObjectsToRemove.ToList())
        {
            if (forcesOnGameObjects.Count != 0)
                forcesOnGameObjects.Remove(gameObject);
        }

    }

    public static void AddImpactOnGameObject(GameObject target, Vector3 impact)
    {
        if (Player.State == "Dead") return;

        if (!forcesOnGameObjects.ContainsKey(target))
        {
            List<Vector3> existingImpacts = new List<Vector3>();
            existingImpacts.Add(impact);
            forcesOnGameObjects.Add(target, existingImpacts);
        }
        else
        {
            List<Vector3> existingImpacts = forcesOnGameObjects[target];
            existingImpacts.Add(impact);
            forcesOnGameObjects[target] = existingImpacts;
        }
    }
}
