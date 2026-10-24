using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public static readonly List<Wall> All = new List<Wall>();

    void OnEnable()  => All.Add(this);
    void OnDisable() => All.Remove(this);
}