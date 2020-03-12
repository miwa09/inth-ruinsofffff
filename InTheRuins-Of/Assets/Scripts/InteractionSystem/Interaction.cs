using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractionSystem {
  public class Interaction {

    public readonly Interactor source;
    public readonly Interactable target;
    public readonly float startDistance;
    public readonly float startTime;

    public Vector3 sourcePos { get => source.transform.position; set => source.transform.position = value; }
    public Vector3 targetPos { get => target.transform.position; set => target.transform.position = value; }

    /// <summary> Vector between source and target </summary>
    public Vector3 dif {
      get => targetPos - sourcePos;
      set => target.transform.position = sourcePos + value;
    }

    public float distance {
      get => Vector3.Distance(sourcePos, targetPos);
      set => target.transform.position = sourcePos + dif * value;
    }

    public float duration { get => ended ? endTime - startTime : Time.time - startTime; }

    public bool ended { get; private set; }
    public float endTime { get; private set; }

    public static implicit operator bool(Interaction interaction) => interaction != null;

    public Interaction(Interactor source, Interactable target) : this(source, target, Time.time) { }
    public Interaction(Interactor source, Interactable target, float startTime) {
      this.source = source;
      this.target = target;
      this.startDistance = Vector3.Distance(source.transform.position, target.transform.position);
      this.startTime = startTime;
    }

    public void End() => End(Time.time);
    public void End(float time) {
      if (ended) return;
      ended = true;
      endTime = time;
    }
  }
}