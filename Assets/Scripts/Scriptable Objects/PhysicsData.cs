using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Boat Physics Data", menuName = "ScriptableObjects/Boat Physics Data" )]
public class PhysicsData : ScriptableObject
{
	[Header("Continuous forces")]
	public float linearDrag;
	public float forceScalar;
	public float gravity;
	public float buoyancy;
	public float minBuoyancyScalar;
	public float waterFlowForce;
	public float baseRowForce;
	public float singleOarSideScalar;
	public float singleOarForwardsScalar;
	public float twoOarsBackwardsScalar;
	[Header("Continuous torques")]
	public float angularDrag;
	public float torqueScalar;
	public float waterTorque;
	public float rowTorque;
	public float normalChangeTorque;
	[Header("Riverbank values")]
	public float riverbankSideForce;
	public float riverbankBackForce;
	public float riverbankMinScalar;
}
