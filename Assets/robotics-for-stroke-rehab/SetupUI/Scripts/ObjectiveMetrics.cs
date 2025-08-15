using System;
using System.Collections.Generic;

[System.Serializable]
public class ObjectiveMetrics 
{
    public string ParticipantId;
    public string Date = DateTime.Now.ToString();
    public Dictionary<string, float> ObjectiveMetricCollection = new Dictionary<string, float>();
    public string uuid;
}
