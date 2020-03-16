using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToPosition : MonoBehaviour {

  [Tooltip("The destination position. This transform will be unparented if a child of this GameObject")]
  public Transform destination;

  [Tooltip("The speed of interpolation or absolute speed")]
  public float speed;
  [Tooltip("Move " + nameof(speed) + "units instead of interpolating between the position")]
  public bool absoluteUnits;

  public State state;

  private float t;
  private Vector3 origin;


  public void MoveToOrigin() {
    state = State.MovingToOrigin;
  }
  public void MoveToDestination() {
    state = State.MovingToDestination;
  }


  void Start() {
    origin = transform.position;
    if (destination.parent == transform) {
      destination.parent = transform.parent;
    }
  }

  void Update() {
    switch (state) {
      case State.Origin:
      case State.Destination:
        break;
      case State.MovingToDestination:
        t += GetInterpolationSpeed(speed) * Time.deltaTime;
        if (t >= 1) {
          t = 1;
          state = State.Destination;
        }
        UpdatePos();
        break;
      case State.MovingToOrigin:
        t -= GetInterpolationSpeed(speed) * Time.deltaTime;
        if (t <= 0) {
          t = 0;
          state = State.Destination;
        }
        UpdatePos();
        break;
    }
  }

  void OnDrawGizmosSelected() {

  }

  void UpdatePos() {
    transform.position = Vector3.Lerp(origin, destination.position, t);
  }

  float GetInterpolationSpeed(float speed) {
    if (!absoluteUnits) return speed;
    var distance = Vector3.Distance(origin, destination.position);
    return speed / distance;
  }

  public enum State {
    Origin,
    Destination,
    MovingToDestination,
    MovingToOrigin
  }
}
