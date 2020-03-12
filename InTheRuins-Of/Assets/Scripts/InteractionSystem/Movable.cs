using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System.Linq;

namespace InteractionSystem {
  [RequireComponent(typeof(Rigidbody))]
  [RequireComponent(typeof(Interactable))]
  public class Movable : MonoBehaviour {

    [
      Tooltip("Keep distance from the " + nameof(Interactor) + " within this range (from 0 to maximum interaction distance)"),
      MyBox.MinMaxRange(0, 1)
    ]
    public MyBox.FloatRange distanceRange = new MyBox.FloatRange(0, 1);

    [Tooltip("Move " + nameof(Interactable) + " to the center of the " + nameof(Interactor) + "'s ray")]
    public bool restrictCenter;

    [Tooltip("Maximum force towards target movement position when colliding")]
    public float dragForce = 20;

    [Tooltip("Enable collision when no longer colliding with the " + nameof(Interactor) + " instead of immediately")]
    public bool waitCollisionEnd = true;

    public bool3 restrictRotation;
    public bool3 restrictPosition;


    private Interactable interactable;
    private Rigidbody rb;


    private Interaction interaction;
    private float targetDistance;
    private bool usedGravity;
    private Vector3 revRelPos { get => prevPoses[1]; }
    private CircularBuffer<Vector3> prevPoses = new CircularBuffer<Vector3>(2);
    private List<Collider> associates = new List<Collider>();
    private Collider waitForExit;

    void Start() {
      rb = GetComponent<Rigidbody>();
      interactable = GetComponent<Interactable>();
      interactable.AddActivationEventListeners(OnActivate, OnActive, OnDeactive);
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
      waitForExit = null;
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

    void OnCollisionExit(Collision col) {
      if (col.collider == waitForExit) {
        waitForExit = null;
      }
    }
    public void OnActive(Interaction inter) {
      prevPoses.Add(inter.dif);
      var targetPos = inter.sourcePos + (inter.dif.SetLenSafe(targetDistance).SetDirSafe(inter.source.transform.forward));
      var dif = targetPos - rb.position;
      var dir = dif.normalized;

      if (waitForExit) {
        rb.AddForce(dir * inter.source.prefs.maxForce, ForceMode.Force);
      } else {
        if (rb.SweepTest(dir, out var ad, dif.magnitude)) {
          waitForExit = ad.collider;
          rb.AddForce(dir * inter.source.prefs.maxForce, ForceMode.Force);
        } else {
          rb.velocity = Vector3.zero;
          transform.position = targetPos;
        }
      }
    }

    public void OnDeactive(Interaction inter) {
      rb.useGravity = usedGravity;

      if (waitCollisionEnd && inter.source.associatedCollider)
        associates.Add(interaction.source.associatedCollider);

      // Transfer some force
      var vel = (inter.dif - revRelPos) / Time.deltaTime;
      var force = vel * rb.mass;
      force = force.SetLenSafe(Mathf.Min(force.magnitude, inter.source.prefs.maxForce));
      rb.AddForce(force, ForceMode.Impulse);

      // Add own velocity
      rb.velocity = (inter.sourcePos - inter.source.prevPos) / Time.deltaTime;

    }
  }
}
