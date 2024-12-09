using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private bool isAttacking = false;

    public void FinishAttacking()
    {
        isAttacking = false;
        Player.State = "Idle";
    }

    public void Attack()
    {
        GameObject g = Instantiate(Player.Attack, transform.position + transform.forward * 5, Player.Attack.transform.rotation);
        g.GetComponent<Rigidbody>().linearVelocity = transform.forward * 70;
        GameObject.Destroy(g, 5);
    }

    void Update()
    {
        if (Input.GetButton("Fire1") && !isAttacking)
        {
            isAttacking = true;
            Player.State = "Attacking";

            string AttackNumber = Random.Range(1,5).ToString();

            Player.animator.Play("Attack" +  AttackNumber);
        }
    }
}
