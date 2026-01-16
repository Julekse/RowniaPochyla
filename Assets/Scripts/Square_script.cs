using UnityEngine;
using UnityEngine.UI;
using System;

public class Square_script : MonoBehaviour
{
    [Header("UI References")]
    public Text dane;
    public InputField velocity_in, angle_in, friction_in;

    [Header("Environment References")]
    public GameObject hill;
    public GameObject ground;
    public GameObject cameraObject;

    [Header("Physics Settings")]
    public double Vp;
    public double friction_factor;
    public float angle;
    public bool isstopped = false;
    
    [Header("Runtime Data")]
    public double currentVp;
    public double s, max_s;
    public double time_passed, global_time;
    public int iteration;

    public event Action OnStopped;
    public event Action OnStarted;

    private const double G = 9.80665;
    private readonly Vector3 start_camera_vector = new Vector3(7.0f, 2.0f, -10.0f);
    private const float EPS = 0.05f;

    void Start()
    {
        Reset();
    }

    void Update()
    {
        HandleInput();
        
        if (!isstopped)
        {
            MoveSquare();
            ShowDane();
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.P))
        {
            Reset();
        }
        
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("Symulacja zakończona");
            Application.Quit();
        }
    }

    public void Reset()
    {
        ReadInputData();
        iteration = 0;
        time_passed = 0;
        global_time = 0;
        currentVp = Vp;
        max_s = 0;
        s = 0;

        // Początkowe transformacje
        transform.eulerAngles = new Vector3(0f, 0f, angle);
        hill.transform.eulerAngles = new Vector3(0f, 0f, angle);
        
        float touchHeight = GetTouchHeight();
        ground.transform.position = new Vector3(0f, -5.61f - touchHeight, -0.1f);
        cameraObject.transform.position = start_camera_vector;
        
        isstopped = false;
        OnStarted?.Invoke();
        ShowDane();
    }

    private void ReadInputData()
    {
        // Bezpieczne czytanie
        double.TryParse(velocity_in.text, out Vp);
        double.TryParse(angle_in.text, out double parsedAngle);
        double.TryParse(friction_in.text, out friction_factor);

        angle = (float)Mathf.Clamp((float)parsedAngle, 0, 90);
        Vp = Mathf.Clamp((float)Vp, 0, 20);
        friction_factor = Mathf.Clamp((float)friction_factor, 0, 1);
    }

    private void MoveSquare()
    {
        time_passed += Time.deltaTime;
        global_time += Time.deltaTime;

        // Czy wrócił na dół
        if (s < 0 && time_passed > 0.1)
        {
            currentVp *= GetVelocityLossFactor();
            time_passed = 0;
            iteration++;
            max_s = 0;
        }

        // Zatrzymanie
        if (currentVp < EPS)
        {
            StopSquare();
            return;
        }

        s = CalculateDistance(time_passed, currentVp);
        if (max_s < s) max_s = s;

        UpdatePositions();
    }

    private void UpdatePositions()
    {
        float rad = angle * Mathf.Deg2Rad;
        float touchHeight = GetTouchHeight();
        
        //Kwadrat i góra
        Vector3 newPos = new Vector3((float)(s * Math.Cos(rad)), (float)(s * Math.Sin(rad)), 0);
        transform.position = newPos;
        hill.transform.position = newPos;

        //Podłoże
        ground.transform.position = new Vector3(transform.position.x, -5.61f - touchHeight, -0.1f);

        //Kamera
        UpdateCamera(touchHeight);
    }

    private void UpdateCamera(float touchHeight)
    {
        float screenRatio = (float)Screen.width / Screen.height;
        float camXLimit = start_camera_vector.x + (5 * screenRatio);
        
        Vector3 camPos = cameraObject.transform.position;

        if (transform.position.x + touchHeight > camXLimit)
            camPos.x = start_camera_vector.x + (transform.position.x + touchHeight - camXLimit);

        if (transform.position.y + touchHeight > start_camera_vector.y + 5)
            camPos.y = start_camera_vector.y + (transform.position.y + touchHeight - (start_camera_vector.y + 5));

        cameraObject.transform.position = camPos;
    }

    //FIZYKAAAA

    public double GetAccUp() => -G * (Math.Sin(angle * Mathf.Deg2Rad) + (friction_factor * Math.Cos(angle * Mathf.Deg2Rad)));
    
    private double GetAccDown() => -G * (Math.Sin(angle * Mathf.Deg2Rad) - (friction_factor * Math.Cos(angle * Mathf.Deg2Rad)));

    private double GetVelocityLossFactor() => Math.Sqrt(Math.Abs(GetAccDown() / GetAccUp()));

    private double CalculateDistance(double t, double Vo)
    {
        double accUp = GetAccUp();
        double accDown = GetAccDown();
        double timeToPeak = -Vo / accUp;

        if (t < timeToPeak)
        {
            return (accUp / 2 * t * t) + (Vo * t);
        }

        double peakDistance = -Vo * Vo / (2 * accUp);
        
        if (accDown > 0) //Zatrzymanie na górze
        {
            StopSquare();
            return peakDistance;
        }

        return (accDown / 2 * Math.Pow(t - timeToPeak, 2)) + peakDistance;
    }

    private float GetTouchHeight() => 0.5f * Mathf.Sqrt(2) * Mathf.Sin((135 - angle) * Mathf.Deg2Rad);

    private void ShowDane()
    {
        dane.text = $"Vp: {Vp:n1}\nKąt: {angle:n0}\nμ: {friction_factor:n2}\nLiczba cykli: {iteration}\nCzas: {global_time:n2} s";
    }

    private void StopSquare()
    {
        if (!isstopped)
        {
            isstopped = true;
            OnStopped?.Invoke();
        }
    }
}