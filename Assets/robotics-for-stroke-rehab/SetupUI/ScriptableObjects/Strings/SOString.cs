using UnityEngine;

[CreateAssetMenu(fileName = "SOString", menuName = "Participant Management/SOString")]
public class SOString : ScriptableObject
{
    public string Data;

    public bool ShouldReset = true;

    private void OnEnable()
    {
        if (ShouldReset)
        {
            Data = "";
        }
    }
}