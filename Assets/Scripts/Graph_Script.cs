using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager.Requests;

[RequireComponent(typeof(LineRenderer))]
public class Graph_Script : MonoBehaviour
{

    public float s, V, time_passed, s_max;
    public Square_script square_script;
    public GameObject main_camera;
    private List<Vector3> points;
    private LineRenderer lr;
    private Vector3 start_graph_vector;

    void Begin()
      {
            time_passed=0;
            s=0;
            points.Clear();
            lr.positionCount = 0;
            transform.position = start_graph_vector;
      }
    void Start()
    {
        points = new List<Vector3>();
        square_script = FindObjectOfType<Square_script>();
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 0;

        start_graph_vector = new Vector3(4.9f*Screen.width/Screen.height,-4.9f, 9);
        transform.position = start_graph_vector + main_camera.transform.position;

        s = 0;
        s_max=0;
        V = 0;
        time_passed = 0;
    }
      void Update()
    {
        time_passed += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.P))
            {
                Begin();
            }
        if (!square_script.isstopped)
            {
                s_max=-(float)square_script.Vp*(float)square_script.Vp /2.0f /(float)square_script.GetAccUp();
                s = (float)square_script.s;

                points.Add(new Vector3(time_passed, (float)(s*2.0f/s_max), 0));
                transform.position = new Vector3(-time_passed, 0, 0) + start_graph_vector + main_camera.transform.position;

                lr.positionCount += 1;
                lr.SetPositions(points.ToArray());
            }
    }
}
