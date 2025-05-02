using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnAnimalMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveRadius = 10f;
    public float moveInterval = 5f;
    public float minWalkDuration = 3f;
    public float rotationSpeed = 120f; // degrees per second
    
    [Header("Rotational Movement")]
    public bool useRotationalMovement = true;
    public float arcAngle = 60f; // maximum turning angle
    public float arcProbability = 0.7f; // chance of arc movement vs straight
    public float circleRadius = 5f; // radius for circular movement
    public float spiralTightness = 0.5f; // how tight spiral movement is (0-1)
    
    [Header("Movement Types")]
    public bool enableArcs = true;
    public bool enableSpirals = true;
    public bool enableCircles = true;
    
    private NavMeshAgent agent;
    private float timer;
    private Vector3 currentTarget;
    private bool isMoving = false;
    private Coroutine movementCoroutine;
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent missing on " + gameObject.name);
            return;
        }
        
        agent.enabled = true;
        timer = moveInterval; // Start with movement
        
        // If we're using rotational movement, we'll control position ourselves
        if (useRotationalMovement)
        {
            agent.updatePosition = false;
            agent.updateRotation = false;
        }
        
        Debug.Log(gameObject.name + ": RotationalAnimalMovement initialized");
    }
    
    void Update()
    {
        // Update timer
        timer += Time.deltaTime;
        
        // If it's time to move and we're not already moving
        if (timer >= moveInterval && !isMoving)
        {
            if (useRotationalMovement)
            {
                // Choose and start a rotational movement pattern
                ChooseMovementPattern();
            }
            else
            {
                // Use standard NavMeshAgent movement
                SetNewRandomDestination();
            }
            
            timer = 0f;
        }
        
        // Debug current target
        if (currentTarget != Vector3.zero)
        {
            Debug.DrawLine(transform.position, currentTarget, Color.blue);
        }
    }
    
    void ChooseMovementPattern()
    {
        // Stop any existing movement
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
        }
        
        // List of available movement patterns
        System.Collections.Generic.List<System.Action> movements = new System.Collections.Generic.List<System.Action>();
        
        // Add enabled movement types
        if (enableArcs) movements.Add(() => StartArcMovement());
        if (enableSpirals) movements.Add(() => StartSpiralMovement());
        if (enableCircles) movements.Add(() => StartCircularMovement());
        
        // Default to straight movement if nothing is enabled
        if (movements.Count == 0)
        {
            StartStraightMovement();
            return;
        }
        
        // Choose and execute a random movement pattern
        int index = Random.Range(0, movements.Count);
        movements[index]();
    }
    
    void StartStraightMovement()
    {
        Vector3 destination = GetRandomNavMeshPoint();
        movementCoroutine = StartCoroutine(StraightMovementCoroutine(destination));
        Debug.Log(gameObject.name + ": Starting straight movement to " + destination);
    }
    
    void StartArcMovement()
    {
        Vector3 destination = GetRandomNavMeshPoint();
        Vector3 arcCenter = GetArcCenter(transform.position, destination);
        movementCoroutine = StartCoroutine(ArcMovementCoroutine(destination, arcCenter));
        Debug.Log(gameObject.name + ": Starting arc movement to " + destination);
    }
    
    void StartSpiralMovement()
    {
        Vector3 spiralCenter = GetRandomNavMeshPoint();
        movementCoroutine = StartCoroutine(SpiralMovementCoroutine(spiralCenter));
        Debug.Log(gameObject.name + ": Starting spiral movement around " + spiralCenter);
    }
    
    void StartCircularMovement()
    {
        Vector3 circleCenter = GetRandomNavMeshPoint();
        movementCoroutine = StartCoroutine(CircularMovementCoroutine(circleCenter));
        Debug.Log(gameObject.name + ": Starting circular movement around " + circleCenter);
    }
    
    IEnumerator StraightMovementCoroutine(Vector3 destination)
    {
        isMoving = true;
        currentTarget = destination;
        
        float duration = Random.Range(minWalkDuration, moveInterval * 0.8f);
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        
        while (elapsed < duration)
        {
            // Rotate towards destination
            RotateTowards(destination);
            
            // Move forward
            MoveForward();
            
            // Check if we're close to destination
            if (Vector3.Distance(transform.position, destination) < 0.5f)
                break;
                
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        isMoving = false;
    }
    
    IEnumerator ArcMovementCoroutine(Vector3 destination, Vector3 arcCenter)
    {
        isMoving = true;
        currentTarget = destination;
        
        float duration = Random.Range(minWalkDuration, moveInterval * 0.8f);
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        
        // Calculate arc parameters
        Vector3 toCenter = arcCenter - startPos;
        float radius = toCenter.magnitude;
        Vector3 startTangent = Vector3.Cross(toCenter, Vector3.up).normalized;
        
        // Determine if we should go clockwise or counterclockwise
        float direction = (Random.value > 0.5f) ? 1f : -1f;
        startTangent *= direction;
        
        while (elapsed < duration)
        {
            // Calculate position on arc
            float t = elapsed / duration;
            float angle = t * arcAngle * Mathf.Deg2Rad;
            
            // Calculate the rotation around the arc center
            Quaternion rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg * direction, Vector3.up);
            Vector3 targetDir = rotation * (startPos - arcCenter).normalized;
            Vector3 targetPoint = arcCenter + targetDir * radius;
            
            // Rotate towards the next point on the arc
            RotateTowards(targetPoint);
            
            // Move forward
            MoveForward();
            
            // Check if we're close to destination
            if (Vector3.Distance(transform.position, destination) < 0.5f)
                break;
                
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        isMoving = false;
    }
    
    IEnumerator SpiralMovementCoroutine(Vector3 spiralCenter)
    {
        isMoving = true;
        currentTarget = spiralCenter;
        
        float duration = Random.Range(minWalkDuration, moveInterval * 0.8f);
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        
        // Calculate spiral parameters
        Vector3 toCenter = spiralCenter - startPos;
        float startRadius = toCenter.magnitude;
        float endRadius = startRadius * (1f - spiralTightness);
        
        // Determine direction (clockwise or counterclockwise)
        float direction = (Random.value > 0.5f) ? 1f : -1f;
        
        while (elapsed < duration)
        {
            // Calculate position on spiral
            float t = elapsed / duration;
            float currentRadius = Mathf.Lerp(startRadius, endRadius, t);
            float angle = t * 360f * 2f * Mathf.Deg2Rad; // 2 full rotations
            
            // Calculate the rotation around the spiral center
            Quaternion rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg * direction, Vector3.up);
            Vector3 targetDir = rotation * (startPos - spiralCenter).normalized;
            Vector3 targetPoint = spiralCenter + targetDir * currentRadius;
            
            // Rotate towards the next point on the spiral
            RotateTowards(targetPoint);
            
            // Move forward
            MoveForward();
                
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        isMoving = false;
    }
    
    IEnumerator CircularMovementCoroutine(Vector3 circleCenter)
    {
        isMoving = true;
        currentTarget = circleCenter;
        
        float duration = Random.Range(minWalkDuration, moveInterval * 0.8f);
        float elapsed = 0f;
        
        // Calculate circle parameters
        Vector3 toCenter = circleCenter - transform.position;
        float radius = Mathf.Min(toCenter.magnitude, circleRadius);
        Vector3 startTangent = Vector3.Cross(toCenter, Vector3.up).normalized;
        
        // Determine direction (clockwise or counterclockwise)
        float direction = (Random.value > 0.5f) ? 1f : -1f;
        startTangent *= direction;
        
        while (elapsed < duration)
        {
            // Calculate position on circle
            float t = elapsed / duration;
            float angle = t * 360f * Mathf.Deg2Rad; // One full rotation
            
            // Calculate the rotation around the circle center
            Quaternion rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg * direction, Vector3.up);
            Vector3 targetDir = rotation * (transform.position - circleCenter).normalized;
            Vector3 targetPoint = circleCenter + targetDir * radius;
            
            // Rotate towards the tangent direction
            Vector3 tangent = Vector3.Cross(targetPoint - circleCenter, Vector3.up).normalized * direction;
            RotateTowards(transform.position + tangent);
            
            // Move forward
            MoveForward();
                
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        isMoving = false;
    }
    
    void RotateTowards(Vector3 target)
    {
        // Calculate direction to target
        Vector3 direction = (target - transform.position).normalized;
        
        // Only rotate if we have a direction
        if (direction != Vector3.zero)
        {
            // Create target rotation
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            
            // Smoothly rotate towards the target direction
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
    
    void MoveForward()
    {
        // Move in the forward direction
        transform.position += transform.forward * agent.speed * Time.deltaTime;
        
        // Update NavMeshAgent position (important to keep it in sync)
        if (agent.enabled)
        {
            agent.nextPosition = transform.position;
        }
        
        // Make sure we stay on the NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 1f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
        }
    }
    
    Vector3 GetArcCenter(Vector3 start, Vector3 end)
    {
        // Calculate midpoint between start and end
        Vector3 midpoint = (start + end) / 2f;
        
        // Calculate direction from start to end
        Vector3 direction = (end - start).normalized;
        
        // Calculate perpendicular direction (left or right)
        Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized;
        if (Random.value > 0.5f) perpendicular = -perpendicular;
        
        // Calculate distance between start and end
        float distance = Vector3.Distance(start, end);
        
        // Calculate offset distance based on arc angle
        float offsetDistance = distance / (2f * Mathf.Tan(arcAngle * 0.5f * Mathf.Deg2Rad));
        
        // Clamp the offset distance to avoid extreme values
        offsetDistance = Mathf.Clamp(offsetDistance, 1f, 20f);
        
        // Calculate arc center position
        Vector3 arcCenter = midpoint + perpendicular * offsetDistance;
        
        return arcCenter;
    }
    
    void SetNewRandomDestination()
    {
        Vector3 destination = GetRandomNavMeshPoint();
        agent.SetDestination(destination);
        currentTarget = destination;
        Debug.Log(gameObject.name + ": Moving to " + destination);
    }
    
    Vector3 GetRandomNavMeshPoint()
    {
        // Generate random direction and position
        Vector3 randomDirection = Random.insideUnitSphere * moveRadius;
        randomDirection += transform.position;
        randomDirection.y = transform.position.y; // Stay on the same height level
        
        // Find nearest point on NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, moveRadius, NavMesh.AllAreas))
        {
            return hit.position;
        }
        
        // Fallback to current position if no valid position found
        Debug.LogWarning(gameObject.name + ": Could not find valid NavMesh position");
        return transform.position;
    }
    
    // Called by Unity Editor button
    public void ForceMoveNow()
    {
        // Stop any current movement
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
            isMoving = false;
        }
        
        // Start new movement
        if (useRotationalMovement)
        {
            ChooseMovementPattern();
        }
        else
        {
            SetNewRandomDestination();
        }
        
        timer = 0f;
    }
    
    // Visualize movement radius and targets
    void OnDrawGizmosSelected()
    {
        // Draw movement radius
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, moveRadius);
        
        // Draw current target
        if (currentTarget != Vector3.zero)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, currentTarget);
            Gizmos.DrawSphere(currentTarget, 0.3f);
        }
    }
}