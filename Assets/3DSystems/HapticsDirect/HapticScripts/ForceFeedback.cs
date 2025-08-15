using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceFeedback : MonoBehaviour
{
    public HapticPlugin hapticPlugin;
    public float magnitude;

    public void SupplyForce(Vector3 dir)
    {
        hapticPlugin.ConstForceGDir = dir;
        hapticPlugin.ConstForceGMag = magnitude;

        hapticPlugin.EnableConstantForce();
    }

    public void StopSupplyingForce()
    {
        hapticPlugin.DisableContantForce();
    }
}
