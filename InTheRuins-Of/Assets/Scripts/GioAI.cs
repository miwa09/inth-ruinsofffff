

namespace global {

  using System.Collections;
  using System.Collections.Generic;

  using UnityEngine;

  using Muc.Types.Extensions;


  [RequireComponent(typeof(Rigidbody))]
  [RequireComponent(typeof(Collider))]
  public partial class GioAI : MonoBehaviour {

    public Transform player;


    public Vector3 moveTarget;
    public Vector3 lookTarget;

    public GioActivate buttonTarget;


    // Tweaks

    public float maxVelocity = 10;

    public float nearDist = 1;
    public Vector3 nearDistPlayerOffset;

    // How much to face towards velocity direction instead of lookTarget
    public float faceMovementFraction = 0;


    // Internal

    private Rigidbody rb;
    private Collider col;

    private Vector3 playerPos => player.transform.position;
    private Vector3 position => transform.position;


    void Start() {
      rb = GetComponent<Rigidbody>();
      col = GetComponent<Collider>();
    }


    void Update() {

      var moveTarget = CalculateNearPoint();


      // Movement
      var moveDif = moveTarget - position;

      rb.velocity += moveDif * Time.deltaTime;
      if (rb.velocity.magnitude > maxVelocity) rb.velocity = rb.velocity.SetLen(maxVelocity);

      // Look
      var lookDif = lookTarget - position;

      var lookRot = Quaternion.LookRotation(lookDif);
      var faceVelocityRot = Quaternion.Euler(rb.velocity);
      var rotation = Quaternion.Lerp(lookRot, faceVelocityRot, faceMovementFraction);
      transform.rotation = Quaternion.Euler(rotation.eulerAngles);

    }

    public Vector3 CalculateNearPoint() {
      return playerPos + (position - playerPos).SetLenSafe(nearDist, Vector3.one.xoo());
    }

  }
}


#if UNITY_EDITOR
namespace global {

  using UnityEngine;
  using UnityEditor;
  using Unity.Mathematics;
  using static Unity.Mathematics.math;

  using Muc.Types.Extensions;


  public partial class GioAI {
    void OnDrawGizmos() {
      DrawArrowWithTargetSphere(playerPos, CalculateNearPoint(), Color.gray);
      DrawArrowWithTargetSphere(position, moveTarget, Color.cyan);
      DrawArrowWithTargetSphere(position, lookTarget, Color.yellow);
    }


    void DrawArrowWithTargetSphere(float3 sourcePoint, float3 endPoint, Color color) {
      var prevColor = Handles.color;
      Handles.color = color;

      var dist = distance(endPoint, sourcePoint);
      dist = min(dist, 1);

      var maxSize = 0.1f;
      var size = min(maxSize, dist / 10);

      var dir = endPoint - sourcePoint;
      var clampDir = dir.SetLenSafe(dist);
      var clampEndPoint = sourcePoint + clampDir;

      Handles.ConeHandleCap(0, clampEndPoint - (dir).SetLen(size) * 0.7f, Quaternion.LookRotation(dir), size, EventType.Repaint);
      Handles.DrawAAPolyLine(sourcePoint, clampEndPoint);

      Handles.SphereHandleCap(0, endPoint, Quaternion.identity, size, EventType.Repaint);

      Handles.color = prevColor;
    }
  }
}
#endif