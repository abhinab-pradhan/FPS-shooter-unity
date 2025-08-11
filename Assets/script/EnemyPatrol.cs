using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float patrolWaitTime = 2f;
    public float detectionRange = 10f;
    public Transform player;

    private NavMeshAgent agent;
    private int currentPatrolIndex = 0;
    private float patrolTimer = 0f;
    private EnemyFSM fsm;
    private Vector3 lastSeenPosition;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        fsm = GetComponent<EnemyFSM>();
    }

    public void PatrolBehavior()
    {
        if (patrolPoints.Length == 0) return;

        if (Vector3.Distance(transform.position, player.position) < detectionRange)
        {
            fsm.SwitchState(EnemyFSM.State.Attack);
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            patrolTimer += Time.deltaTime;

            if (patrolTimer >= patrolWaitTime)
            {
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                agent.SetDestination(patrolPoints[currentPatrolIndex].position);
                patrolTimer = 0f;
            }
        }
        else
        {
            patrolTimer = 0f;
        }
    }

    public void SearchLastKnownPosition()
    {
        if (Vector3.Distance(transform.position, lastSeenPosition) > agent.stoppingDistance + 0.2f)
        {
            agent.SetDestination(lastSeenPosition);
        }

        // Check if enemy has arrived and waited a bit
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            patrolTimer += Time.deltaTime;

            if (patrolTimer >= patrolWaitTime)
            {
                fsm.SwitchState(EnemyFSM.State.Patrol);
                patrolTimer = 0f;
            }
        }
        else
        {
            patrolTimer = 0f; // reset timer if still moving
            fsm.SwitchState(EnemyFSM.State.Patrol);
        }
    
    Debug.Log("Searching... Distance left: " + agent.remainingDistance);

}


    public void SetLastSeenPosition(Vector3 pos)
    {
        lastSeenPosition = pos;
    }
}
