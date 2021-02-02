using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    public List<Vector3> waypoints = new List<Vector3>();
    public bool isLooped = true;

    void Awake() {
        createWaypoints();
    }

    public void createWaypoints() {
        Vector3 v0 = this.transform.position;
        waypoints.Add(v0);

        foreach (Transform child in transform)
        {
            waypoints.Add(child.position);
        }
    }

    public void OnDrawGizmos()
    {
         Gizmos.color = Color.cyan;
        for (int i = 0; i < waypoints.Count; i++) {
            Vector3 waypoint = waypoints[i];

            Gizmos.DrawSphere(waypoint, 1);

            if (i < waypoints.Count - 1) {
                Gizmos.DrawLine(waypoints[i], waypoints[i + 1]);

            } else if (i == waypoints.Count - 1 && isLooped) {
                Gizmos.DrawLine(waypoints[i], waypoints[0]);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
