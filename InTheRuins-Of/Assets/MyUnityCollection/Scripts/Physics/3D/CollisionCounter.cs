using UnityEngine;
using System.Collections;



[RequireComponent(typeof(Collider))]
public class CollisionCounter : MonoBehaviour {

  public bool colliding { get; private set; }

  private int count;

  // Happens first
  void FixedUpdate() {
    colliding = count > 0;
    count = 0;
    print(colliding);
  }

  void OnCollisionEnter() => OnCollisionStay();
  void OnCollisionStay() {
    // !!! stay is not always used before exit
    count++;
  }
}
