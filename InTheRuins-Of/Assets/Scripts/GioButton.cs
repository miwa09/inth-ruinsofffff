
namespace global {

  using System.Collections;
  using System.Collections.Generic;

  using UnityEngine;
  using UnityEngine.Events;

  public class GioActivate : MonoBehaviour {

    public bool destroyOnActivate = true;

    public UnityEvent onActivate;

    public void Activate() {
      onActivate.Invoke();
      if (destroyOnActivate)
        Destroy(this);
    }
  }

}