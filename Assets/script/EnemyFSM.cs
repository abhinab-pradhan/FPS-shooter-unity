using UnityEngine;

public class EnemyFSM : MonoBehaviour
{
    public enum State { Patrol, Attack, Search }
    public State currentState = State.Patrol;

    private EnemyPatrol patrol;
    private EnemyAttack attack;

    private void Start()
    {
        patrol = GetComponent<EnemyPatrol>();
        attack = GetComponent<EnemyAttack>();
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Patrol:
                patrol.PatrolBehavior();
                break;

            case State.Attack:
                attack.AttackBehavior();
                break;

            case State.Search:
                patrol.SearchLastKnownPosition();
                break;
        }
    }

    public void SwitchState(State newState)
    {
        currentState = newState;
    }
}
