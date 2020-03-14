using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;



[RequireComponent(typeof(Collider))]
public class CollisionTracker : MonoBehaviour {

  public bool colliding { get => colliders.Count > 0; }

  protected List<Collider> colliders = new List<Collider>();

  void Update() {
    Prune();
  }


  public bool CollidingWith(Collider collider) {
    Prune();
    return colliders.Contains(collider);
  }

  public bool CollidingWith(GameObject gameObject, bool includeChildren = true) {
    Prune();
    var cols = includeChildren ? gameObject.GetComponentsInChildren<Collider>() : gameObject.GetComponents<Collider>();
    return colliders.Any(c => cols.Contains(c));
  }


  void Prune() {
    colliders.RemoveAll(c => !c || !c.enabled || c.isTrigger);
  }

  void OnCollisionEnter(Collision col) {
    colliders.Add(col.collider);
  }

  void OnCollisionExit(Collision col) {
    colliders.RemoveAll(c => c == col.collider);
  }
}
