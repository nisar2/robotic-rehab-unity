using System.Collections.Generic;

[System.Serializable]
public class TrajectoryData
{
    public List<TargetData> Targets = new List<TargetData>();
    public bool Looping;
}
