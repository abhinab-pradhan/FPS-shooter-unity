using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack : MonoBehaviour
{
    public Transform player;
    public float attackRange = 15f;
    public float shootCooldown = 1f;

    private float shootTimer = 0f;
    private NavMeshAgent agent;
    private EnemyFSM fsm;
    private EnemyPatrol patrol;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        fsm = GetComponent<EnemyFSM>();
        patrol = GetComponent<EnemyPatrol>();
    }

    public void AttackBehavior()
    {
        float dist = Vector3.Distance(transform.position, player.position);

        if (dist > attackRange)
        {
            patrol.SetLastSeenPosition(player.position); //  Save current last seen
            fsm.SwitchState(EnemyFSM.State.Search);
            return;
        }


        agent.SetDestination(player.position);

        shootTimer += Time.deltaTime;
        if (shootTimer >= shootCooldown)
        {
            Debug.Log("Enemy shoots at player!");
            shootTimer = 0f;
        }
    }
}
