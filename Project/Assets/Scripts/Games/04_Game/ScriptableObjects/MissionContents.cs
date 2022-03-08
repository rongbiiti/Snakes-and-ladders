using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "MyScriptable/Create MissionContents")]
public class MissionContents : ScriptableObject
{
    public string[] MissionList;
}
