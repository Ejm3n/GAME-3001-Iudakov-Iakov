using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarShip : AgentObject
{
    [SerializeField]
    float movementspeed;
    [SerializeField]
    float rotationspeed;


    Vector2 initialPosition;

    Rigidbody2D rb;
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        Debug.Log("Starting Starship");

        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (TargetPosition != null)
        {
            //   Seek();
            SeekForward();
            //  ResetStarShip();

        }
    }
    private void Seek()
    {
        //Calculate desired velocity using kinematic seek equation
        Vector2 desiredvelocity = (TargetPosition - transform.position).normalized * movementspeed;


        //calculate the steering force

        Vector2 steeringforce = desiredvelocity - rb.velocity;

        //apply the steering force to the agent
        rb.AddForce(steeringforce);


    }
    private void SeekForward()//always move toward while rotate to the target
    {
        //Calculate direction to the target
        Vector2 directiototarget = (TargetPosition - transform.position).normalized;

        //Calculate the angle to rotate towards to the target
        float targetangle = Mathf.Atan2(directiototarget.y, directiototarget.x) * Mathf.Rad2Deg + 90.0f;

        //Smoothly Rotate towards the target
        float angledifference = Mathf.DeltaAngle(targetangle, transform.eulerAngles.z);//delate angle from target to ship
        float rotationStep = rotationspeed * Time.deltaTime;
        float rotationamount = Mathf.Clamp(angledifference, -rotationStep, rotationStep);

        transform.Rotate(Vector3.forward, rotationamount);

        //move along the forward vector using rigidbody2d

        rb.velocity = transform.up * movementspeed;

    }
    public void Restart()
    {
        SceneLoader.LoadSceneByIndex(0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Planet"))
        {
            Restart();
        }
    }
}
