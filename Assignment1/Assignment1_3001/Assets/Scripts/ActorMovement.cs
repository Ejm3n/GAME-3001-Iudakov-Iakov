using UnityEngine;
public enum ActorState
{
    Seeking,
    Fleeing,
    Arrival,
    Avoidance
}

public class ActorMovement : MonoBehaviour
{
  
    [SerializeField] ActorState state;
    [SerializeField] Transform targetTransform;
    [SerializeField] float rotationSpeed;
    [SerializeField] float movementspeed;
    [SerializeField] float whiskerLength;
    [SerializeField] float distanceToStartSlow;
    [SerializeField] float whiskerAngle;
    [SerializeField] float avoidanceWeight;
    Rigidbody2D rb;

   

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {        
        switch (state)
        {
            case ActorState.Seeking:
                Seek();
                break;
            case ActorState.Fleeing:
                Flee();
                break;
            case ActorState.Arrival:
                Arrival();
                break;
            case ActorState.Avoidance:
                Avoidance();
                break;
            default:
                Debug.LogError("Unknown behavior!");
                break;
        }
    }
    
    public void UpdateActor(ActorState state, Transform targetPosition)
    {
        this.state = state;
        this.targetTransform = targetPosition;
    }
    void Seek()
    {

        //Calculate direction to the target
        Vector2 directiototarget = (targetTransform.position - transform.position).normalized;

        //Calculate the angle to rotate towards to the target
        float targetangle = Mathf.Atan2(directiototarget.y, directiototarget.x) * Mathf.Rad2Deg + 90.0f;

        //Smoothly Rotate towards the target
        float angledifference = Mathf.DeltaAngle(targetangle, transform.eulerAngles.z);
        float rotationStep = rotationSpeed * Time.deltaTime;
        float rotationamount = Mathf.Clamp(angledifference, -rotationStep, rotationStep);

        transform.Rotate(Vector3.forward, rotationamount);

        //move along the forward vector using rigidbody2d

        rb.velocity = transform.up * movementspeed;
    }
    void Flee()
    {
        Vector3 dir = targetTransform.position - transform.position;
        float angle = 180 + Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Vector2 directiototarget = (targetTransform.position - transform.position).normalized;

        float targetangle = Mathf.Atan2(directiototarget.y, directiototarget.x) * Mathf.Rad2Deg + 270.0f;

        float angledifference = Mathf.DeltaAngle(targetangle, transform.eulerAngles.z);
        float rotationStep = rotationSpeed * Time.deltaTime;
        float rotationamount = Mathf.Clamp(angledifference, -rotationStep, rotationStep);
        transform.Rotate(Vector3.forward, rotationamount);

        bool hitBack = CastWiskers(180, Color.red, float.MaxValue);
        if (hitBack)
        {
            rb.velocity = transform.up * movementspeed;
        }
    }
    void Arrival()
    {
        //Calculate direction to the target
        Vector2 directiototarget = (targetTransform.position - transform.position).normalized;

        //Calculate the angle to rotate towards to the target
        float targetangle = Mathf.Atan2(directiototarget.y, directiototarget.x) * Mathf.Rad2Deg + 90.0f;

        //Smoothly Rotate towards the target
        float angledifference = Mathf.DeltaAngle(targetangle, transform.eulerAngles.z);
        float rotationStep = rotationSpeed * Time.deltaTime;
        float rotationamount = Mathf.Clamp(angledifference, -rotationStep, rotationStep);

        transform.Rotate(Vector3.forward, rotationamount);

        float speedRaito = 1;
        float distanceToTarget = Vector2.Distance(transform.position, targetTransform.position);
        if (distanceToTarget < distanceToStartSlow)
            speedRaito = distanceToTarget / distanceToStartSlow;
        rb.velocity = transform.up * movementspeed * speedRaito;
    }
    void Avoidance()
    {
        Seek();
        AvoidObstacles();
    }
    private bool CastWiskers(float angle, Color color, float whiskerLength)
    {
        bool hitResult = false;
        Color rayColor = color;
        Vector2 whiskerDirection = Quaternion.Euler(0, 0, angle) * transform.up;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, whiskerDirection, whiskerLength);

        if (hit.collider != null)
        {
            Debug.Log("Obstacle detected!");
            rayColor = Color.green;
            hitResult = true;
        }
        Debug.DrawRay(transform.position, whiskerDirection * whiskerLength, rayColor);
        return hitResult;
    }
    private void AvoidObstacles()
    {
        // Cast whiskers to detect obstacles.
        bool hitLeft = CastWiskers(whiskerAngle, Color.red, whiskerLength);
        bool hitRight = CastWiskers(-whiskerAngle, Color.blue, whiskerLength);
        bool hitBackLeft = CastWiskers(whiskerAngle*2, Color.cyan, whiskerLength / 2);
        bool hitBackRight = CastWiskers(-whiskerAngle*2, Color.green, whiskerLength / 2);
        //

        // Adjust rotation based on detected obstacles.
        if (hitLeft && !hitRight)
        {
            RotateClockwise();
        }
        else if (hitRight && !hitLeft)
        {
            RotateCounterClockwise();
        }
        else if (hitBackLeft && !hitLeft && !hitRight)
        {
            RotateClockwise();
        }
        else if (hitBackRight && !hitRight && !hitLeft)
        {
            RotateCounterClockwise();

        }
    }
    private void RotateClockwise()
    {
        transform.Rotate(Vector3.forward, -rotationSpeed * avoidanceWeight * Time.deltaTime);
    }
    private void RotateCounterClockwise()
    {
        transform.Rotate(Vector3.forward, rotationSpeed * avoidanceWeight * Time.deltaTime);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Target"))
        {
            SoundManager.Instance.PlaySound("Target");
        }
    }
}
