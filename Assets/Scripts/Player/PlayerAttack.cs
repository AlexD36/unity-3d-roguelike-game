using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private bool isAttacking = false;

    public void FinishAttacking()
    {
        isAttacking = false;
        Player.State = "Idle";
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
