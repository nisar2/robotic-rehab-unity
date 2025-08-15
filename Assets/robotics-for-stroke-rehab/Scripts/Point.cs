//Version: 15.7.25
//added explanatory comments
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
    //Defining the visual states for the point class
    public Material WindowMaterial; //in search window
    public Material DefaultMaterial; //
    public Material NearestMaterial; // closesst to stylus
    public Material LeaderMaterial;// used as a leader in exercises

    public Segment Segment;
    public int SegmentIndex;
    public int PointIndex;
    //public Target PreviousTarget;
    //public Target NextTarget;


    public void SwitchToWindowMat()
    {
        ChangeMaterial(WindowMaterial);
    }

    public void SwitchToDefaultMat()
    {
        ChangeMaterial(DefaultMaterial);
    }

    public void SwitchToNearestMaterial()
    {
        ChangeMaterial(NearestMaterial);
    }

    public void SwitchToLeaderMaterial()
    {
        ChangeMaterial(LeaderMaterial);
    }

    private void ChangeMaterial(Material m)
    {
        Renderer r = GetComponent<Renderer>();
        Material[] mats = r.materials;
        mats[0] = m;
        r.materials = mats;
    }

    public float ComputeDistanceFrom(Vector3 pos)//returns the distance between this point and a given position like stylus
    {
        return Vector3.Distance(pos, transform.position);
    }

    public Vector3 GetPos() // returns 3D position
    {
        return transform.position;
    }
}
