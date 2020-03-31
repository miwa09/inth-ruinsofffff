

namespace CharacterSystem {

  using System;
  using System.Collections;
  using System.Collections.Generic;
  using UnityEngine;

  using MUC.Inspector;

  [CreateAssetMenu(menuName = "Value/Health")]
  public class HealthValue : Value<float> {
    public override List<ValueModifier<float>> modifiers { get; } = new List<ValueModifier<float>>();

    public override string name { get; } = "Health";

  }
}
