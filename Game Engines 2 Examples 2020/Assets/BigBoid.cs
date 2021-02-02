using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBoid : MonoBehaviour
{
    
    public Vector3 velocity;
    public float speed;
    public Vector3 acceleration;
    public Vector3 force;
    public float maxSpeed = 5;
    public float maxForce = 10;

    public float mass = 1;

    public bool seekEnabled = true;
    public Transform seekTargetTransform;
    public Vector3 seekTarget;

    public bool arriveEnabled = false;
    public Transform arriveTargetTransform;
    public Vector3 arriveTarget;
    public float slowingDistance = 10;

    public GameObject Path;
    private  List<Vector3> waypoints;
    private bool isLooped;
    private int waypointIndex = 0;
    public bool followPath = true;

    public bool fleeEnabled = true;
    public GameObject fleeFrom;

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + velocity);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + acceleration);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + force * 10);

        if (arriveEnabled)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(arriveTargetTransform.position, slowingDistance);
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        waypoints = Path.GetComponent<Path>().waypoints;
        isLooped = Path.GetComponent<Path>().isLooped;

        if (followPath) {
            waypointIndex = 0;
            FollowPath();
        }
    }

    public Vector3 Seek(Vector3 target)
    {
        Vector3 toTarget = target - transform.position;
        Vector3 desired = toTarget.normalized * maxSpeed;

        return (desired - velocity);
    } 

    public Vector3 Arrive(Vector3 target)
    {
        Vector3 toTarget = target - transform.position;
        float dist = toTarget.magnitude;
        float ramped = (dist / slowingDistance) * maxSpeed;
        float clamped = Mathf.Min(ramped, maxSpeed);
        Vector3 desired = (toTarget / dist) * clamped;

        return desired - velocity;
    }

    public Vector3 CalculateForce()
    {

        Vector3 f = Vector3.zero;
        if (seekEnabled)
        {
            if (seekTargetTransform != null)
            {
                seekTarget = seekTargetTransform.position;

            }
            f += Seek(seekTarget);
        }

        if (arriveEnabled)
        {
            if (arriveTargetTransform != null)
            {
                arriveTarget = arriveTargetTransform.position;                
            }
            f += Arrive(arriveTarget);
        }

        if (fleeEnabled) {
            float fleeDist = 10.0f;
            float dist = Vector3.Distance(transform.position, fleeFrom.transform.position);

            if (dist < fleeDist) {
                f += Flee(fleeFrom.transform.position);
            }
        }

        return f;
    }

    public void FollowPath() {
        if (followPath) {
            float dist = Vector3.Distance(transform.position, seekTarget);
            if (dist < 1.0f) {
                if (waypointIndex < waypoints.Count - 1) {
                    waypointIndex += 1;
                    seekTarget = waypoints[waypointIndex];
                    arriveTarget = waypoints[waypointIndex];

                } else {
                    if (isLooped) {
                        waypointIndex = (waypointIndex + 1) % waypoints.Count;
                        seekTarget = waypoints[waypointIndex];
                        arriveTarget = waypoints[waypointIndex];
                    }
                }
            }
        }
    }

    public Vector3 Flee(Vector3 target) {
        Vector3 awayFromTarget = transform.position - target;
        Vector3 desired = awayFromTarget.normalized * maxSpeed * 2;

        return (desired - velocity);
    }

    // Update is called once per frame
    void Update()
    {
        FollowPath();
        
        force = CalculateForce();
        acceleration = force / mass;
        velocity = velocity + acceleration * Time.deltaTime;
        transform.position = transform.position + velocity * Time.deltaTime;
        speed = velocity.magnitude;
        if (speed > 0)
        {
            transform.forward = velocity;
        }        
    }
}
