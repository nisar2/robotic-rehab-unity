using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class HapticPluginDataRecorder : MonoBehaviour
{
    [SerializeField] private HapticPlugin hapticPlugin;
    [SerializeField] private float sampleRate;
    [SerializeField] private string absoluteRootSavePath;
    [SerializeField] private string experimentName;
    [SerializeField] private string fileName;
    [SerializeField] private UnityEvent OnRecordingStarted = new UnityEvent();
    [SerializeField] private UnityEvent OnRecordingStopped = new UnityEvent();
    [SerializeField] private GameObject EndEffectorGO;
    [SerializeField] private TrailMaker trailMaker;

    private bool isRecording;

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame & !isRecording)
        {
            StartRecording();
        }
        else if (Keyboard.current.spaceKey.wasPressedThisFrame & isRecording)
        {
            StopRecording();
        }
    }

    private void Start()
    {
        Debug.Log(getAbsoluteFilePath());
    }

    private string getAbsoluteFilePath()
    {
        string absoluteFilePath = Path.Combine(Path.Combine(absoluteRootSavePath, experimentName),fileName);
        absoluteFilePath += "_" + hapticPlugin.DeviceIdentifier;
        absoluteFilePath += ".csv";
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
        string absFilePath = getAbsoluteFilePath();
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
            "Is Assisting,"
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
                + trailMaker.GetIsAssisting().ToString()
            );
            yield return new WaitForSeconds(1 / sampleRate);
        }
        Debug.Log($"Recording to {absFilePath} complete.");
        tw.Close();
    }
}
