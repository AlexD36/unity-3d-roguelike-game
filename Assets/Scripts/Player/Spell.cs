using System.Collections;
using UnityEngine;

public class Spell : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        GameObject.Destroy(Instantiate(Player.AttackExplosion, transform.position, Quaternion.identity), 3);

        Unit U;
        if (collision.collider.TryGetComponent<Unit>(out U))
        {
            U.TriggerMaterialChange();
            U.Health -= Player.Damage;
            if (U.Health <= 0)
            {
                GameObject.Destroy(Instantiate(U.EnemyDeathEffect,U.transform.position,U.transform.rotation),2);

                Level.EnemyCount--;
                U.gameObject.SetActive(false);

                if (Level.EnemyCount <= 0)
                {
                    Transform Doors = GameObject.Find(Player.CurrentRoom.RoomNumber.ToString()).transform.Find("Doors");

                    Player.CurrentRoom.Cleared = true;

                    GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("Door");

                    Animator A;
                    foreach (GameObject g in objectsWithTag)
                    {
                        if (g.TryGetComponent<Animator>(out A))
                            A.enabled = true;
                    }

                    if (collision.transform.parent != null)
                        if (collision.transform.parent.name == "Enemies")
                            GameObject.Destroy(collision.transform.parent.gameObject);
                }

            }
            else
            {
                U.HitPlayer = true;
                U.transform.GetComponent<Rigidbody>().AddForce((U.transform.position - transform.position) * Player.EnemyKnockBack, ForceMode.Impulse);
                CoroutineManager.Instance.StartManagedCoroutine(ResetHitPlayer(U));
            }
        }

        GameObject.Destroy(gameObject);
    }

    private IEnumerator ResetHitPlayer(Unit U)
    {
        yield return new WaitForSeconds(0.2f);
        U.HitPlayer = false;
    }
}

public class CoroutineManager : MonoBehaviour
{
    private static CoroutineManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static CoroutineManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("CoroutineManager");
                instance = go.AddComponent<CoroutineManager>();
            }
            return instance;
        }
    }

    public void StartManagedCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
}
