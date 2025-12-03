using System;
using NUnit.Framework;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Square_script : MonoBehaviour
{
    public Text dane;
    public GameObject hill, ground, cameraObject;
    public InputField velocity_in, angle_in, friction_in;
    public double Vp, Current_Vp, friction_factor, max_s, s, iteration, time_passed;
    float angle;
    public bool isstopped = false;
    double g = 9.80665f;

    // zdarzenia do powiadamiania innych skryptów
    public event Action OnStopped;
    public event Action OnStarted;

    Vector3 start_camera_vector = new Vector3(7.0f, 4.0f, -10.0f);

    void StopSquare()
    {
        if (!isstopped)
        {
            isstopped = true;
            Debug.Log("Square_script: StopSquare() wywołane. iteration=" + iteration);
            OnStopped?.Invoke();
        }
    }

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

    double GetDistance(double t, double Vo)
    {
        double p, q, V2, t_max, wsp_a, ap, ad;
        ap = GetAccUp();
        ad = GetAccDown();
        p = -Vo/ap;
        if (time_passed < p)
        {
            return ap/2*t*t + Vo * t;
        }
        q = -Vo*Vo / (2*ap);
        if (ad > 0)
        {
            StopSquare();
            return q;
        }
        V2 = Math.Sqrt(2 * g * q);
        t_max = V2/ad;
        wsp_a = V2/(t_max-p);
        return wsp_a * (t-p)*(t-p) + q;
    }

    void ShowDane()
    {
        dane.text = "Vp: " + Vp.ToString("n1") + "\n" +
        "Kąt: " + angle.ToString("n0") + "\n" +
        "μ: " + friction_factor.ToString("n2") + "\n" +
        "Liczba cykli: " + iteration.ToString();
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
        return 1 / 2f * (float)Math.Sqrt(2) * (float)Math.Sin(Rad(135 - angle));
    }

    void Reset()
    {
        GetDane();
        iteration = 0;
        time_passed = 0;
        Current_Vp = Vp;
        transform.eulerAngles = new Vector3(0f, 0f, angle);
        hill.transform.eulerAngles = new Vector3(0f, 0f, angle);
        ground.transform.position = new Vector3(0f, -5.61f - GetTouchHeight(), -0.1f);
        cameraObject.transform.position = start_camera_vector;
        max_s = 0;
        s = 0;

        // jeśli wcześniej było zatrzymane, sygnalizujemy wznowienie
        if (isstopped)
        {
            isstopped = false;
            OnStarted?.Invoke();
        }

        ShowDane();
    }

    // ... (reszta metody MoveSquare/Update bez zmian) ...

    public float EPS = 0.01f, SEPS = 0.1f;

    double GetNewVelocity()
      {
        return math.sqrt((Math.Sin(Rad(angle)) - (friction_factor * Math.Cos(Rad(angle))))/(Math.Sin(Rad(angle)) + (friction_factor * Math.Cos(Rad(angle)))));
      }

    void MoveSquare()
    {
        if (isstopped) return;

        time_passed += Time.deltaTime;

        if (s < 0)
            {
                  Current_Vp *= GetNewVelocity();
                  time_passed = 0;
                  iteration += 1;
                  max_s = 0;
                  ShowDane();
            }

        if (!isstopped && Current_Vp < EPS)
            {
                StopSquare();
            }

        if (isstopped && Current_Vp > EPS)
            {
                isstopped = false;
                OnStarted?.Invoke();
             
            }

        s = GetDistance(time_passed, Current_Vp);
        if (max_s < s) max_s = s;

        Vector3 new_pos = new Vector3((float)(s * Math.Cos(Rad(angle))), (float)(s * Math.Sin(Rad(angle))), 0);
        transform.position = new_pos;
        hill.transform.position = new_pos;
        ground.transform.position = new Vector3(transform.position.x, -5.61f - GetTouchHeight(), -0.1f);
        float screen_ratio = 1.0f * Screen.width / Screen.height;
        if (transform.position.x + GetTouchHeight() > start_camera_vector.x + (5 * screen_ratio))
            cameraObject.transform.position = start_camera_vector + new Vector3(transform.position.x + GetTouchHeight() - start_camera_vector.x - (5 * screen_ratio), 0, 0);
        if (transform.position.y + GetTouchHeight() > start_camera_vector.y + 5)
            cameraObject.transform.position += new Vector3(0, transform.position.y + GetTouchHeight() - (cameraObject.transform.position.y + 5), 0);
    }

    private void Start()
    {
        ShowDane();
        Reset();
    }

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