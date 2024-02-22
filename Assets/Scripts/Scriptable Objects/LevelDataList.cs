using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Data List", menuName = "ScriptableObjects/Level Data List" )]
public class LevelDataList : ScriptableObject
{
	public LevelData[] levels;
}
