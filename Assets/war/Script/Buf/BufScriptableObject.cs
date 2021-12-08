using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/BufScriptableObject", order = 1)]
public class BufScriptableObject : ScriptableObject
{
    public string buf_name;
    public float max_buf_time;
    public float value;
}