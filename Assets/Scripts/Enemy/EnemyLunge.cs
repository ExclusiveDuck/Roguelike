using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLunge : MonoBehaviour
{
    EnemyNavigation enemyNav;
    public float lungeSpeed;
    public float lungeTime;
    bool canAttack = true;

    private void Start()
    {
        enemyNav = GetComponent<EnemyNavigation>();
    }
    public void Attack()
    {
        if (canAttack)
        {
            canAttack = false;
            
            StartCoroutine(LungeAttack());
        }
    }

    private IEnumerator LungeAttack()
    {
        float startTime = Time.time;
        while (Time.time < startTime + lungeTime)
        {
            Debug.Log("Lunge");
            transform.position += enemyNav.player.forward * lungeSpeed * Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(2);
        canAttack = true;
    }
}
