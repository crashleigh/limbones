using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] EnemyAttack enemyAttack;
    [SerializeField] EnemyPatrol enemyPatrol;
    [SerializeField] Goob goob;

    private void Awake()
    {
        enemyPatrol = GetComponentInParent<EnemyPatrol>();
        goob = GetComponentInParent<Goob>();
        enemyAttack = GetComponentInParent<EnemyAttack>();
    }


    void Update()
    {
        if (goob.isAttack)
        {
            animator.SetBool("IsAttack", true);
            animator.SetBool("IsIdle", false);
            animator.SetBool("IsWalk", false);
        }

        if (enemyPatrol.isMoving)
        {
            animator.SetBool("IsWalk", true);
            animator.SetBool("IsIdle", false);
            animator.SetBool("IsAttack", false);

        }

        if( enemyPatrol.isIdle)
        {
            animator.SetBool("IsIdle", true);
            animator.SetBool("IsWalk", false);
            animator.SetBool("IsAttack", false);
        }



    }
}
