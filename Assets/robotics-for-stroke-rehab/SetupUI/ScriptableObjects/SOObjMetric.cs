using UnityEngine;

[CreateAssetMenu(fileName = "SOObjMetric", menuName = "Participant Management/SOString")]
public class SOObjMetric : ScriptableObject
{
    public ObjectiveMetrics Data;

    public bool ShouldReset = true;

    private void OnEnable()
    {
        if (ShouldReset)
        {
            //Data = "";
        }
    }
}