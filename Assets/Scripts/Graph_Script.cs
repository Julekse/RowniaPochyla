using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class Graph_Script : MonoBehaviour
{

    public double s, V, time_passed;
    public Square_script square_script;
    private List<Vector3> points;
    private LineRenderer lr;

    void Start()
    {
        points = new List<Vector3>();

        square_script = FindObjectOfType<Square_script>();
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 0;

        s = 0;
        V = 0;
        time_passed = 0;
    }

    void Update()
    {
        s = square_script.s;
        time_passed = square_script.time_passed;

        points.Add(new Vector3((float)time_passed, (float)s, 0));

        lr.positionCount += 1;
        lr.SetPositions(points.ToArray());
    }
}
