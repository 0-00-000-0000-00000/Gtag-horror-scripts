using UnityEngine;
using UnityEngine.AI;

public class HorrorAI : MonoBehaviour
{
    [Header("AI Configuration - Made by IDK83")]
    [SerializeField] private NavMeshAgent mob;
    [SerializeField] private string targetTag = "GorillaPlayer"; // Tag of the target to chase
    [SerializeField] private float chaseRange = 4f; // Range to start chasing
    [SerializeField] private float wanderRange = 10f; // Range for wandering
    [SerializeField] private float enemySpeed = 3f; // AI movement speed
    [SerializeField] private float wanderInterval = 3f; // Time between wander destination changes
    [SerializeField] private float targetCheckInterval = 0.5f; // How often to check for targets

    private enum AIState { Idle, Wander, Chase }
    private AIState currentState = AIState.Idle;
    private Vector3 wanderDestination;
    private Transform currentTarget;
    private float wanderTimer;
    private float targetCheckTimer;

    private void Awake()
    {
        // Validate NavMeshAgent
        if (mob == null)
            mob = GetComponent<NavMeshAgent>();
        
        if (mob == null)
        {
            Debug.LogError("NavMeshAgent component missing on " + gameObject.name);
            enabled = false;
            return;
        }

        // Initialize NavMeshAgent settings
        mob.speed = enemySpeed;
        mob.stoppingDistance = 0.1f;
        wanderTimer = wanderInterval;
    }

    private void Update()
    {
        // Update target check timer
        targetCheckTimer -= Time.deltaTime;
        if (targetCheckTimer <= 0)
        {
            UpdateTarget();
            targetCheckTimer = targetCheckInterval;
        }

        switch (currentState)
        {
            case AIState.Idle:
                TransitionToWander();
                break;
            case AIState.Wander:
                Wander();
                break;
            case AIState.Chase:
                Chase();
                break;
        }
    }

    private void UpdateTarget()
    {
        // Find all potential targets with the specified tag
        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
        currentTarget = GetClosestTarget(targets);

        if (currentTarget != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);
            currentState = distanceToTarget <= chaseRange ? AIState.Chase : AIState.Wander;
        }
        else
        {
            currentState = AIState.Wander;
        }
    }

    private Transform GetClosestTarget(GameObject[] targets)
    {
        Transform closest = null;
        float minDistance = float.MaxValue;

        foreach (GameObject target in targets)
        {
            if (target == null) continue;
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = target.transform;
            }
        }

        return closest;
    }

    private void Wander()
    {
        if (currentTarget != null && Vector3.Distance(transform.position, currentTarget.position) <= chaseRange)
        {
            currentState = AIState.Chase;
            return;
        }

        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0 || !mob.hasPath || mob.remainingDistance <= mob.stoppingDistance)
        {
            // Generate new wander destination
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * wanderRange;
            randomPoint.y = transform.position.y; // Keep Y consistent for 3D environments
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, wanderRange, NavMesh.AllAreas))
            {
                wanderDestination = hit.position;
                mob.SetDestination(wanderDestination);
                wanderTimer = wanderInterval;
            }
        }
    }

    private void Chase()
    {
        if (currentTarget == null || Vector3.Distance(transform.position, currentTarget.position) > chaseRange)
        {
            TransitionToWander();
            return;
        }

        mob.SetDestination(currentTarget.position);
    }

    private void TransitionToWander()
    {
        currentState = AIState.Wander;
        wanderTimer = 0; // Force new wander destination
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize chase range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        // Visualize wander range
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, wanderRange);
    }

    // Public method to adjust speed dynamically if needed
    public void SetSpeed(float newSpeed)
    {
        enemySpeed = Mathf.Max(0, newSpeed);
        if (mob != null)
            mob.speed = enemySpeed;
    }
}