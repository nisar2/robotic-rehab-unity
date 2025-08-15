using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityComp : MonoBehaviour
{
    public Transform start;
    public Transform end;

    private void Update()
    {
        Vector3 s = start.position;
        Vector3 e = end.position;

        Vector3 e_minus_s = e - s;
        float theta = Mathf.PI - Mathf.Atan2(e.y - s.y, e.x - s.x); // in rads 

        Debug.Log("Theta: " + theta * (180.0f / Mathf.PI));
    }
}
