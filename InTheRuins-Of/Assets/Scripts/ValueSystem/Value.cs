

namespace CharacterSystem {

  using System;
  using System.Collections;
  using System.Collections.Generic;
  using UnityEngine;

  using MUC.Inspector;


  public abstract class Value<T> : ScriptableObject {

    public Type type = typeof(T);

    public new abstract string name { get; }

    public abstract List<ValueModifier<T>> modifiers { get; }

    [Button]
    public void AddCompatibleModifiers() {
      foreach (var modifier in ValueModifierTracker.GetModifiers<T>()) {
        if (!modifiers.Contains(modifier)) {
          modifiers.Add(modifier);
        }
      }
    }
  }
}
