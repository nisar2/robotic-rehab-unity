using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoTo : MonoBehaviour
{
    public HapticPlugin plugin;

    public GameObject target;

    public void Start()
    {
        TakeEndEffectorTo(target);
    }

    /// <summary>
    /// Takes the end effector of the robot controlled by the plugin 
    /// referenced in this script to the target.
    /// </summary>
    /// <param name="target"></param>
    public void TakeEndEffectorTo(GameObject target)
    {
        plugin.SpringAnchorObj = target;
        plugin.SpringGMag = 0.1f;
        plugin.EnableSpring();
    }
}
