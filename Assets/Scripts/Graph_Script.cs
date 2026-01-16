using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(LineRenderer))]
public class Graph_Script : MonoBehaviour
{
    public float s, s_max;
    public Square_script square_script;
    public GameObject main_camera;
    public Text SmaxText;

    private List<Vector3> points = new List<Vector3>();
    private LineRenderer lr;
    private Vector3 start_graph_vector;
    private float time_passed;

    void Begin()
    {
        time_passed = 0;
        s = 0;
        points.Clear();
        lr.positionCount = 0;
        
        transform.position = start_graph_vector + main_camera.transform.position;
        SmaxText.rectTransform.anchoredPosition = new Vector2(500, SmaxText.rectTransform.anchoredPosition.y);
    }

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        square_script = FindFirstObjectByType<Square_script>();

        //USTAWIENIA LINII
        lr.useWorldSpace = false;
        lr.alignment = LineAlignment.View;
        lr.widthMultiplier = 0.2f;
        lr.numCornerVertices = 5;

        start_graph_vector = new Vector3(4.9f * Screen.width / Screen.height, -4.9f, 9);
        Begin();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.P))
            Begin();

        if (square_script != null && !square_script.isstopped)
        {
            time_passed += Time.deltaTime;
            
            //Fizyka
            s_max = -(float)square_script.Vp * (float)square_script.Vp / 2.0f / (float)square_script.GetAccUp();
            SmaxText.text = s_max.ToString("n2") + " m";
            s = (float)square_script.s;

            points.Add(new Vector3(time_passed, (float)(s * 2.0f / s_max), 0));

            lr.positionCount = points.Count;
            lr.SetPositions(points.ToArray());

            transform.position = start_graph_vector + main_camera.transform.position + new Vector3(-time_passed, 0, 0);

            UpdateText();
        }
    }

    void UpdateText()
    {
        float targetX = (start_graph_vector.x - time_passed) * 108;
        if (targetX < -900) targetX = -900;
        
        SmaxText.rectTransform.anchoredPosition = new Vector2(targetX, SmaxText.rectTransform.anchoredPosition.y);
    }
}