using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Mathematics;

namespace InteractionSystem {
  [RequireComponent(typeof(Rigidbody))]
  [RequireComponent(typeof(Interactable))]
  public class Movable : MonoBehaviour {

    [
      Tooltip("Keep distance from the " + nameof(Interactor) + " within this range (from 0 to maximum interaction distance)"),
      MyBox.MinMaxRange(0, 1)
    ]
    public MyBox.FloatRange distanceRange = new MyBox.FloatRange(0, 1);

    [Tooltip("!!! Move " + nameof(Interactable) + " to the center of the " + nameof(Interactor) + "'s ray")]
    public bool restrictCenter = true;

    [Tooltip("Enable collision when no longer colliding with the " + nameof(Interactor) + " instead of immediately")]
    public bool waitCollisionEnd = true;

    public bool3 restrictRotation;
    public bool3 restrictPosition;

    public float baseReturnSpeed = 2f;
    public float returnTimeScale = 5f;

    private Interactable interactable;
    private Rigidbody rb;
    private CollisionTracker cc;


    private Interaction interaction;
    private PositionHistory posHistory;
    private List<Collider> associates = new List<Collider>();

    private float targetDistance;
    private float returnTime;
    private bool returned;
    private bool usedGravity;

    void Start() {
      rb = GetComponent<Rigidbody>();
      interactable = GetComponent<Interactable>();
      interactable.AddActivationEventListeners(OnActivate, OnActive, OnDeactive);

      cc = GetComponent<CollisionTracker>() ?? gameObject.AddComponent<CollisionTracker>();
      posHistory = GetComponent<PositionHistory>() ?? gameObject.AddComponent<PositionHistory>();
      posHistory.SetMinSize(2);
    }

    void FixedUpdate() {
      if (associates.Count > 0) {
        Collider[] cols = rb.GetComponentsInChildren<Collider>();
        Bounds bounds = new Bounds(transform.position, Vector3.zero);
        foreach (Collider nextCollider in cols) bounds.Encapsulate(nextCollider.bounds);

        var overlaps = Physics.OverlapBox(bounds.center, bounds.extents);

        var unignored = new List<Collider>();
        foreach (var associate in associates) {
          if (!overlaps.Contains(associate)) {
            Physics.IgnoreCollision(rb.GetComponent<Collider>(), associate, false);
            unignored.Add(associate);
          }
        }

        associates.RemoveAll(e => unignored.Contains(e));
      }
    }



    public void OnActivate(Interaction inter) {
      if (interaction && !interaction.ended) inter.End();
      interaction = inter;
      usedGravity = rb.useGravity;
      var associate = inter.source.associatedCollider;
      if (associate) {
        Physics.IgnoreCollision(rb.GetComponent<Collider>(), associate);
        associates.RemoveAll(e => e == associate);
      }
      rb.useGravity = false;
      var maxDif = inter.dif.SetLen(inter.source.maxDistance);
      Line line = new Line(
        Vector3.Lerp(inter.sourcePos, inter.sourcePos + maxDif, distanceRange.min),
        Vector3.Lerp(inter.sourcePos, inter.sourcePos + maxDif, distanceRange.max)
      );
      var closestPoint = line.ClampToLine(inter.targetPos);
      targetDistance = Vector3.Distance(inter.sourcePos, closestPoint);
    }


    public void OnActive(Interaction inter) {
      var targetPos = inter.sourcePos + (inter.dif.SetLenSafe(targetDistance).SetDirSafe(inter.source.transform.forward));
      var dif = targetPos - rb.position;
      var dir = dif.normalized;

      if (cc.colliding) {
        rb.AddForce(dir * inter.source.prefs.maxForce, ForceMode.Force);
        returned = false;
        returnTime = Time.time;
        // Project velocity towards target if there is no collision
        if (!rb.SweepTest(dir, out var _, dif.magnitude)) {
          rb.velocity = Vector3.Project(rb.velocity, dif);
        }
      } else if (rb.SweepTest(dir, out var _, dif.magnitude)) {
        rb.AddForce(dir * inter.source.prefs.maxForce, ForceMode.Force);
      } else {
        // No collision detected
        if (returned) {
          rb.velocity = Vector3.zero;
          inter.targetPos = targetPos;
        } else {
          float elapsed = (Time.time - returnTime) * returnTimeScale;
          float maxMove = baseReturnSpeed * Time.deltaTime + Mathf.Pow(elapsed, 2);
          transform.position = Vector3.MoveTowards(transform.position, targetPos, maxMove);
          if (transform.position == targetPos) returned = true;
        }
      }
    }


    public void OnDeactive(Interaction inter) {
      rb.useGravity = usedGravity;

      if (waitCollisionEnd && inter.source.associatedCollider)
        associates.Add(interaction.source.associatedCollider);

      // Transfer some force
      var prevDif = posHistory[1] - inter.source.posHistory[1];
      var vel = (inter.dif - prevDif) / Time.deltaTime;

      var force = vel * rb.mass;
      force = force.SetLenSafe(Mathf.Min(force.magnitude, inter.source.prefs.maxForce));
      rb.AddForce(force, ForceMode.Impulse);

      // Add interactor movement
      rb.velocity = (inter.sourcePos - inter.source.posHistory[1]) / Time.deltaTime;

    }
  }
}
