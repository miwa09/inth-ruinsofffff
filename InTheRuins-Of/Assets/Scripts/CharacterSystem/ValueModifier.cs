

namespace CharacterSystem {

  using System;
  using System.Collections;
  using System.Collections.Generic;


  public class ValueModifier<T> {

    public readonly Type type = typeof(T);

    public readonly string name;
    public readonly Func<T, T> function;

    protected void Register() {
      ValueModifierTracker.RegisterModifier(this as ValueModifier<dynamic>);
    }
  }
}
