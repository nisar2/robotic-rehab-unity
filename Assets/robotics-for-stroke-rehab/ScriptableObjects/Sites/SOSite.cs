using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SOSite", menuName = "SOSite")]
public class SOSite : ScriptableObject
{
    public string DataPath;
    public string ParticipantListFileName = "participants.json";

    public string GetAbsoluteParticipantListFilePath()
    {
        return $"{DataPath}/{ParticipantListFileName}";
    }

    public string GetAbsoluteParticipantFolderPath(string participantId)
    {
        return $"{DataPath}/{participantId}";
    }

    public string GetAbsoluteParticipantObjectiveMetricsFolderPath(string participantId)
    {
        return $"{GetAbsoluteParticipantFolderPath(participantId)}/objective-metrics";
    }

    public string GetAbsoluteParticipantSessionFolderPath(string participantId)
    {
        return $"{GetAbsoluteParticipantFolderPath(participantId)}/sessions";
    }

    public string GetAbsoluteParticipantObjectiveMetricsFilePath(string participantId)
    {
        return $"{GetAbsoluteParticipantObjectiveMetricsFolderPath(participantId)}/{participantId}_{DateTime.UtcNow.ToString("yyyy-MM-dd-hh-mm-ss")}.json";
    }
}
