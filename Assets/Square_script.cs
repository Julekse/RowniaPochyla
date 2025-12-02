using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Square_script : MonoBehaviour
{
    public Text dane;
    public GameObject hill, ground, cameraObject;
    public InputField velocity_in, angle_in, friction_in;
    double velocity, Vp, old_v, friction_factor, a, max_s, s, iteration;
    float angle;
    double g = 9.80665f;
    Vector3 start_camera_vector = new Vector3(7, 4, -10);

    double Rad(double angle_stp)
      {
            return angle_stp * Math.PI / 180;
      }
    double GetAccUp()
      {
            return -g * (Math.Sin(Rad(angle)) + (friction_factor * Math.Cos(Rad(angle))));
      }
    double GetAccDown()
      {
            return -g * (Math.Sin(Rad(angle)) - (friction_factor * Math.Cos(Rad(angle))));
      }

    void ShowDane()
      {
            dane.text = "Vp: " + Vp.ToString("n1") + "\n" +
            "Kąt: " + angle.ToString("n0") + "\n" +
            "μ: " + friction_factor.ToString("n2") + "\n" +
            "Idx: " + iteration.ToString()
            ;

      }

    void GetDane()
      {
            Vp = (float)Convert.ToDouble(velocity_in.text);
            angle = (float)Convert.ToDouble(angle_in.text);
            friction_factor = (float)Convert.ToDouble(friction_in.text);

            if (Vp < 0) Vp = 0;
            if (Vp > 20) Vp = 20;
            if (angle < 0) angle = 0;
            if (angle > 90) angle = 90;
            if (friction_factor < 0) friction_factor = 0;
            if (friction_factor > 1) friction_factor = 1;

            ShowDane();
        
      }

    float GetTouchHeight()
      {
            return 1/2f * (float)Math.Sqrt(2) * (float)Math.Sin(Rad(135 - angle));
      }

      void Reset()
    {
        GetDane();
        iteration = 0;
        velocity = Vp;
        transform.eulerAngles = new Vector3(0f, 0f, angle);
        hill.transform.eulerAngles = new Vector3(0f, 0f, angle);
        ground.transform.position = new Vector3(0f, -5.61f - GetTouchHeight(), -0.1f);
        cameraObject.transform.position = new Vector3(7f, 4f, -10f);
        a = GetAccUp();
        max_s = 0;
        s = 0;
        ShowDane();
    }
      // Start is called once before the first execution of Update after the MonoBehaviour is created
      void Start()
    {   
        ShowDane();
        Reset();
    }

    public float EPS = 0.01f, SEPS = 0.1f;

    void MoveSquare()
      {
        if (s < 0)
            {
                velocity *= -1;
                a = GetAccUp();
                iteration += 1;
                max_s = 0;
                s = 0;
                ShowDane();
            }
        if (old_v > 0 && velocity < 0)
            {
                a = GetAccDown();
                old_v = 0;
            }

        if (a > 0)
            {
                a = 0;
                old_v = 0;
                velocity = 0;
            }
           
        if (velocity < 0 && max_s < SEPS)
            {
                Reset();
            }
        else
            {
                old_v = velocity;
                velocity += a * Time.deltaTime; 
            }
        
        s += (float)velocity * Time.deltaTime;
        
        if (s > max_s)
            {
                max_s = s;
            }

        Vector3 new_pos = new Vector3((float)(s*Math.Cos(Rad(angle))), (float)(s*Math.Sin(Rad(angle))), 0);
        transform.position = new_pos;
        hill.transform.position = new_pos;
        ground.transform.position = new Vector3(transform.position.x, -5.61f - GetTouchHeight(), -0.1f);
        float screen_ratio = Screen.width/Screen.height;
        if (transform.position.x + GetTouchHeight() > start_camera_vector.x + (5*screen_ratio)) 
          cameraObject.transform.position = start_camera_vector + new Vector3(transform.position.x + GetTouchHeight() - start_camera_vector.x - (5*screen_ratio), 0, 0);
        if (transform.position.y + GetTouchHeight() > start_camera_vector.y + 5) 
          cameraObject.transform.position += new Vector3(0, transform.position.y + GetTouchHeight() - (cameraObject.transform.position.y + 5), 0);
      }
    // Update is called once per frame

    void Update()
    {   
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.P))
            {
                  Reset();
            }
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.X))
            {
              Debug.Log("End");
              Application.Quit();
            }
        MoveSquare();
    }
}
