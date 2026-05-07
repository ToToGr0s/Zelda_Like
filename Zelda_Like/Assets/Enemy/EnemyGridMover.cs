using System.Collections.Generic;
using UnityEngine;

public class EnemyGridMover : MonoBehaviour
{
    [SerializeField] private RoomGridPathfinder pathfinder;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float farChaseSpeed = 4f;
    [SerializeField] private float nearChaseSpeed = 2f;
    [SerializeField] private float farRange = 12f;
    [SerializeField] private float nearRange = 4f;
    [SerializeField] private float stopDistance = 1.2f;
    [SerializeField] private float reachDistance = 0.1f;
    [SerializeField] private float repathInterval = 0.4f;
    [SerializeField] private bool lockRotation = true;
    [SerializeField] private Collider roomTrigger;

    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float minPatrolWidth = 2f;
    [SerializeField] private float maxPatrolWidth = 6f;
    [SerializeField] private float minPatrolHeight = 2f;
    [SerializeField] private float maxPatrolHeight = 6f;
    [SerializeField] private float maxPatrolOffsetX = 4f;
    [SerializeField] private float maxPatrolOffsetZ = 4f;
    [SerializeField] private float patrolPointWaitTime = 0.2f;
    [SerializeField] private float patrolLoopWaitTime = 0.6f;
    [SerializeField] private bool randomizePatrolDirection = true;
    [SerializeField] private bool forcePerfectSquare = false;

    [SerializeField] private Color farFillColor = new Color(1f, 1f, 0f, 0.15f);
    [SerializeField] private Color farWireColor = Color.yellow;
    [SerializeField] private Color nearFillColor = new Color(1f, 0.5f, 0f, 0.15f);
    [SerializeField] private Color nearWireColor = new Color(1f, 0.5f, 0f, 1f);
    [SerializeField] private Color stopFillColor = new Color(0f, 1f, 1f, 0.15f);
    [SerializeField] private Color stopWireColor = Color.cyan;
    [SerializeField] private bool drawPatrolZone = true;
    [SerializeField] private Color patrolFillColor = new Color(0f, 1f, 0f, 0.08f);
    [SerializeField] private Color patrolWireColor = Color.green;
    [SerializeField] private float gizmoHeight = 0.2f;
    [SerializeField] private float gizmoThickness = 0.1f;

    private Transform target;
    private List<Vector3> currentPath;
    private int currentIndex;
    private float repathTimer;
    private Quaternion initialRotation;
    private Rigidbody rb;

    private Vector3 patrolOrigin;
    private List<Vector3> patrolPoints = new List<Vector3>();
    private int patrolPointIndex;
    private float patrolWaitTimer;
    private Vector3 currentPatrolCenter;
    private float currentPatrolWidth;
    private float currentPatrolHeight;

    private void Awake()
    {
        initialRotation = transform.rotation;
        rb = GetComponent<Rigidbody>();

        if (rb != null && lockRotation)
            rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);

        if (playerObject != null)
            target = playerObject.transform;

        if (pathfinder == null)
            pathfinder = GetComponentInParent<RoomGridPathfinder>();

        if (roomTrigger == null)
            roomTrigger = GetComponentInParent<Collider>();

        patrolOrigin = transform.position;
        GenerateNewPatrolRoute();
        
    }
    public void Init(RoomGridPathfinder pathfinder, Collider roomTrigger)
    {
        this.pathfinder = pathfinder;
        this.roomTrigger = roomTrigger;
    }
    private void LateUpdate()
    {
        if (lockRotation)
            transform.rotation = initialRotation;
    }

    private void Update()
    {
        if (pathfinder == null || roomTrigger == null)
            return;

        if (target == null)
        {
            HandlePatrol();
            return;
        }

        if (!roomTrigger.bounds.Contains(target.position))
        {
            currentPath = null;
            currentIndex = 0;
            HandlePatrol();
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, target.position);

        if (distanceToPlayer <= stopDistance)
        {
            currentPath = null;
            currentIndex = 0;
            return;
        }

        if (distanceToPlayer > farRange)
        {
            currentPath = null;
            currentIndex = 0;
            HandlePatrol();
            return;
        }

        patrolWaitTimer = 0f;

        float currentSpeed = distanceToPlayer <= nearRange ? nearChaseSpeed : farChaseSpeed;

        repathTimer -= Time.deltaTime;

        if (repathTimer <= 0f)
        {
            currentPath = pathfinder.FindPath(transform.position, target.position);
            currentIndex = 0;
            repathTimer = repathInterval;
        }

        if (currentPath == null || currentPath.Count == 0 || currentIndex >= currentPath.Count)
            return;

        Vector3 nextPoint = currentPath[currentIndex];
        Vector3 moveTarget = new Vector3(nextPoint.x, transform.position.y, nextPoint.z);

        transform.position = Vector3.MoveTowards(transform.position, moveTarget, currentSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, moveTarget) <= reachDistance)
            currentIndex++;
    }

    private void HandlePatrol()
    {
        if (patrolPoints == null || patrolPoints.Count == 0)
            GenerateNewPatrolRoute();

        if (patrolWaitTimer > 0f)
        {
            patrolWaitTimer -= Time.deltaTime;
            return;
        }

        if (currentPath == null || currentPath.Count == 0 || currentIndex >= currentPath.Count)
        {
            if (patrolPointIndex >= patrolPoints.Count)
            {
                GenerateNewPatrolRoute();
                patrolWaitTimer = patrolLoopWaitTime;
                return;
            }

            currentPath = pathfinder.FindPath(transform.position, patrolPoints[patrolPointIndex]);
            currentIndex = 0;
        }

        if (currentPath == null || currentPath.Count == 0)
        {
            patrolPointIndex++;

            if (patrolPointIndex >= patrolPoints.Count)
            {
                GenerateNewPatrolRoute();
                patrolWaitTimer = patrolLoopWaitTime;
            }

            return;
        }

        Vector3 nextPoint = currentPath[currentIndex];
        Vector3 moveTarget = new Vector3(nextPoint.x, transform.position.y, nextPoint.z);

        transform.position = Vector3.MoveTowards(transform.position, moveTarget, patrolSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, moveTarget) <= reachDistance)
            currentIndex++;

        if (currentIndex >= currentPath.Count)
        {
            currentPath = null;
            currentIndex = 0;
            patrolPointIndex++;
            patrolWaitTimer = patrolPointWaitTime;
        }
    }

    private void GenerateNewPatrolRoute()
    {
        patrolPoints.Clear();
        patrolPointIndex = 0;
        currentPath = null;
        currentIndex = 0;

        float width = Random.Range(minPatrolWidth, maxPatrolWidth);
        float height = forcePerfectSquare ? width : Random.Range(minPatrolHeight, maxPatrolHeight);

        Vector3 centerOffset = new Vector3(
            Random.Range(-maxPatrolOffsetX, maxPatrolOffsetX),
            0f,
            Random.Range(-maxPatrolOffsetZ, maxPatrolOffsetZ)
        );

        Vector3 center = patrolOrigin + centerOffset;

        Vector3 topLeft = center + new Vector3(-width * 0.5f, 0f, height * 0.5f);
        Vector3 topRight = center + new Vector3(width * 0.5f, 0f, height * 0.5f);
        Vector3 bottomRight = center + new Vector3(width * 0.5f, 0f, -height * 0.5f);
        Vector3 bottomLeft = center + new Vector3(-width * 0.5f, 0f, -height * 0.5f);

        topLeft = pathfinder.GetNearestWalkableWorld(topLeft);
        topRight = pathfinder.GetNearestWalkableWorld(topRight);
        bottomRight = pathfinder.GetNearestWalkableWorld(bottomRight);
        bottomLeft = pathfinder.GetNearestWalkableWorld(bottomLeft);

        bool clockwise = randomizePatrolDirection ? Random.value > 0.5f : true;

        if (clockwise)
        {
            patrolPoints.Add(topLeft);
            patrolPoints.Add(topRight);
            patrolPoints.Add(bottomRight);
            patrolPoints.Add(bottomLeft);
        }
        else
        {
            patrolPoints.Add(topLeft);
            patrolPoints.Add(bottomLeft);
            patrolPoints.Add(bottomRight);
            patrolPoints.Add(topRight);
        }

        currentPatrolCenter = center;
        currentPatrolWidth = width;
        currentPatrolHeight = height;
    }

    private void DrawZone(Vector3 center, float range, Color fillColor, Color wireColor)
    {
        Vector3 size = new Vector3(range * 2f, gizmoThickness, range * 2f);

        Gizmos.color = fillColor;
        Gizmos.DrawCube(center, size);

        Gizmos.color = wireColor;
        Gizmos.DrawWireCube(center, size);
    }

    private void DrawPatrolZone()
    {
        if (!drawPatrolZone)
            return;

        Vector3 center = currentPatrolCenter + Vector3.up * gizmoHeight;
        Vector3 size = new Vector3(currentPatrolWidth, gizmoThickness, currentPatrolHeight);

        Gizmos.color = patrolFillColor;
        Gizmos.DrawCube(center, size);

        Gizmos.color = patrolWireColor;
        Gizmos.DrawWireCube(center, size);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 center = transform.position + Vector3.up * gizmoHeight;

        DrawZone(center, farRange, farFillColor, farWireColor);
        DrawZone(center, nearRange, nearFillColor, nearWireColor);
        DrawZone(center, stopDistance, stopFillColor, stopWireColor);
        DrawPatrolZone();
    }
}