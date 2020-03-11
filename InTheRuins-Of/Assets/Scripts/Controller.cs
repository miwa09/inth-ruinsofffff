using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Controller : MonoBehaviour {
  //Urg that's ugly, maybe find a better way
  public static Controller Instance { get; protected set; }

  public Camera cam;

  public float sensitivity = 5;
  public float PlayerSpeed = 5;
  public float RunningSpeed = 8;
  public float JumpSpeed = 5;


  float verticalSpeed = 0.0f;
  bool paused = false;

  float verAngle, playerAngle;

  public float speed { get; private set; }

  public bool lockControl;
  public bool canPause = true;

  public bool Grounded => grounded;

  CharacterController charCtrlr;

  bool grounded;
  float m_GroundedTimer;
  float speedAtJump = 0.0f;

  void Awake() {
    Instance = this;
  }

  void Start() {
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;

    paused = false;
    grounded = true;

    charCtrlr = GetComponent<CharacterController>();


    verAngle = 0.0f;
    playerAngle = transform.localEulerAngles.y;
  }

  void Update() {
    bool wasGrounded = grounded;
    bool loosedGrounding = false;

    //we define our own grounded and not use the Character controller one as the character controller can flicker
    //between grounded/not grounded on small step and the like. So we actually make the controller "not grounded" only
    //if the character controller reported not being grounded for at least .5 second;
    if (!charCtrlr.isGrounded) {
      if (grounded) {
        m_GroundedTimer += Time.deltaTime;
        if (m_GroundedTimer >= 0.5f) {
          loosedGrounding = true;
          grounded = false;
        }
      }
    } else {
      m_GroundedTimer = 0.0f;
      grounded = true;
    }

    speed = 0;
    Vector3 move = Vector3.zero;
    if (!paused && !lockControl) {
      // Jump (we do it first as 
      if (grounded && Input.GetButtonDown("Jump")) {
        verticalSpeed = JumpSpeed;
        grounded = false;
        loosedGrounding = true;
      }

      bool running = Input.GetButton("Run");
      float actualSpeed = running ? RunningSpeed : PlayerSpeed;

      if (loosedGrounding) {
        speedAtJump = actualSpeed;
      }

      // Move around with WASD
      move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
      if (move.sqrMagnitude > 1.0f)
        move.Normalize();

      var usedSpeed = grounded ? actualSpeed : speedAtJump;

      move = move * usedSpeed * Time.deltaTime;

      move = transform.TransformDirection(move);
      charCtrlr.Move(move);

      // Player rotate left/right
      var playerRotation = Input.GetAxis("Mouse X") * sensitivity;
      playerAngle = playerAngle + playerRotation;

      if (playerAngle > 360) playerAngle -= 360.0f;
      if (playerAngle < 0) playerAngle += 360.0f;

      var angles = transform.localEulerAngles;
      angles.y = playerAngle;
      transform.localEulerAngles = angles;

      // Camera look up/down
      var cameraRotation = -Input.GetAxis("Mouse Y");
      cameraRotation = cameraRotation * sensitivity;
      verAngle = Mathf.Clamp(cameraRotation + verAngle, -89.9f, 89.9f);
      angles = cam.transform.localEulerAngles;
      angles.x = verAngle;
      cam.transform.localEulerAngles = angles;


      speed = move.magnitude / (PlayerSpeed * Time.deltaTime);


      //Key input to change weapon

      for (int i = 0; i < 10; ++i) {
        if (Input.GetKeyDown(KeyCode.Alpha0 + i)) {
          int num = 0;
          if (i == 0)
            num = 10;
          else
            num = i - 1;

        }
      }
    }

    // Fall down / gravity
    verticalSpeed = verticalSpeed - 10.0f * Time.deltaTime;
    if (verticalSpeed < -10.0f)
      verticalSpeed = -10.0f; // max fall speed
    var verticalMove = new Vector3(0, verticalSpeed * Time.deltaTime, 0);
    var flag = charCtrlr.Move(verticalMove);
    if ((flag & CollisionFlags.Below) != 0)
      verticalSpeed = 0;

  }

  public void DisplayCursor(bool display) {
    paused = display;
    Cursor.lockState = display ? CursorLockMode.None : CursorLockMode.Locked;
    Cursor.visible = display;
  }
}
