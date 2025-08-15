using UnityEngine;

[System.Serializable]
public class TargetData
{
    public float PosX;
    public float PosY;
    public float PosZ;

    public TargetData(Vector3 pos)
    {
        SetValues(pos);
    }

    public void SetValues(Vector3 pos)
    {
        PosX = pos.x;
        PosY = pos.y;
        PosZ = pos.z;
    }
}
