using UnityEngine;
using System.Collections;

public class NumberInterpolator : MonoBehaviour
{
    // Target value to interpolate to
    public float targetValue = 0.5f;

    // Duration over which to interpolate
    public float duration = 2.0f;

    // Current interpolated value
    private float currentValue = 0.0f;

    void Start()
    {
        // Start the interpolation coroutine
        StartCoroutine(InterpolateToTarget());
    }

    IEnumerator InterpolateToTarget()
    {
        // Keep track of the starting time
        float startTime = Time.time;

        // Loop over the set duration
        while (Time.time < startTime + duration)
        {
            // Calculate the time fraction (0 to 1) based on elapsed time
            float t = (Time.time - startTime) / duration;

            // Linearly interpolate the value
            currentValue = Mathf.Lerp(0, targetValue, t);

            // You can do something with currentValue here
            Debug.Log(currentValue);

            // Wait for the next frame
            yield return null;
        }

        // Ensure the final value is exactly the target value
        currentValue = targetValue;
        Debug.Log("Final Value: " + currentValue);
    }
}
