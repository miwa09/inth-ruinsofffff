using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System.Linq;

namespace InteractionSystem {
  [RequireComponent(typeof(Rigidbody))]
  [RequireComponent(typeof(Interactable))]
  public class Movable : MonoBehaviour {

    [Tooltip(
      "The maximum amount of samples to take from previous positions for velocity calculation.\n\n" +
      "Higher values reduce the chance of small movements affecting the throw in an unwanted manner."
    )]
    public int sampleCount = 3;

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

    private CircularBuffer<Sample> samples;
    private class Sample {
      public Vector3 pos; public float delta;
      public Sample(Vector3 pos, float delta) { this.pos = pos; this.delta = delta; }
    }

    private Interaction interaction;
    private float targetDistance;
    private bool usedGravity;
    private Vector3 prevPos;
    private List<Collider> associates = new List<Collider>();

    void OnValidate() {
      sampleCount = math.max(3, sampleCount);
      samples = new CircularBuffer<Sample>(sampleCount);
    }


    void Start() {
      OnValidate();
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

        var removes = new List<Collider>();
        foreach (var associate in associates) {
          if (!overlaps.Contains(associate)) {
            Physics.IgnoreCollision(rb.GetComponent<Collider>(), associate, false);
            removes.Add(associate);
          }
        }

        associates.RemoveAll(e => removes.Contains(e));
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
      var maxDir = inter.dir.SetLen(inter.source.maxDistance);
      Line line = new Line(
        Vector3.Lerp(inter.sourcePos, inter.sourcePos + maxDir, distanceRange.min),
        Vector3.Lerp(inter.sourcePos, inter.sourcePos + maxDir, distanceRange.max)
      );
      var closestPoint = line.ClampToLine(inter.targetPos);
      targetDistance = Vector3.Distance(inter.sourcePos, closestPoint);
    }

    public void OnActive(Interaction inter) {
      samples.Add(new Sample(transform.position, Time.deltaTime));
      var targetPos = inter.sourcePos + (inter.dir.SetLenSafe(targetDistance).SetDirSafe(inter.source.transform.forward));
      var dir = (targetPos - transform.position);
      var dirNorm = dir.normalized;

      if (rb.SweepTest(dirNorm, out var _, dir.magnitude)) {
        rb.AddForce(dirNorm * dragForce, ForceMode.Force);
      } else {
        rb.velocity = Vector3.zero;
        transform.position = targetPos;
      }
    }

    public void OnDeactive(Interaction inter) {
      rb.useGravity = usedGravity;
      if (waitCollisionEnd && inter.source.associatedCollider)
        associates.Add(interaction.source.associatedCollider);

      // Transfer velocity >>>
      var count = 0;
      Sample merged = new Sample(Vector3.zero, 0);
      foreach (var sample in samples) {
        if (sample != null) {
          count++;
          if (count == 1) continue;
          merged.pos += sample.pos;
          merged.delta += sample.delta;
        }
      }
      // Need atleast 3 valid samples
      if (count >= 3) {
        var oldest = samples[samples.Length - 1].pos;
        var vel = -(oldest - merged.pos / (count - 1)) / merged.delta;
        rb.velocity = vel;
      } else {
        rb.velocity = Vector3.zero;
      }
      samples.Clear();
      // <<<
    }
  }
}
