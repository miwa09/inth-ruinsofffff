using UnityEngine;

using MUC.Types.Extensions;


[RequireComponent(typeof(Controller))]
public class CharacterPushObjects : MonoBehaviour {

  public float force = 50;

  void OnControllerColliderHit(ControllerColliderHit hit) {
    var rb = hit.collider.attachedRigidbody;

    // No rigidbody
    if (rb == null || rb.isKinematic) {
      return;
    }

    // Ignore objects below us
    if (hit.moveDirection.y < -0.3f) {
      return;
    }

    // Push based on movement direction, only along the horizontal plane
    Vector3 pushDir = hit.moveDirection.SetY(0);

    // If you know how fast your character is trying to move,
    // then you can also multiply the push velocity by that.

    // Apply the push reflected along the hit normal
    Vector3 reflected = Vector3.Reflect(pushDir, hit.normal.SetY(0));
    rb.AddForce(reflected * force, ForceMode.Force);
  }
}