using System;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class BoatPhysics : MonoBehaviour
{
    private const float MaxDepth = -1.5f;
    [SerializeField] private Transform bowSemicircleCenter;
    [SerializeField] private PhysicsData physicsData;
    [Header("River shape values")]
    [SerializeField] private float riverRadius;
    [SerializeField] private float waterSurfaceMargin;
    private new Rigidbody rigidbody;
    // Continuous forces
    private float forceScalar;
    private float gravity;
    private float buoyancy;
    private float minBuoyancyScalar;
    private float waterFlowForce;
    private float baseRowForce;
    private float singleOarSideScalar;
    private float singleOarForwardsScalar;
    private float twoOarsBackwardsScalar;
    // Continuous torques
    private float torqueScalar;
    private float waterTorque;
    private float rowTorque;
    private float normalChangeTorque;
    // Riverbank values
    private float riverbankSideForce;
    private float riverbankBackForce;
    private float riverbankMinScalar;
    // Spline fields
    private Vector3[] splinePositions;
    private Vector3[] splineDirections;
    private Vector3 localSplinePosition;
    private Vector3 localFlowDirection;
    private Vector3 localNormal;
    private Vector3 nextSplinePosition;
    // Update loop booleans
    private bool hasBeenKnockedBack;
    public bool IsFalling { get; private set; }
    // Speeds
    public float TopSpeed {get; private set;}
    public float Speed => rigidbody.velocity.magnitude;
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        
        rigidbody.drag = physicsData.linearDrag;
        forceScalar = physicsData.forceScalar;
        gravity = physicsData.gravity;
        buoyancy = physicsData.buoyancy;
        minBuoyancyScalar = physicsData.minBuoyancyScalar;
        waterFlowForce = physicsData.waterFlowForce;
        baseRowForce = physicsData.baseRowForce;
        singleOarSideScalar = physicsData.singleOarSideScalar;
        singleOarForwardsScalar = physicsData.singleOarForwardsScalar;
        twoOarsBackwardsScalar = physicsData.twoOarsBackwardsScalar;
        rigidbody.angularDrag = physicsData.angularDrag;
        torqueScalar = physicsData.torqueScalar;
        waterTorque = physicsData.waterTorque;
        rowTorque = physicsData.rowTorque;
        normalChangeTorque = physicsData.normalChangeTorque;
        riverbankSideForce = physicsData.riverbankSideForce;
        riverbankBackForce = physicsData.riverbankBackForce;
        riverbankMinScalar = physicsData.riverbankMinScalar;
        TopSpeed = (waterFlowForce + baseRowForce) * forceScalar / rigidbody.drag;
    }
    private void Start()
    {
        splinePositions = SplineManager.Instance.Positions;
        splineDirections = SplineManager.Instance.Directions;
    }
    public void AddMovementForces(float waterForceChange, float rowForceChange)
    {
        waterFlowForce += waterForceChange;
        baseRowForce += rowForceChange;
    }
    public void AddSpeedPaddle()
    {
        baseRowForce = physicsData.baseRowForce + UpgradeHolder.Instance.GetUpgradeValue(UpgradeType.Paddle);
    }

    public void ResetTo(Vector3 position, Quaternion rotation)
    {
        ResetLinearDrag();
        rigidbody.position = transform.position = position;
        rigidbody.rotation = transform.rotation = rotation;
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        // Resets force and torque to zero.
        rigidbody.AddForce(-rigidbody.GetAccumulatedForce());
        rigidbody.AddTorque(-rigidbody.GetAccumulatedTorque());
    }
    public void ResetMovementForces()
    {
        waterFlowForce = physicsData.waterFlowForce;
        baseRowForce = physicsData.baseRowForce + UpgradeHolder.Instance.GetUpgradeValue(UpgradeType.Paddle);
    }
    public void ScaleLinearDrag(float scale)
    {
        rigidbody.drag *= scale;
    }
    public void ResetLinearDrag()
    {
        rigidbody.drag = physicsData.linearDrag;
    }
    public void AddKnockBack(float force, Vector3 normal, float minSpeedScalar)
    {
        if (hasBeenKnockedBack)
        {
            return;
        }
        Vector3 direction = ProjectOnRiverSurface(normal).normalized;
        Vector3 velocityAlongDirection = Vector3.Project(rigidbody.velocity, direction);
        float speedScalar = Mathf.Max(velocityAlongDirection.magnitude, minSpeedScalar);
        rigidbody.AddForce(force * speedScalar * direction, ForceMode.Acceleration);
        // Nullify the river flow when being knocked back. 
        AddContinuousForce(-1 * waterFlowForce * localFlowDirection);
        hasBeenKnockedBack = true;
    }
    public void PushBoat(int side)
    {
        AddContinuousAxialTorque(side * rowTorque);
        Vector3 direction = side * Vector3.Cross(transform.up, transform.forward);
        Vector3 sideRowForce =  baseRowForce * singleOarSideScalar * direction;
        Vector3 forwardsRowForce =  baseRowForce * singleOarForwardsScalar * transform.forward;
        AddContinuousForce(sideRowForce+forwardsRowForce);
    }
    public void RowStraight(bool isRowingForwards)
    {
        Vector3 rowForce = baseRowForce * transform.forward;
        if (!isRowingForwards)
        {
            rowForce *= twoOarsBackwardsScalar;
        }
        AddContinuousForce(rowForce);
    }
    private void FixedUpdate()
    {

        if (GameManager.Instance.CurrentGameState != GameState.Playing &&
            GameManager.Instance.CurrentGameState != GameState.EndGame)
        {
            return;
        }
        hasBeenKnockedBack = false;
        CalculateSplinePosition();
        UpdateRiverNormal();
        bool isTooDeep = transform.position.y - nextSplinePosition.y < MaxDepth;
        if (isTooDeep)
        {
            // All downwards velocity is deleted.
            rigidbody.AddForce(new Vector3(0, Mathf.Max(-rigidbody.velocity.y, 0), 0), ForceMode.Impulse);
        }
        AddContinuousForce(waterFlowForce*(isTooDeep ? ProjectOnXZPlane(localFlowDirection).normalized : localFlowDirection));
        MakeFall(isTooDeep);
        if (!IsFalling)
        {
            float angleDifference = Mathf.Deg2Rad * Vector3.SignedAngle(transform.forward, localFlowDirection, localNormal);
            AddContinuousAxialTorque(Mathf.Sin(angleDifference) * waterTorque);
        }
        CheckRiverbankCollision();
        
        HandleWrongWayUI();
    }
    private void CalculateSplinePosition()
    {
        float minDistance = float.MaxValue;
        int splineIndex = -1;
        for (int i = 0; i < splinePositions.Length; i++)
        {
            float distance = ProjectOnXZPlane(splinePositions[i]-transform.position).magnitude;
            if (minDistance > distance)
            {
                minDistance = distance;
                splineIndex = i;
            }
        }
        Vector3 closestNonZeroFlowDirection = ProjectOnXZPlane(splineDirections[Math.Min(splineIndex, splineDirections.Length-2)]);
        Vector3 splinePositionToBoat = ProjectOnXZPlane(splinePositions[splineIndex]-transform.position);
        bool isBeforePoint = 0 < Vector3.Dot(closestNonZeroFlowDirection, splinePositionToBoat);
        if (isBeforePoint){
            splineIndex--;
            if (splineIndex < 0)
            {
                localFlowDirection = Vector3.zero;
                return;
            }
        }
        localSplinePosition = splinePositions[splineIndex];
        localFlowDirection = splineDirections[splineIndex];
        
        nextSplinePosition = splinePositions[Math.Min(splineIndex+1, splinePositions.Length-1)];
    }
    private void UpdateRiverNormal()
    {
        Vector3 perpendicular = Vector3.Cross(localFlowDirection, Vector3.up);
        Vector3 newLocalNormal = Vector3.Cross(perpendicular, localFlowDirection).normalized;
        localNormal = newLocalNormal == Vector3.zero ? Vector3.up : newLocalNormal;
        
        Vector3 axisScaledByError = Vector3.Cross(transform.up, localNormal);
        rigidbody.AddTorque(normalChangeTorque * torqueScalar * axisScaledByError, ForceMode.Acceleration);
    }
    private void MakeFall(bool isTooDeep)
    {
        IsFalling = false;
        Vector3 fromPointToBoat = transform.position-localSplinePosition;
        Vector3 heightOverRiverPlane = GetDifferenceToPlane(fromPointToBoat);
        if (heightOverRiverPlane.magnitude < waterSurfaceMargin)
        {
            return;
        }
        bool isAboveSurface = 0 < Vector3.Dot(localNormal, heightOverRiverPlane);
        if (isAboveSurface && !isTooDeep)
        {
            
            AddContinuousForce(gravity*Vector3.up);
            IsFalling = true;
        } else
        {
            float depthInWater = Mathf.Max(heightOverRiverPlane.magnitude, minBuoyancyScalar);
            AddContinuousForce(buoyancy*depthInWater*localNormal);
        }
    }
    private void CheckRiverbankCollision()
    {
        if (localFlowDirection == Vector3.zero)
        {
            return;
        }
        Vector3 fromPointToBoat = Vector3.ProjectOnPlane(bowSemicircleCenter.position-localSplinePosition, localNormal);
        Vector3 projectedOntoSpline = Vector3.Project(fromPointToBoat, localFlowDirection);
        Vector3 shortestToSpline = fromPointToBoat-projectedOntoSpline;
        float overlapWithWall = shortestToSpline.magnitude - riverRadius;
        if (overlapWithWall <= 0)
        {
            return;
        }
        overlapWithWall = Mathf.Max(overlapWithWall, riverbankMinScalar);
        Vector3 sideForce = riverbankSideForce * overlapWithWall * shortestToSpline.normalized;
        Vector3 backForce = riverbankBackForce * rigidbody.velocity.normalized;
        rigidbody.AddForce(sideForce + backForce, ForceMode.Acceleration);
    }
    private Vector3 GetDifferenceToPlane(Vector3 vector)
    {
       return vector-ProjectOnRiverSurface(vector);
    }
    private Vector3 ProjectOnRiverSurface(Vector3 vector)
    {
        return Vector3.ProjectOnPlane(vector, localNormal);
    }
    private Vector3 ProjectOnXZPlane(Vector3 vector)
    {
        return new Vector3(vector.x, 0, vector.z);
    }
    private void AddContinuousForce(Vector3 force)
    {
        rigidbody.AddForce(forceScalar * force, ForceMode.Acceleration);
    }
    private void AddContinuousAxialTorque(float torque)
    {
        rigidbody.AddTorque(torque * torqueScalar * transform.up, ForceMode.Acceleration);
    }
    private void HandleWrongWayUI()
    {
        bool isGoingWrongWay = Vector3.Dot(transform.forward, localFlowDirection) < 0;
        GameManager.Instance.WrongWayUISetActive(isGoingWrongWay);
    }
}
