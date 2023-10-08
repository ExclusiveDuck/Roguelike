using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBasicMelee : MonoBehaviour
{
    public float damage;
    public BoxCollider attackCollider;
    public float attackLifeSpan;
    EnemyNavigation enemyNav;
    Animator anim;

    private void Start()
    {
        enemyNav = GetComponent<EnemyNavigation>();
        anim = GetComponent<Animator>();
    }
    public void Attack()
    {
        anim.SetBool("Attack", true);
    }

    public void ActuallyAttack()
    {
        attackCollider.enabled = true;
        enemyNav.navAgent.speed = 0;
        Invoke("ResetAttack", attackLifeSpan);
        
    }

    private void ResetAttack()
    {
        attackCollider.enabled = false;
        enemyNav.navAgent.speed = 17;
        anim.SetBool("Attack", false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerStats>().TakeDamage(damage);
        }
    }
}
