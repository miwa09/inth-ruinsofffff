

namespace CharacterSystem {

  using System;
  using System.Collections;
  using System.Collections.Generic;
  using UnityEngine;

  using MUC.Inspector;


  public class Value : ScriptableObject {

    public Type type = typeof(float);

    public List<ValueModifier<dynamic>> modifiers;

    [Button]
    public void AddCompatibleModifiers() {
      foreach (var modifier in ValueModifierTracker.list) {
        if (modifier.type == type) {
          if (!modifiers.Contains(modifier)) {
            modifiers.Add(modifier);
          }
        }
      }
    }
  }
}
