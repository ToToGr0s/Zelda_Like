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
    [SerializeField] private Color farFillColor = new Color(1f, 1f, 0f, 0.15f);
    [SerializeField] private Color farWireColor = Color.yellow;

    [SerializeField] private Color nearFillColor = new Color(1f, 0.5f, 0f, 0.15f);
    [SerializeField] private Color nearWireColor = new Color(1f, 0.5f, 0f, 1f);

    [SerializeField] private Color stopFillColor = new Color(0f, 1f, 1f, 0.15f);
    [SerializeField] private Color stopWireColor = Color.cyan;

    [SerializeField] private float gizmoHeight = 0.2f;
    [SerializeField] private float gizmoThickness = 0.1f;

    private Transform target;
    private List<Vector3> currentPath;
    private int currentIndex;
    private float repathTimer;
    private Quaternion initialRotation;
    private Rigidbody rb;

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
    }

    private void LateUpdate()
    {
        if (lockRotation)
            transform.rotation = initialRotation;
    }

    private void Update()
    {
        if (pathfinder == null || target == null || roomTrigger == null)
            return;

        if (!roomTrigger.bounds.Contains(target.position))
        {
            currentPath = null;
            currentIndex = 0;
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
            return;
        }

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
    private void DrawZone(Vector3 center, float range, Color fillColor, Color wireColor)
    {
        Vector3 size = new Vector3(range * 2f, gizmoThickness, range * 2f);

        Gizmos.color = fillColor;
        Gizmos.DrawCube(center, size);

        Gizmos.color = wireColor;
        Gizmos.DrawWireCube(center, size);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 center = transform.position + Vector3.up * gizmoHeight;

        DrawZone(center, farRange, farFillColor, farWireColor);
        DrawZone(center, nearRange, nearFillColor, nearWireColor);
        DrawZone(center, stopDistance, stopFillColor, stopWireColor);
    }
}