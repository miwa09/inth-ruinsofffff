

namespace CharacterSystem {

  using System;
  using System.Collections;
  using System.Collections.Generic;

  [Serializable]
  public abstract class ValueModifier<T> {

    public readonly Type type = typeof(T);

    public readonly string name;
    public readonly Func<T, T, T> function;

    protected void Register() {
      ValueModifierTracker.RegisterModifier(this);
    }
  }
}
