using System.Collections.Generic;

[System.Serializable]
public class Participant
{ 
    public string ParticpantID;
    public List<SOObjectiveMetric> FMFelxorsReflexHistory = new List<SOObjectiveMetric>();
    public List<SOObjectiveMetric> FMExtensorsReflexHistory = new List<SOObjectiveMetric>();
}
