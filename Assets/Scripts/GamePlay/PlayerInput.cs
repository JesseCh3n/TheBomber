using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DefaultExecutionOrder(-100)]
public class PlayerInput : MonoBehaviour
{
    public bool _isDiabled = true;
    public bool _weapon1Pressed { get; private set; }
    public bool _weapon2Pressed { get; private set; }

    public float _horizontal { get; private set; }
    public float _vertical { get; private set; }
    public float _mouseX { get; private set; }
    public float _mouseY { get; private set; }

    public bool _leftPressed { get; private set; }

    public bool _rightPressed { get; private set; }

    public bool _sprintHeld { get; private set; }
    public bool _jumpPressed { get; private set; }

    public bool _undoPressed { get; private set; }
    public bool _activatePressed { get; private set; }
    public bool _primaryShootPressed { get; private set; }
    public bool _secondaryShootPressed { get; private set; }
    public bool _commandPressed { get; private set; }

    public bool _escapePressed { get; private set; }

    //Singelton
    private static PlayerInput instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(instance);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        _isDiabled = false;
    }
    public static PlayerInput GetInstance()
    {
        return instance;
    }
    //Singelton


    //flag to indicate if inputs should be cleared
    private bool _clear;

    private void ProcessInput()
    {
        _horizontal = Input.GetAxis("Horizontal");
        _vertical = Input.GetAxis("Vertical");
        _mouseX = Input.GetAxis("Mouse X");
        _mouseY = Input.GetAxis("Mouse Y");

        _leftPressed = _leftPressed || Input.GetKeyDown(KeyCode.A);
        _rightPressed = _rightPressed || Input.GetKeyDown(KeyCode.D);
        _jumpPressed = _jumpPressed || Input.GetButtonDown("Jump");
        _sprintHeld = _sprintHeld || Input.GetButton("Sprint");
        _activatePressed = _activatePressed || Input.GetKeyDown(KeyCode.E);
        _primaryShootPressed = _primaryShootPressed || Input.GetButtonDown("Fire1");

        _weapon1Pressed = _weapon1Pressed || Input.GetKeyDown(KeyCode.Alpha1);
        _weapon2Pressed = _weapon2Pressed || Input.GetKeyDown(KeyCode.Alpha2);
        _commandPressed = _commandPressed || Input.GetKeyDown(KeyCode.G);
        _undoPressed = _undoPressed || Input.GetKeyDown(KeyCode.R);
        _escapePressed = _escapePressed || Input.GetKeyDown(KeyCode.Escape);
    }

    //Called once per fixed frame-rate frame
    private void FixedUpdate()//50
    {
        _clear = true;
    }

    void Update()//30 --- 40- 60
    {
        ClearInputs();
        if (!_isDiabled)
        {
            ProcessInput();
        }

    }

    private void ClearInputs()
    {
        if (!_clear)
        {
            return;
        }

        _horizontal = 0;
        _vertical = 0;
        _mouseX = 0;
        _mouseY = 0;
        _sprintHeld = false;
        _activatePressed = false;
        _primaryShootPressed = false;
        _secondaryShootPressed = false;
        _leftPressed = false;
        _rightPressed = false;
        _jumpPressed = false;//////////////////////
        _weapon1Pressed = false;
        _weapon2Pressed = false;
        _commandPressed = false;
        _undoPressed = false;
        _escapePressed = false;
    }
}
