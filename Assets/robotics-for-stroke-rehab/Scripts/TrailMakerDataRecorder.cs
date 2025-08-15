using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class TrailMakerDataRecorder : MonoBehaviour
{
    [SerializeField] private HapticPlugin hapticPlugin;
    [SerializeField] private float sampleRate;
    [SerializeField] private UnityEvent OnRecordingStarted = new UnityEvent();
    [SerializeField] private UnityEvent OnRecordingStopped = new UnityEvent();
    [SerializeField] private GameObject EndEffectorGO;
    [SerializeField] private TrailMaker trailMaker;

    private bool isRecording;

    //private void Update()
    //{
    //    if (Keyboard.current.spaceKey.wasPressedThisFrame & !isRecording)
    //    {
    //        StartRecording();
    //    }
    //    else if (Keyboard.current.spaceKey.wasPressedThisFrame & isRecording)
    //    {
    //        StopRecording();
    //    }
    //}


    private string getAbsoluteRoot()
    {
        string root = DataManager.Instance.GetSite().DataPath;
        string particpantId = DataManager.Instance.GetSessionParticipant().Data;
        string sessionId = DataManager.Instance.SessionId + "_" + particpantId;
        string absoluteFilePath = Path.Combine(Path.Combine(root, particpantId), sessionId);
        return absoluteFilePath;
    }

    private string getFileName(string fileName)
    {
        string particpantId = DataManager.Instance.GetSessionParticipant().Data;
        string sessionId = DataManager.Instance.SessionId + "_" + particpantId;
        return fileName + "_" + sessionId + ".csv";
    }

    private string getAbsoluteRobotPath()
    {
        string absoluteFilePath = Path.Combine(getAbsoluteRoot(), getFileName("robot"));
        return absoluteFilePath;
    }

    private string getAbsoluteTargetsPath()
    {
        string absoluteFilePath = Path.Combine(getAbsoluteRoot(), getFileName("targets"));
        return absoluteFilePath;
    }

    public void StartRecording()
    {
        isRecording = true;
        StartCoroutine(RecordingRoutine());
        OnRecordingStarted.Invoke();
    }

    public void StopRecording()
    {
        isRecording = false;
        OnRecordingStopped.Invoke();
    }

    IEnumerator RecordingRoutine()
    {
        string absFilePath = getAbsoluteRobotPath();
        TextWriter tw = new StreamWriter(absFilePath, false);
        tw.WriteLine(
            "Timestamp," +
            "Stylus Position X," +
            "Stylus Position Y," +
            "Stylus Position Z," +
            "Unity Stylus Position X," +
            "Unity Stylus Position Y," +
            "Unity Stylus Position Z," +
            "Joint angle 0," +
            "Joint angle 1," +
            "Joint angle 2," +
            "Gimbal angle 0," +
            "Gimbal angle 1," +
            "Gimbal angle 2," +
            "Is Assisting," +
            "Assistance Level"
        );
        Debug.Log($"Recording to {absFilePath}");
        while (isRecording)
        {
            double[] currentFramePos = new double[3];
            double[] currentFrameJointAngles = new double[3];
            double[] currentFrameGimbalAngles = new double[3];
            HapticPlugin.getPosition(hapticPlugin.DeviceIdentifier, currentFramePos);
            HapticPlugin.getJointAngles(hapticPlugin.DeviceIdentifier, currentFrameJointAngles, currentFrameGimbalAngles);
            tw.WriteLine(
                DateTime.Now.Ticks.ToString() + ","
                + currentFramePos[0].ToString() + ","
                + currentFramePos[1].ToString() + ","
                + currentFramePos[2].ToString() + ","
                + EndEffectorGO.transform.position.x.ToString() + ","
                + EndEffectorGO.transform.position.y.ToString() + ","
                + EndEffectorGO.transform.position.z.ToString() + ","
                + currentFrameJointAngles[0].ToString() + ","
                + currentFrameJointAngles[1].ToString() + ","
                + currentFrameJointAngles[2].ToString() + ","
                + currentFrameGimbalAngles[0].ToString() + ","
                + currentFrameGimbalAngles[1].ToString() + ","
                + currentFrameGimbalAngles[2].ToString() + ","
                + trailMaker.GetIsAssisting().ToString() + ","
                + (trailMaker.currentAssistanceLevel / 2.0f).ToString()
            );
            yield return new WaitForSeconds(1 / sampleRate);
        }
        Debug.Log($"Recording to {absFilePath} complete.");
        tw.Close();
    }

    public void SaveTargets()
    {
        string absFilePath = getAbsoluteTargetsPath(); // change this
        TextWriter tw = new StreamWriter(absFilePath, false);
        tw.WriteLine(
            "Name," +
            "X," +
            "Y," +
            "Z," 
        );
        for(int i =0; i<trailMaker.GetTargets().Count; i++)
        {
            Target t = trailMaker.GetIthTarget(i);
            tw.WriteLine(
                "Target " + i.ToString() + ","
                + t.transform.position.x.ToString() + ","
                + t.transform.position.y.ToString() + ","
                + t.transform.position.z.ToString() 
            );
        }
        
        Debug.Log($"Recording to {absFilePath} complete.");
        tw.Close();
    }
}
