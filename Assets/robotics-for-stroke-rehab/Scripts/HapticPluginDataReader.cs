using UnityEngine;

// nothing really impressive here. just converting the output from the device information to Vector3
// may not even be needed, but Vector3 gives us a lot of functions like normalization and distances  for free...
public class HapticPluginDataReader : MonoBehaviour
{
    // haptic plugin for the device you want to read from
    [SerializeField] private HapticPlugin hapticPlugin;

    // data that will be read
    [SerializeField] private Vector3 jointAngles = new Vector3();
    [SerializeField] private Vector3 gimbalAngles = new Vector3();
    [SerializeField] private Vector3 position = new Vector3(); // this will be in meters!

    

    // just for updating the values so we can see them in inspector
    private void FixedUpdate()
    {
        GetJointAndGimbalAngles(out jointAngles, out gimbalAngles);
        GetPosition(out position);
    }

    // read joint and gimbal angles from plugin (double[]) and convert them to  Vector3s
    public void GetJointAndGimbalAngles(out Vector3 jointAngles, out Vector3 gimbalAngles)
    {
        double[] tempJointAngles = new double[3];
        double[] tempGibalAngles = new double[3];

        HapticPlugin.getJointAngles(hapticPlugin.DeviceIdentifier, tempJointAngles, tempGibalAngles);

        Vector3 _jointAngles = new Vector3();
        _jointAngles[0] = (float)tempJointAngles[0];
        _jointAngles[1] = (float)tempJointAngles[1];
        _jointAngles[2] = (float)tempJointAngles[2];
        jointAngles = _jointAngles;

        Vector3 _gimbalAngles = new Vector3();
        _gimbalAngles[0] = (float)tempGibalAngles[0];
        _gimbalAngles[1] = (float)tempGibalAngles[1];
        _gimbalAngles[2] = (float)tempGibalAngles[2];
        gimbalAngles = _gimbalAngles;
    }

    // read position from plugin (double[]) and convert it to  Vector3
    public void GetPosition(out Vector3 position)
    {
        double[] tempPositionArray = new double[3];
        HapticPlugin.getPosition(hapticPlugin.DeviceIdentifier, tempPositionArray); // mm

        Vector3 _position = new Vector3();
        _position[0] = (float)tempPositionArray[0] / 1000f; // converting from mm to m 
        _position[1] = (float)tempPositionArray[1] / 1000f;
        _position[2] = (float)tempPositionArray[2] / 1000f;
        position = _position;
    }
}
