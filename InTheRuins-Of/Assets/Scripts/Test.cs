using System;
using System.Collections.Generic;
using UnityEngine;
using Muc.Inspector;
using ValueSystem;

public class Test : MonoBehaviour {

  public ValueData vd;

  public class IntValue : Value<int> {
    public IntValue(ValueData vd) : base(vd) { }
  }

  public Value<int> value;

  [Button]
  public void CreateValue() {
    value = new IntValue(vd);
  }

  [Button]
  public void AddTrump() {
    value.AddModifier(new IntModifierTrump());
  }

  [Button]
  public void AddModifierHarbinger() {
    value.AddModifier(new IntModifierHarbringer());
  }
  [Button]
  public void AddModifierSource() {
    value.AddModifier(new IntModifierSource());
  }

  [Button]
  public void PrintModifiers() {
    foreach (var modifier in value) {
      print(modifier.GetType());
    }
  }
}
