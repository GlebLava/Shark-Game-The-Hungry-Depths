using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }


        SignalBus.OnLevelFinishedLoading += () =>
        {
            swimJoystick = GameObject.FindGameObjectWithTag("SwimJoystick").GetComponent<Joystick>();
            touchArea = GameObject.FindGameObjectWithTag("SwimUpTouchArea").GetComponent<TouchArea>();
        };

        SignalBus.OnLevelFinishedReturnToMainMenu += () =>
        {
            swimJoystick = null;
            touchArea = null;
        };

        // DEBUG PURPOSES, so we can start a level directly instead of through the main menu
        var sj = GameObject.FindGameObjectWithTag("SwimJoystick");
        var suta = GameObject.FindGameObjectWithTag("SwimUpTouchArea");
        if (sj != null) swimJoystick = sj.GetComponent<Joystick>();
        if (suta != null) touchArea = suta.GetComponent<TouchArea>();
    }

    public Vector2 swimDir;
    public bool pressingSwimUp;

    // mobile joystick
    private Joystick swimJoystick;
    private TouchArea touchArea;

    private void Update()
    {
        // swimDir
        if (swimJoystick != null)
        {
            swimDir = swimJoystick.Direction;
        }

        if (swimDir.sqrMagnitude < 0.1f)
        {
            swimDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }


        // swimmingUp
        if (touchArea != null)
        {
            pressingSwimUp = false;
            pressingSwimUp = touchArea.pointerDown;
        }

    }
}
