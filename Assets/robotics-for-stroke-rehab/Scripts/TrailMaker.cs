using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class TrailMaker : MonoBehaviour
{
    /// <summary>
    /// The transform of the end effector. It reperesents the transformation of where the user
    /// has moved the stylus to in the Unity coordinate frame. We make it serialize field so 
    /// that we can set it in the inspector. 
    /// </summary>>
    [Tooltip("The transform of the end effector. It reperesents the transformation of where the user" +
        " has moved the stylus to in the Unity coordinate frame. We make it serialize field so " +
        "that we can set it in the inspector. ")]
    [SerializeField] private Transform EndEffectorTransform;

    /// <summary>
    /// The haptic plugin is used to turn on/off forces on the haptic device.
    /// </summary>
    [Tooltip("The haptic plugin is used to turn on/off forces on the haptic device.")]
    [SerializeField] private HapticPlugin hapticPlugin;

    /// <summary>
    /// The visualization for the targets.
    /// </summary>
    [Tooltip("The visualization for the targets.")]
    [SerializeField] private GameObject TargetPrefab;

    /// <summary>
    /// The visualization for the points in between the targets.
    /// </summary>
    [Tooltip("The visualization for the points in between the targets.")]
    [SerializeField] private GameObject PointPrefab;

    /// <summary>
    /// At any point, the user is on one particular segment. Whichever segment that is has a 
    /// rectangle highlight. This controls the visual properties like the color of that highlight.
    /// </summary>
    [SerializeField] private Material CurrentSegHighlightMaterial;

    /// <summary>
    /// At any point, the user is on one particular segment. Whichever segment that is has a 
    /// rectangle highlight. This controls how thick that rectangle is
    /// </summary>
    [Tooltip("How thick the rectangle highlights are.")]
    [Range(0f, 0.02f)]
    [SerializeField] private float CurrentSegHighlightWidth;

    /// <summary>
    /// The distance in millimeters between any two points between the targets.
    /// </summary>
    [Range(1f, 20f)]
    [SerializeField] private float PointIncrementDist;

    /// <summary>
    /// How much assistance to provide in the x direction.
    /// </summary>
    [Tooltip("How much assistance to provide in the x direction.")]
    [Range(-150.0f, 150.0f)]
    [SerializeField] private float XAssistanceScale;

    /// <summary>
    /// How much assistance to provide in the y direction.
    /// </summary>
    [Tooltip("How much assistance to provide in the y direction.")]
    [Range(-150.0f, 150.0f)]
    [SerializeField] private float YAssistanceScale;

    /// <summary>
    /// How much assistance to provide in the z direction.
    /// </summary>
    [Tooltip("How much assistance to provide in the z direction.")]
    [Range(-50.0f, 150.0f)]
    [SerializeField] private float ZAssistanceScale;

    /// <summary>
    /// How much to pull to the nearest point. Only used in method 2.
    /// </summary>
    [Tooltip("How much to pull to the nearest point. Only used in method 2.")]
    [Range(0.0f, 1f)]
    [SerializeField] private float npScale;

    /// <summary>
    /// How much importance to give to the next target
    /// </summary>
    [Range(0.0f, 10f)]
    [SerializeField] private float nextTargetScale;

    /// <summary>
    /// How far to the behihnd we search when finding the closest point
    /// </summary>
    [Tooltip("How far to the behind we search when finding the closest point")]
    [Range(1, 50)]
    [SerializeField] private int BehdindWindowSize;

    /// <summary>
    /// How far to the ahead we search when finding the closest point
    /// </summary>
    [Tooltip("How far to the ahead we search when finding the closest point")]
    [Range(1, 50)]
    [SerializeField] private int AheadWindowSize;

    /// <summary>
    /// Should we show the search window. We usually want this off but is useful for debugging purposes.
    /// </summary>
    [SerializeField] private bool ShowWindow = false;

    /// <summary>
    /// List of targets for the trailmaker activity. It gets populated by the therapist as 
    /// they add new targets.
    /// </summary>
    private List<Target> targets = new List<Target>();

    /// <summary>
    /// List of segments for the trailmaker activity. Right now, they are just used to see 
    /// if there are any unconnected targets.
    /// </summary>
    private List<Segment> Segments = new List<Segment>();

    /// <summary>
    /// List of intermediary points between targets. 
    /// </summary>
    public List<Point> Points = new List<Point>();

    /// <summary>
    /// Tracks if the assitive force is on/off. The assistive force is composed of a force 
    /// that pulls to the next target and to the nearest point on the path. 
    /// </summary>
    private bool isAssisting;

    /// <summary>
    /// The closest Point from the list of Points to the end effector.
    /// </summary>
    private Point CurrentNp;

    /// <summary>
    /// The index to access the closest point from the end effector in the Points list.
    /// </summary>
    private int CurrentNpIndex;

    /// <summary>
    /// Cache of the highlights so we can turn them on and off accordingly.
    /// </summary>
    private List<GameObject> HighlightMeshGOs = new List<GameObject>();

    public AssistanceMethods assistanceMethod;

    public UnityEvent<bool> CanClearScreen = new UnityEvent<bool>();
    public UnityEvent<bool> CanGenerateSegments = new UnityEvent<bool>();
    public UnityEvent<bool> CanClearSegments = new UnityEvent<bool>();
    public UnityEvent<bool> CanBeginTrailMaker = new UnityEvent<bool>();
    public UnityEvent<bool> OnPullToStartToggled = new UnityEvent<bool>();

    public UnityEvent OnTrailmakerRunning = new UnityEvent();
    public UnityEvent OnTrailmakerEnd = new UnityEvent();

    public UnityEvent OnAssistanceActivation = new UnityEvent();
    public UnityEvent OnAssistanceDeactivation = new UnityEvent();

    public UnityEvent<float> OnSetAssistanceLevel = new UnityEvent<float>();


    public float alpha; public float gamma;


    [Range(0.0f, 0.5f)]
    public float gravityScale;


    Coroutine assistanceRoutine;

    public List<Target> GetTargets()
    {
        return targets;
    }

    public Target GetIthTarget(int i)
    {
        return targets[i];
    }

    public bool GetIsAssisting()
    {
        return isAssisting;
    }

    public enum AssistanceMethods
    {
        Method1,
        Method2,
        Method3,
        Method4
    };
    
    public void CanGenerateSegment()
    {
        if (targets.Count > 0)
        {
            CanClearScreen.Invoke(true);
        }
        else
        {
            CanClearScreen.Invoke(false);
        }

        // if we have at least 2 targets at least one is not connected
        // (based on segments count), then we can still generate segments
        if(targets.Count >= 2 & targets.Count - 1 > Segments.Count)
        {
            CanGenerateSegments.Invoke(true); 
        } else
        {
            CanGenerateSegments.Invoke(false);
        }
        if(Segments.Count>0)
        {
            CanClearSegments.Invoke(true);
        }
        else
        {
            CanClearSegments.Invoke(false);
        }

        // as long as we have at least 2 targets and they are all connected,
        // we can begin the trail maker.
        if (shouldLoop)
        {
            if ((targets.Count >= 2 & targets.Count == Segments.Count))
            {
                CanBeginTrailMaker.Invoke(true);
            }
            else
            {
                CanBeginTrailMaker.Invoke(false);
            }
        }
        else
        {
            if ((targets.Count >= 2 & targets.Count - 1 == Segments.Count))
            {
                CanBeginTrailMaker.Invoke(true);
            }
            else
            {
                CanBeginTrailMaker.Invoke(false);
            }
        }

        if (assistanceRoutine != null)
        {
            OnTrailmakerRunning.Invoke();

        }
        else
        {
            OnTrailmakerEnd.Invoke();
            // BeginButton.SetActive(true);

        }
    }

    public void ClearScreen()
    {
        EndTrailMaker();
        if(Segments.Count > 0 )
        {
            ClearSegments();
        }
        if(targets.Count > 0)
        {
            ClearTargets();
        }
    }

    public void ClearTargets()
    {
        EndTrailMaker();
        if (targets.Count == 0) return;
        foreach (Target t in targets)
        {
            Destroy(t.gameObject);
        }
        targets.Clear();
    }

    public void ClearSegments()
    {
        EndTrailMaker();
        if (Segments.Count == 0) return;
        foreach (GameObject b in HighlightMeshGOs)
        {
            Destroy(b);
        }
        foreach (Point p in Points)
        {
            Destroy(p.gameObject);
        }
        HighlightMeshGOs.Clear();
        Points.Clear();
        Segments.Clear();
    }


    


    private void Update()
    {
        CanGenerateSegment();
        if (assistanceRoutine != null)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame & !isAssisting)
            {
                isAssisting = true;
                OnAssistanceActivation.Invoke();
            }
            else if (Keyboard.current.spaceKey.wasPressedThisFrame & isAssisting)
            {
                isAssisting = false;
                OnAssistanceDeactivation.Invoke();
            }
        }

        if (hapticPlugin.CurrentVelocity.magnitude > 10)
        {
            hapticPlugin.DisableContantForce();
        }

    }

    bool PullingToFirstTarget = false;

    public void BeginTrailmaker()
    {
        hapticPlugin.EnableConstantForce();
        // StartCoroutine(LeaderRoutine());
        assistanceRoutine = StartCoroutine(AssistanceRoutine());
    }

    public void EndTrailMaker()
    {
        hapticPlugin.DisableContantForce();
        if (assistanceRoutine != null)
        {
            isAssisting = false;
            StopCoroutine(assistanceRoutine);
            assistanceRoutine = null;
        }
        OnTrailmakerEnd.Invoke();
    }

    private IEnumerator AssistanceRoutine()
    {
        while(true)
        {
            if(shouldLoop)
            {
                SetNearestPointWithLooping();
            } else
            {
                SetNearestPointWithoutLooping();
            }
            SupplyAssistance();
            yield return new WaitForSeconds(0.01f);
        }
    }

    public bool shouldLoop;
    public void SetLooping(bool _shouldLoop)
    {
        shouldLoop = _shouldLoop;
        if(Segments.Count == 1)
        {
            shouldLoop = false;
        } 
    }

    //private IEnumerator LeaderRoutine()
    //{
    //    while(LeaderIndex < Points.Count - 1)
    //    {
    //        Points[LeaderIndex].SwitchToDefaultMat();
    //        Points[LeaderIndex + 1].SwitchToLeaderMaterial();
    //        LeaderIndex++;
    //        yield return new WaitForSeconds(DelayForLeader);
    //    }
    //}

    //private void SupplyAssistance(NearestPoint np)
    //{
    //    Vector3 npCoords = np.Coords;
    //    float npDist = np.Distance;
    //    //Debug.Log("Apply assistance " + AssistanceLevel * npDist);
        
    //    hapticPlugin.ConstForceGDir = (npCoords - EndEffectorTransform.position).normalized;
    //    hapticPlugin.ConstForceGMag = AssistanceLevel * npDist;
    //    hapticPlugin.EnableConstantForce();
    //}

    private float Sigmoid(float x)
    {
        Debug.Log("x: " + x);
        return 1 / (1 + Mathf.Exp(-20f * (x-0.06f)));    
    }

    
    private void SupplyAssistance()
    {
        DeactivateSpring();
        switch(assistanceMethod)
        {
            case AssistanceMethods.Method1:
                Vector3 npCoords = CurrentNp.GetPos();
                float npDist = CurrentNp.ComputeDistanceFrom(EndEffectorTransform.position);
                Vector3 forceDir = (npCoords - EndEffectorTransform.position).normalized;
                forceDir.x *= XAssistanceScale * npDist; forceDir.y *= YAssistanceScale * npDist; forceDir.z *= ZAssistanceScale * npDist;
                hapticPlugin.ConstForceGDir = forceDir;
                hapticPlugin.ConstForceGMag = 1f;
                hapticPlugin.EnableConstantForce();
                break;
            case AssistanceMethods.Method2:
                npDist = CurrentNp.ComputeDistanceFrom(EndEffectorTransform.position);
                int targetIndex = CurrentNp.PointIndex + 30;
                if (targetIndex >= Points.Count)
                {
                    targetIndex = Points.Count - 1;
                }

                npCoords = CurrentNp.GetPos();
                Vector3 npForceDir = npCoords - EndEffectorTransform.position;
                Vector3 targetForceDir = Points[targetIndex].GetPos() - EndEffectorTransform.position;
                alpha = Sigmoid(npDist);
                gamma = Vector3.Distance(EndEffectorTransform.position, Points[targetIndex].GetPos());
                Vector3 resultantForceDir = (alpha * npForceDir * npScale + gamma * targetForceDir * nextTargetScale).normalized;
                resultantForceDir.z *= ZAssistanceScale * npDist;
                hapticPlugin.ConstForceGDir = resultantForceDir;
                hapticPlugin.ConstForceGMag = .5f;
                break;
            case AssistanceMethods.Method3:
                Method3();
                break;
            case AssistanceMethods.Method4:
                Method4();
                break;
        }
    }

    private void Method3()
    {
        // get the closest point on path
        Vector3 npCoords = CurrentNp.GetPos();

        // get distance to that closest point
        float npDist = CurrentNp.ComputeDistanceFrom(EndEffectorTransform.position);

        // get vector from end effector to that closest point
        Vector3 f_np = (npCoords - EndEffectorTransform.position);

        // get the target point's index 
        int targetIndex = CurrentNp.PointIndex + 5;
        if (targetIndex >= Points.Count)
        {
            targetIndex = Points.Count - 1;
        }

        // get target point
        Vector3 targetCoords = Points[targetIndex].GetPos();

        // Get the vector from end effector to that target 
        Vector3 f_t = (targetCoords - EndEffectorTransform.position);

        // compute the total force and normalize b/c we only care about direction; this vector will be scaled below
        Vector3 f_total = (f_np + f_t).normalized;

        // scale by X,Y,Z assistance levels (hyperparams); scale by distance to nearest point (computed); scale by nextWpScale (hyperparam)
        f_total.x *= XAssistanceScale * npDist * nextTargetScale; f_total.y *= YAssistanceScale * npDist * nextTargetScale; f_total.z *= ZAssistanceScale * npDist * nextTargetScale;
        
        // set the haptic plugin to use the forces
        hapticPlugin.ConstForceGDir = f_total;
        hapticPlugin.ConstForceGMag = 1f;
        hapticPlugin.EnableConstantForce();
    }

    [Range(0.0f, 2.0f)]
    public float assistanceLevel;

    public float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public void SetAssistanceLevel(float level)
    {
        assistanceLevel = Remap(level, 0.0f, 1.0f, 0.0f, 2.0f);
        OnSetAssistanceLevel.Invoke(level);
    }

 

    // Duration over which to interpolate
    public float duration = 10.0f;

    // Current interpolated value
    public float currentAssistanceLevel = 0.0f;

    IEnumerator InterpolateToTarget()
    {
        // Keep track of the starting time
        float startTime = Time.time;

        // Loop over the set duration
        while (Time.time < startTime + duration)
        {
            // Calculate the time fraction (0 to 1) based on elapsed time
            float t = (Time.time - startTime) / duration;

            // Linearly interpolate the value
            currentAssistanceLevel = Mathf.Lerp(0, assistanceLevel, t);

            // You can do something with currentValue here
            Debug.Log(currentAssistanceLevel);

            // Wait for the next frame
            yield return null;
        }

        // Ensure the final value is exactly the target value
        currentAssistanceLevel = assistanceLevel;
        Debug.Log("Final Value: " + currentAssistanceLevel);
    }
    Coroutine assistanceRampRoutine = null;
    private void Method4()
    {
        if (isAssisting)
        {
            if(assistanceRampRoutine == null)
            {
                assistanceRampRoutine = StartCoroutine(InterpolateToTarget());
            }
            
            hapticPlugin.DisableContantForce();
            // get the closest point on path
            Vector3 npCoords = CurrentNp.GetPos();

            // get distance to that closest point
            float npDist = CurrentNp.ComputeDistanceFrom(EndEffectorTransform.position);

            // get vector from end effector to that closest point
            Vector3 f_np = (npCoords - EndEffectorTransform.position);

            // get the target point's index 
            int targetIndex = CurrentNp.PointIndex + 5;
            if (targetIndex >= Points.Count)
            {
                targetIndex = Points.Count - 1;
            }

            // get target point
            Vector3 targetCoords = Points[targetIndex].GetPos();

            // Get the vector from end effector to that target 
            Vector3 f_t = (targetCoords - EndEffectorTransform.position);

            Segment npSeg = CurrentNp.Segment;
            Target npSegStart = npSeg.StartTarget;
            Target npSegEnd = npSeg.EndTarget;

            Vector3 s = npSegStart.transform.position;
            Vector3 e = npSegEnd.transform.position;

            Vector3 e_minus_s = e - s;
            float theta = Mathf.PI - Mathf.Atan2(e.y - s.y, e.x - s.x); // in rads 

            // compute the total force and normalize b/c we only care about direction; this vector will be scaled below
            Vector3 f_total = (f_np + f_t).normalized;


            // scale by X,Y,Z assistance levels (hyperparams)
            // scale by distance to nearest point (computed)
            // scale by nextWpScale (hyperparam)
            f_total.x *= (XAssistanceScale * npDist * nextTargetScale);
            f_total.y *= (YAssistanceScale * npDist * nextTargetScale);
            f_total.z *= (ZAssistanceScale * npDist * nextTargetScale);


            if (0.0f <= theta & theta <= Mathf.PI)
            {
                //Debug.Log("Theta: " + theta * (180.0f / Mathf.PI) + " Sin theta: " + Mathf.Sin(theta));
                f_total += (Vector3.up * gravityScale * Mathf.Sin(theta));
            }
            else
            {
               // Debug.Log("Theta: " + theta * (180.0f / Mathf.PI) + " Cos theta: " + Mathf.Cos(theta));
                f_total += (Vector3.up * gravityScale * Mathf.Cos(theta));

            }

            // set the haptic plugin to use the forces
            hapticPlugin.ConstForceGDir = f_total;
            hapticPlugin.ConstForceGMag = currentAssistanceLevel; // 1f;
            hapticPlugin.EnableConstantForce();
        }
        else
        {
            assistanceRampRoutine = null;
            currentAssistanceLevel = 0.0f;
            hapticPlugin.DisableContantForce();
            // get the closest point on path
            Vector3 npCoords = CurrentNp.GetPos();

            // get distance to that closest point
            float npDist = CurrentNp.ComputeDistanceFrom(EndEffectorTransform.position);

            // get vector from end effector to that closest point
            Vector3 f_np = (npCoords - EndEffectorTransform.position);

            // get the target point's index 
            int targetIndex = CurrentNp.PointIndex + 5;
            if (targetIndex >= Points.Count)
            {
                targetIndex = Points.Count - 1;
            }

            // get target point
            Vector3 targetCoords = Points[targetIndex].GetPos();

            // Get the vector from end effector to that target 
            Vector3 f_t = (targetCoords - EndEffectorTransform.position);

            Segment npSeg = CurrentNp.Segment;
            Target npSegStart = npSeg.StartTarget;
            Target npSegEnd = npSeg.EndTarget;
            Vector3 segVec = (npSegEnd.transform.position - npSegStart.transform.transform.position).normalized;
            Vector3 vertEnd = npSegStart.transform.position;
            vertEnd.y += 1.0f;
            Vector3 vertVec = (vertEnd - npSegStart.transform.position).normalized;
            float alignment = Vector3.Dot(segVec, vertVec);
            alignment = Mathf.Abs(alignment);
            Debug.Log(alignment);

            // compute the total force and normalize b/c we only care about direction; this vector will be scaled below
            Vector3 f_total = (f_np + f_t).normalized;


            // scale by X,Y,Z assistance levels (hyperparams); scale by distance to nearest point (computed); scale by nextWpScale (hyperparam)
            f_total.x *= XAssistanceScale * npDist * 0.00001f; f_total.y *= YAssistanceScale * npDist * 0.00001f; f_total.z *= ZAssistanceScale * npDist * nextTargetScale;
            //alignment *= gravityScale;
            //f_total += Vector3.up * alignment;

            // set the haptic plugin to use the forces
            hapticPlugin.ConstForceGDir = f_total;
            hapticPlugin.ConstForceGMag = 1f;
            hapticPlugin.EnableConstantForce();
        }
    }
    private void StopAssitance()
    {
        hapticPlugin.DisableContantForce();
    }

    private void DrawSurface(Vector3 corner1, Vector3 corner2, Vector3 corner3, Vector3 corner4)
    {
        // Create a new mesh
        Mesh mesh = new Mesh();

        // Define vertices
        Vector3[] vertices = new Vector3[4];
        vertices[0] = corner1;
        vertices[1] = corner2;
        vertices[2] = corner3;
        vertices[3] = corner4;

        // Define triangles
        int[] triangles = new int[6];
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        triangles[3] = 2;
        triangles[4] = 1;
        triangles[5] = 3;

        // Define normals (for lighting)
        Vector3[] normals = new Vector3[4];
        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = Vector3.up;
        }

        // Define UVs (for textures)
        Vector2[] uv = new Vector2[4];
        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(0, 1);
        uv[3] = new Vector2(1, 1);

        // Assign the vertices, triangles, normals, and UVs to the mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uv;

        // Create a new GameObject and add components
        GameObject planeObject = new GameObject("Plane");
        MeshFilter meshFilter = planeObject.AddComponent<MeshFilter>();
        MeshRenderer renderer = planeObject.AddComponent<MeshRenderer>();

        // Assign the created mesh to the MeshFilter
        meshFilter.mesh = mesh;

        // You may want to assign a material to the MeshRenderer here
        renderer.material = CurrentSegHighlightMaterial;

        // Optionally, you can set the position and rotation of the GameObject
        planeObject.transform.position = Vector3.zero;
        planeObject.transform.rotation = Quaternion.identity;
        planeObject.SetActive(false);
        HighlightMeshGOs.Add(planeObject);
    }

    private void DrawBoundary(Vector3 startPos, Vector3 endPos)
    {
        float z = startPos.z;
        float x1 = startPos.x; float y1 = startPos.y; float x2 = endPos.x; float y2 = endPos.y;
        float theta = (Mathf.PI / 2) - Mathf.Atan2(y2 - y1, x2 - x1);
        theta = -theta;
        
        float B1X1 = CurrentSegHighlightWidth * Mathf.Cos(theta) + x1; float B1Y1 = CurrentSegHighlightWidth * Mathf.Sin(theta) + y1;
        Vector3 B11 = new Vector3(B1X1, B1Y1, z);
        //GameObject point11 = Instantiate(BoundaryPrefab);
        //point11.transform.position = B11;
        //point11.transform.parent = transform;

        float B1X2 = CurrentSegHighlightWidth * Mathf.Cos(theta) + x2; float B1Y2 = CurrentSegHighlightWidth * Mathf.Sin(theta) + y2;
        Vector3 B12 = new Vector3(B1X2, B1Y2, z);
        //GameObject point12 = Instantiate(BoundaryPrefab);
        //point12.transform.position = B12;
        //point12.transform.parent = transform;

        float B2X1 = -CurrentSegHighlightWidth * Mathf.Cos(theta) + x1; float B2Y1 = -CurrentSegHighlightWidth * Mathf.Sin(theta) + y1;
        Vector3 B21 = new Vector3(B2X1, B2Y1, z);
        //GameObject point21 = Instantiate(BoundaryPrefab);
        //point21.transform.position = B21;
        //point21.transform.parent = transform;

        float B2X2 = -CurrentSegHighlightWidth * Mathf.Cos(theta) + x2; float B2Y2 = -CurrentSegHighlightWidth * Mathf.Sin(theta) + y2;
        Vector3 B22 = new Vector3(B2X2, B2Y2, z);
        //GameObject point22 = Instantiate(BoundaryPrefab);
        //point22.transform.position = B22;
        //point22.transform.parent = transform;
        DrawSurface(B11, B12, B21, B22);
    }

    public int GeneratePath(int prevTargetIdx, int nextTargetIdx, int startingCount)
    {
        
        // get the current and next indices
        //int prevTargetIdx = i;
        //int nextTargetIdx = i + 1;

        Target prevTarget = targets[prevTargetIdx];
        Target nextTarget = targets[nextTargetIdx];

        // get the current and next target positions
        Vector3 currentTargetTransform = prevTarget.transform.position;
        Vector3 nextTargetTransform = nextTarget.transform.position;
      

        // compute distances between current and next pos
        float pathDistance = Vector3.Distance(currentTargetTransform, nextTargetTransform);

        // determine the number of points between two targets
        int numSampledPoints = (int)(pathDistance / (PointIncrementDist / 1000.0f)) + 1;


        Segment seg = new Segment();
        seg.StartTarget = prevTarget;
        seg.EndTarget = nextTarget;

        DrawBoundary(currentTargetTransform, nextTargetTransform);
        int pointCount = startingCount;
        

        // loop over the number of sample points
        for (int j = 0; j <= numSampledPoints; j++)
        {
            // get percentage
            float t = (float)j / (float)numSampledPoints;

            // get the interpolated position
            Vector3 interpolatedPos = Vector3.Lerp(currentTargetTransform, nextTargetTransform, t);

            // create point obj
            GameObject point = Instantiate(PointPrefab);

            // position, parent, name the point obj
            point.transform.position = interpolatedPos;
            point.transform.parent = transform;
            point.gameObject.name = "Path " + prevTargetIdx.ToString() + " point " + j.ToString();

            // add Point component to the object
            Point pointComponent = point.gameObject.GetComponent<Point>();

            pointComponent.Segment = seg;
            pointComponent.SegmentIndex = prevTargetIdx;
            pointComponent.PointIndex = pointCount;
            Points.Add(pointComponent);
            seg.points.Add(pointComponent);
            pointCount++;
        }
        Segments.Add(seg);
        return pointCount;
    }

    public void GeneratePaths()
    {
        ClearSegments();
        int pointCount = 0;
        // loop over all targets   
        for (int i = 0; i < targets.Count - 1; i++)
        {
            Debug.Log("starting count: " + pointCount);
            int pathPtCount = GeneratePath(i, i + 1, pointCount);
            Debug.Log("adding to count: " + pathPtCount);
            pointCount = pathPtCount;
        }
        Debug.Log("starting count: " + pointCount);
        if (shouldLoop)
        {
            GeneratePath(targets.Count - 1, 0, pointCount);
        }

    }

    private void SetNearestPointWithoutLooping()
    {
        foreach(GameObject b in HighlightMeshGOs)
        {
            b.SetActive(false);
        }
        float smallestDistance = 100000f;
        int smallestIndex = 0;

        if (ShowWindow)
        {
            SwitchAllPointsToDefault();
        }
        int windowStart = CurrentNpIndex - BehdindWindowSize >= 0 ? CurrentNpIndex - BehdindWindowSize : 0;
        int windowEnd = CurrentNpIndex + AheadWindowSize < Points.Count ? CurrentNpIndex + AheadWindowSize : Points.Count-1;
        for (int i = windowStart; i <= windowEnd; i++)
        {
            Point ithPoint = Points[i];
            if (ShowWindow) ithPoint.SwitchToWindowMat();
            Vector3 ithPointPos = ithPoint.transform.position;
            float ithDistance = Vector3.Distance(ithPointPos, EndEffectorTransform.position);
            if (ithDistance < smallestDistance)
            {
                smallestIndex = i;
                smallestDistance = ithDistance;
            }
        }
        if (ShowWindow)
        {
            Points[smallestIndex].SwitchToNearestMaterial();
        }
        CurrentNp = Points[smallestIndex];
        CurrentNpIndex = smallestIndex;
        HighlightMeshGOs[CurrentNp.SegmentIndex].SetActive(true);
    }
    public List<Point> window = new List<Point>();
    private void SetNearestPointWithLooping()
    {
        window.Clear();
        if(CurrentNp == null)
        {
            CurrentNp = Points[0];
        }
        
        foreach (GameObject b in HighlightMeshGOs)
        {
            b.SetActive(false);
        }
        float smallestDistance = 100000f;
        int smallestIndex = 0;

        if (ShowWindow)
        {
            SwitchAllPointsToDefault();
        }

        int windowStart = CurrentNpIndex - BehdindWindowSize >= 0 ? CurrentNpIndex - BehdindWindowSize : 0;
        int windowEnd = CurrentNpIndex + AheadWindowSize;
        Debug.Log("window start: " + windowStart.ToString());
        Debug.Log("window end: " + windowEnd.ToString());
        Debug.Log(CurrentNp);
        //Debug.Log("condition: " + (CurrentNp.PointIndex + AheadWindowSize).ToString() + " " + (Points.Count - 1).ToString());
        if (CurrentNp.PointIndex + AheadWindowSize >= Points.Count)
        {
            //Debug.Log("Wrap around");
            for (int i = windowStart; i <= Points.Count - 1; i++)
            {
                window.Add(Points[i]);
            }

            int totalWindowSize = AheadWindowSize + BehdindWindowSize;
            int remainingWindowSize = totalWindowSize - window.Count;
            for (int i=0; i<=remainingWindowSize; i++)
            {
                window.Add(Points[i]);
            }
        } else
        {
            //Debug.Log("Continuous");
            for (int i = windowStart; i <= windowEnd; i++)
            {
                window.Add(Points[i]);
            }
        }
        for (int i = 0; i < window.Count; i++)
        {
            Point ithPoint = window[i];
            if (ShowWindow) ithPoint.SwitchToWindowMat();
            Vector3 ithPointPos = ithPoint.transform.position;
            float ithDistance = Vector3.Distance(ithPointPos, EndEffectorTransform.position);
            if (ithDistance < smallestDistance)
            {
                smallestIndex = window[i].PointIndex;
                smallestDistance = ithDistance;
            }
        }
        //Debug.Log("smallest index: " + smallestIndex.ToString());
        if (ShowWindow)
        {
            // Points[smallestIndex].SwitchToNearestMaterial();
        }
        CurrentNp = Points[smallestIndex];
        CurrentNp.PointIndex = smallestIndex;
        CurrentNpIndex = smallestIndex;
        HighlightMeshGOs[CurrentNp.SegmentIndex].SetActive(true);



        //int windowStart = CurrentNpIndex - BehdindWindowSize >= 0 ? CurrentNpIndex - BehdindWindowSize : 0;
        //int windowEnd = CurrentNpIndex + AheadWindowSize < Points.Count ? CurrentNpIndex + AheadWindowSize : Points.Count - 1;
        //for (int i = windowStart; i <= windowEnd; i++)
        //{
        //    Point ithPoint = Points[i];
        //    if (ShowWindow) ithPoint.SwitchToWindowMat();
        //    Vector3 ithPointPos = ithPoint.transform.position;
        //    float ithDistance = Vector3.Distance(ithPointPos, EndEffectorTransform.position);
        //    if (ithDistance < smallestDistance)
        //    {
        //        smallestIndex = i;
        //        smallestDistance = ithDistance;
        //    }
        //}
        //if (ShowWindow)
        //{
        //    Points[smallestIndex].SwitchToNearestMaterial();
        //}
        //CurrentNp = Points[smallestIndex];
        //CurrentNpIndex = smallestIndex;
        //HighlightMeshGOs[CurrentNp.SegmentIndex].SetActive(true);
    }

    private void SwitchAllPointsToDefault()
    {
        //foreach (Point p in Points)
        //{
        //    p.SwitchToDefaultMat();
        //}
        for (int i = 0; i < Points.Count; i++)
        {
            // if (i == LeaderIndex) continue;
            // Debug.Log(Points[i]);
            Points[i].SwitchToDefaultMat();
        }
    }


    private void CheckAndProvideAssistance(Point np)
    {
        //if (ManualAssist)
        //{
        //    if (Keyboard.current.enterKey.wasPressedThisFrame)
        //    {
        //        IsAssiting = !IsAssiting;
        //    }

        //    if (IsAssiting)
        //    {
        //        SupplyAssistance(np);
        //    }
        //    else
        //    {
        //        StopAssitance();
        //    }
        //}
        //else
        //{
        //    IsAssiting = np.Distance > DistanceThresh;
        //    if (IsAssiting)
        //    {
        //        SupplyAssistance(np);
        //    }
        //    else
        //    {
        //        StopAssitance();
        //    }
        //}
    }


    public void AddTarget()
    {
        Debug.Log("ADDING TARGET");
        GameObject targetGo = null;
        if(targets.Count == 0) {
            targetGo = Instantiate(TargetPrefab, transform);
            targetGo.transform.localPosition = Vector3.zero;
            targetGo.transform.rotation = Quaternion.identity;
        } else
        {
            Vector3 prevTargetPos = targets[targets.Count - 1].transform.position;
            Vector3 thisTargetPos = prevTargetPos;
            thisTargetPos.x -= 0.01f;
            targetGo = Instantiate(TargetPrefab, thisTargetPos, Quaternion.identity, transform);
        }
        
        Target targetComp = targetGo.GetComponent<Target>();
        targets.Add(targetComp);
        targetComp.UpdateTargetLabelText(targets.Count.ToString());

        ClickAndDrag clickAndDrag = targetGo.GetComponent<ClickAndDrag>();
        clickAndDrag.OnDrag.AddListener(UpdatePoints);
    }

    public void UpdatePoints()
    {
        if(Points.Count != 0)
        {
            ClearSegments();
            GeneratePaths();
        }
    }

    public void PullToStart()
    {
        hapticPlugin.DisableContantForce();
        if (assistanceRoutine != null)
        {
            isAssisting = false;
            StopCoroutine(assistanceRoutine);
            assistanceRoutine = null;
        }

        hapticPlugin.SpringAnchorObj = targets[0].transform.gameObject;
        hapticPlugin.SpringGMag = 0.1f;
        hapticPlugin.EnableSpring();
        OnPullToStartToggled.Invoke(false);
    }

    public void DeactivateSpring()
    {
        PullingToFirstTarget = false;
        hapticPlugin.DisableSpring();
        OnPullToStartToggled.Invoke(true);
    }
    private string TrajFileName;

    public void SetTrajFileName(string filename)
    {
        TrajFileName = filename;
    }

    public void SaveTrajectory()
    {
        TrajectoryData trajData = new TrajectoryData();
        foreach(Target t in targets)
        {
            trajData.Targets.Add(new TargetData(t.transform.position));
        }
        trajData.Looping = shouldLoop;

        TrajectorySaver st = new TrajectorySaver();
        st.SaveTrajectory(trajData, TrajFileName);
    }

    public void LoadTrajectory()
    {
        ClearScreen();
        TrajectorySaver st = new TrajectorySaver();
        TrajectoryData trajData = st.LoadTrajectory(TrajFileName);

        foreach (TargetData tToLoad in trajData.Targets)
        {
            AddTarget();
            Target tAdded = targets[targets.Count - 1];
            Vector3 tToLoadPos = new Vector3(tToLoad.PosX, tToLoad.PosY, tToLoad.PosZ);
            tAdded.transform.position = tToLoadPos;
        }
        shouldLoop = trajData.Looping;
        GeneratePaths();
    }
}