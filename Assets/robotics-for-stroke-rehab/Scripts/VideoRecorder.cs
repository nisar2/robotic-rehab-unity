#if UNITY_EDITOR

using System.ComponentModel;
using System.IO;
using UnityEditor;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;
using static UnityEditor.VersionControl.Asset;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using UnityEngine.UIElements;

namespace UnityEngine.Recorder.Examples
{
    /// <summary>
    /// This example shows how to set up a recording session via script, for an MP4 file.
    /// To use this example, add the MultipleRecordingsExample component to a GameObject.
    ///
    /// Enter the Play Mode to start the recording.
    /// The recording automatically stops when you exit the Play Mode or when you disable the component.
    ///
    /// This script saves the recording outputs in [Project Folder]/SampleRecordings.
    /// </summary>
    public class VideoRecorder : MonoBehaviour
    {
        RecorderController m_RecorderController;
        public bool m_RecordAudio = true;
        internal MovieRecorderSettings m_Settings = null;

        public FileInfo OutputFile
        {
            get
            {
                var fileName = m_Settings.OutputFile + ".mp4";
                return new FileInfo(fileName);
            }
        }

        private string getAbsFilePath()
        {
            string root = DataManager.Instance.GetSite().DataPath;
            string particpantId = DataManager.Instance.GetSessionParticipant().Data;
            string sessionId = DataManager.Instance.SessionId + "_" + particpantId;
            string absoluteFilePath = root + '/' + particpantId + '/' + sessionId + '/' + "video_"+sessionId;
            Debug.Log("here" + absoluteFilePath);
            return absoluteFilePath; 
        }

        public void StartRecording()
        {
            var controllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
            
            m_RecorderController = new RecorderController(controllerSettings);
           
            // Video
            m_Settings = ScriptableObject.CreateInstance<MovieRecorderSettings>();
            m_Settings.name = "My Video Recorder";
            m_Settings.Enabled = true;
            
            // This example performs an MP4 recording
            m_Settings.OutputFormat = MovieRecorderSettings.VideoRecorderOutputFormat.MP4;
            m_Settings.VideoBitRateMode = VideoBitrateMode.High;

            m_Settings.ImageInputSettings = new CameraInputSettings
            {
                Source = ImageSource.TaggedCamera,
                CameraTag = "Recording Camera",
                OutputWidth = 1920,
                OutputHeight = 1080,
                FlipFinalOutput = false,
            };

            m_Settings.AudioInputSettings.PreserveAudio = m_RecordAudio;

            // Simple file name (no wildcards) so that FileInfo constructor works in OutputFile getter.
            Debug.Log("Here2" + getAbsFilePath());
            m_Settings.OutputFile = getAbsFilePath(); 
            Debug.Log(m_Settings.OutputFile);

            // Setup Recording
            controllerSettings.AddRecorderSettings(m_Settings);
            controllerSettings.SetRecordModeToManual();
            controllerSettings.FrameRate = 30.0f;
            

            RecorderOptions.VerboseMode = false;
            m_RecorderController.PrepareRecording();
            m_RecorderController.StartRecording();

            Debug.Log($"Started recording for file {OutputFile.FullName}");
        }

        public void StopRecording()
        {
            if ( m_RecorderController != null ) { m_RecorderController.StopRecording(); }
        }
    }
}

#endif