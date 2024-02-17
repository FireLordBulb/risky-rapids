using System;
using System.Collections.Generic;
using Dreamteck.Splines;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using Vector3 = UnityEngine.Vector3;

public class NPCBoatController : MonoBehaviour
{
    [SerializeField] private Transform bowSemicircleCenter;
    // The npc spawns the finish line at the end of the spline.
    // Bit weird solution but works very nicely since RaftController has to know the full river spline anyway.
    // Will rework after beta when spline init is move out of the npc
    [SerializeField] private FinishLine finishLinePrefab;
    [SerializeField] private SplineComputer[] splines;
    [Header("Continuous forces")]
    [SerializeField] private float forceScalar;
    [SerializeField] private float gravity;
    [SerializeField] private float buoyancy;
    [SerializeField] private float waterFlowForce;
    [SerializeField] private float baseRowForce;
    [SerializeField] private float sideForceScalar;
    [SerializeField] private float backwardsForceScalar;
    [Header("Continuous torques")]
    [SerializeField] private float torqueScalar;
    [SerializeField] private float waterTorque;
    [SerializeField] private float rowTorque;
    [Header("Instant Forces")]
    [SerializeField] private float riverbankSideForce;
    [SerializeField] private float riverbankBackForce;
    [Header("River shape values")]
    [SerializeField] private float riverRadius;
    [SerializeField] private float waterSurfaceMargin;
    
    public float WaterFlowForce => waterFlowForce;
    public float BaseRowForce => baseRowForce;
    public Vector3 RiverNormal {get; private set;}
    public Vector3 RiverStartForward {get; private set;}

    private const int Left = -1, Right = +1;
    private const int RowingBackwards = -1, RowingForwards = +1;
    private Rigidbody rb;
    private SplinePoint[] splinePoints;
    private Vector3[] splineVectors;
    private SplinePoint closestSplinePoint;
    private Vector3 localFlowDirection;
    private int leftOar = 0;
    private int rightOar = 0;
    private float distanceBetweenObjects;
    private GameObject player;
    private void Awake()
    {
        SetStartingRotation(transform.rotation);
    }
    public void SetStartingRotation(Quaternion rotation)
    {
        RiverNormal = rotation*Vector3.up;
        RiverStartForward = rotation*Vector3.forward;
    }

    public void SetMovementForces(float newWaterForce, float newRowForce)
    {
        waterFlowForce = newWaterForce;
        baseRowForce = newRowForce;
    }

    private void Start()
    {
        rb = GetComponentInChildren<Rigidbody>();
        if (splines.Length == 0)
        {
            splines = new []{FindObjectOfType<SplineComputer>()};
        }
        if (splines[0] != null){
            InitializeSpline();
            SpawnFinishLine();
        }
    }
    private void InitializeSpline()
    {
        List<SplinePoint> pointList = new List<SplinePoint>();
        foreach (SplineComputer spline in splines)
        {
            pointList.AddRange(spline.GetPoints());
        }
        splinePoints = pointList.ToArray();
        splineVectors = new Vector3[splinePoints.Length-1];
        for (int i = 0; i < splineVectors.Length; i++){
            splineVectors[i] = splinePoints[i+1].position-splinePoints[i].position;
        }
    }
    private void SpawnFinishLine()
    {
        Instantiate(finishLinePrefab, splinePoints[^1].position, Quaternion.LookRotation(splineVectors[^1]), transform.parent);
    }
    private void FixedUpdate()
    {
        if (GameManager.Instance.CurrentGameState != GameStates.Playing)
        {
            return;
        }
        if (splinePoints != null)
        {
            CalculateSplinePosition();
            AddContinuousForce(waterFlowForce * localFlowDirection);
            MakeFall(out bool isFalling);
            if (!isFalling)
            {
                float angleDifference = Mathf.Deg2Rad * Vector3.SignedAngle(transform.forward, localFlowDirection, RiverNormal);
                AddContinuousTorque(Mathf.Sin(angleDifference) * waterTorque);
            }
            CheckRiverbankCollision();
        }
        AIScan();
    }
    private void CalculateSplinePosition()
    {
        float minDistance = float.MaxValue;
        int closestPointIndex = -1;
        for (int i = 0; i < splinePoints.Length; i++)
        {
            float distance = Vector3.Distance(ProjectOnRiverSurface(splinePoints[i].position), ProjectOnRiverSurface(transform.position));
            if (minDistance > distance)
            {
                minDistance = distance;
                closestPointIndex = i;
            } else
            {
                break;
            }
        }
        Vector3 splinePointPosition = splinePoints[closestPointIndex].position;
        int currentVectorIndex = closestPointIndex;
        bool isBeforePoint = Vector3.Dot(rb.velocity, splinePointPosition-transform.position) > 0;
        if (isBeforePoint){
            currentVectorIndex--;
        }
        bool isOnRiver = 0 <= currentVectorIndex && currentVectorIndex < splineVectors.Length;
        closestSplinePoint = splinePoints[Math.Clamp(currentVectorIndex, 0, splinePoints.Length-1)];
        localFlowDirection = isOnRiver ? splineVectors[currentVectorIndex].normalized : Vector3.zero;
    }
    private void CheckRiverbankCollision()
    {
        if (localFlowDirection == Vector3.zero)
        {
            return;
        }
        Vector3 perpendicular = Vector3.Cross(localFlowDirection, RiverNormal).normalized;
        Vector3 localNormal = Vector3.Cross(perpendicular, localFlowDirection);
        
        Vector3 fromPointToBoat = Vector3.ProjectOnPlane(bowSemicircleCenter.position-closestSplinePoint.position, localNormal);
        Vector3 projectedOntoSpline = Vector3.Project(fromPointToBoat, localFlowDirection);
        Vector3 shortestToSpline = fromPointToBoat-projectedOntoSpline;
        if (riverRadius < shortestToSpline.magnitude)
        {
            Vector3 sideForce = riverbankSideForce * shortestToSpline.normalized;
            Vector3 backForce = riverbankBackForce * localFlowDirection;
            rb.AddForce(sideForce+backForce, ForceMode.Acceleration);
        }
    }
    private void MakeFall(out bool isFalling)
    {
        isFalling = false;
        if (localFlowDirection == Vector3.zero)
        {
            return;
        }
        Vector3 fromPointToBoat = transform.position-closestSplinePoint.position;
        Vector3 heightOverRiverPlane = GetDifferenceToPlane(fromPointToBoat);
        if (heightOverRiverPlane.magnitude < waterSurfaceMargin)
        {
            return;
        }
        bool isAboveSurface = 0 < Vector3.Dot(RiverNormal, heightOverRiverPlane);
        bool localFlowIsFlat = GetDifferenceToPlane(localFlowDirection).magnitude < waterSurfaceMargin;
        if (!isAboveSurface && localFlowIsFlat)
        {
            AddContinuousForce(buoyancy * RiverNormal);
        } else
        {
            // Gravity is the only force that uses Vector3.up instead of riverNormal, since the slope of the
            // river won't change the direction of Earth's gravity.
            AddContinuousForce(gravity * Vector3.up);
            isFalling = true;
        }
    }
    private Vector3 GetDifferenceToPlane(Vector3 vector)
    {
       return vector-ProjectOnRiverSurface(vector);
    }
    private Vector3 ProjectOnRiverSurface(Vector3 vector)
    {
        return Vector3.ProjectOnPlane(vector, RiverNormal);
    }
    private void AIScan()
    {
        float radius = 10;
        float centerMargin = 10;

        GameObject currentObject;
        List<GameObject> nearbyObjects = new List<GameObject>();

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        foreach (var hitCollider in hitColliders)
        {
            if (CheckDistance(gameObject, hitCollider.gameObject) < 5)
            {
                continue;
            }
            if (hitCollider.gameObject == gameObject || (hitCollider.transform.parent && hitCollider.transform.parent.gameObject == gameObject))
            {
                continue;
            }
            currentObject = (hitCollider.name == "Physics Hitbox") ? hitCollider.transform.parent.gameObject : hitCollider.gameObject;
            if (currentObject.transform.position.z > transform.position.z - centerMargin)
            {
                nearbyObjects.Add(currentObject);
            }
        }

        MakeDecision(nearbyObjects);
    }
    private void MakeDecision(List<GameObject> nearbyObjects)
    {
        leftOar = 0;
        rightOar = 0;

        float width;
        MeshRenderer[] meshRenderers;

        foreach (GameObject obj in nearbyObjects)
        {
            Vector3 NPCPos = transform.position;
            Vector3 objPos = obj.transform.position;

            if (obj.tag == "Player")
            {
                //if (obj.transform.position.x > )
                //rightOar = -1;
            }
            if (obj.name == "Obstacle Template (8)")
            {
                meshRenderers = obj.GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer thisMeshRenderer in meshRenderers)
                {
                    thisMeshRenderer.material.color = Color.red;
                    width = thisMeshRenderer.bounds.size[2];
                    if (objPos.x > NPCPos.x)
                    {
                        Debug.Log("left");
                        leftOar = -1;
                    } else {
                        Debug.Log("right "+NPCPos.z+" / "+objPos.z);
                        rightOar = -1;
                    }
                }
            }
        }

        HandleRowing();
    }
    private float CheckDistance(GameObject obj1, GameObject obj2)
    {
        return Vector3.Distance(obj1.transform.position, obj2.transform.position);
    }
    private void HandleRowing()
    {
        bool rowingIsPushingLeft = leftOar == RowingBackwards || rightOar == RowingForwards;
        bool rowingIsPushingRight = rightOar == RowingBackwards || leftOar == RowingForwards;
        if (rowingIsPushingLeft && rowingIsPushingRight)
        {
            Vector3 rowForce = baseRowForce * transform.forward;
            if (leftOar == RowingBackwards || rightOar == RowingBackwards)
            {
                rowForce *= backwardsForceScalar;
            }
            AddContinuousForce(rowForce);
        }
        else if (rowingIsPushingLeft)
        {
            PushBoat(Left);
        }
        else if (rowingIsPushingRight)
        {
            PushBoat(Right);
        }
    }
    private void PushBoat(int side)
    {
        AddContinuousTorque(side * rowTorque);
        Vector3 direction = side * Vector3.Cross(transform.up, transform.forward);
        Vector3 rowForce =  baseRowForce * sideForceScalar * direction;
        AddContinuousForce(rowForce);
    }
    private void AddContinuousTorque(float torque)
    {
        rb.AddTorque(torque * torqueScalar * Time.fixedDeltaTime * RiverNormal, ForceMode.Acceleration);
    }
    private void AddContinuousForce(Vector3 rowForce)
    {
        rb.AddForce(forceScalar * Time.fixedDeltaTime * rowForce, ForceMode.Acceleration);
    }
}
