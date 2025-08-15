using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryTrackingController : MonoBehaviour
{
    // some settings
    public float TrackingTimeFrame; // how often we provide tracking
    public float DistanceThresholdForAssitance; // how far away (in meters) do you need to receive tracking

    // leader and follower device information so we can get data in a Vector3 for convenience
    [SerializeField] private HapticPluginDataReader leaderDeviceReader;
    [SerializeField] private HapticPluginDataReader followerDeviceReader;

    // tracking state
    private bool isTracking; // are we currently tracking
    private Coroutine trackingRoutine; // reference to the tracking routine so we can stop it

    private void Update()
    {
        HandleKeyboardInput();
    }

    // start and stop the tracking routine
    private void HandleKeyboardInput()
    {
        // if already tracking and space bar is pressed, then stop tracking
        if (isTracking)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StopCoroutine(trackingRoutine);
                isTracking = false;
            }
        }
        else // if not tracking and space bar is pressed, then start tracking
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isTracking = true;
                trackingRoutine = StartCoroutine(TrackingRoutine());

            }
        }
    }
    public bool HasDeviated;
    public int DeviationIndex;
    public List<Vector3> LeaderPositionHistory = new List<Vector3>();
    // routine to provide "assistance" for tracking
    private IEnumerator TrackingRoutine()
    {
        Debug.Log("Tracking routine started");

        List<Vector3> leaderPositionHistory = new List<Vector3>();
        int deviationIndex = 0; // not using the actual position because we want this to stay fixed when an error is made
        bool hasDeviated = false;

        // tracking loop
        while (isTracking)
        {
            // get the position of the leader
            Vector3 leaderPosition;
            leaderDeviceReader.GetPosition(out leaderPosition);
            leaderPositionHistory.Add(leaderPosition);

            // get the position of the follower
            Vector3 followerPosition;
            followerDeviceReader.GetPosition(out followerPosition);

            // Determine distance between leader and follower
            float leaderFollowerDistance = CalculatePositionError(leaderPosition, followerPosition);

            // check if calculated distance is greater than threshold
            if (leaderFollowerDistance > DistanceThresholdForAssitance)
            {
                if(!hasDeviated)
                {
                    // provide assistance
                    Debug.Log("Providing assistance");
                    deviationIndex = leaderPositionHistory.Count - 1;
                    hasDeviated = true;
                }
            } else
            {
                hasDeviated = false;
            }
            HasDeviated = hasDeviated;
            DeviationIndex = deviationIndex;
            LeaderPositionHistory = leaderPositionHistory;
            // wait for the amount specified 
            yield return new WaitForSeconds(TrackingTimeFrame);
        }
    }

    // calculate some type of "error" metric
    // right now we are using Euclidean distance
    // To-Do: make more robust to handle other types of error metrics
    public float CalculatePositionError(Vector3 leaderPosition, Vector3 followerPosition)
    {
        // calculate the distance between the two
        return Vector3.Distance(leaderPosition, followerPosition);
    }

    // Gizmos for debugging
    private void OnDrawGizmos()
    {
        DrawDistanceThresholdSphere();
    }

    // Draw a sphere to show how far the follower can be from the leader before receiving assistance
    private void DrawDistanceThresholdSphere()
    {
        Vector3 leaderPosition;
        leaderDeviceReader.GetPosition(out leaderPosition);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(leaderPosition, DistanceThresholdForAssitance);
    }
}
