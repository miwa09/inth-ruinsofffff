
namespace ValueSystem {

  using System;
  using System.Linq;
  using System.Collections;
  using System.Collections.Generic;

  public abstract class Value<T> : IEnumerable<Modifier<T>> {

    public readonly ValueData vd;
    private readonly List<Modifier<T>> modifiers = new List<Modifier<T>>();
    private readonly List<Type> types;

    public Value(ValueData vd) {
      this.vd = vd;
      types = vd.GetModifiers<T>();
    }

    public void AddModifier(Modifier<T> modifier) {

      int pos = types.IndexOf(modifier.GetType());

      for (int i = 0; i < modifiers.Count; i++) {
        var other = modifiers[i];
        if (types.IndexOf(other.GetType()) <= pos) {
          modifiers.Insert(i, modifier);
          return;
        }
      }
      modifiers.Add(modifier);
    }

    public Modifier<T> FindModifier<TModifier>() {
      return modifiers.Find(v => v.GetType() == typeof(TModifier));
    }

    IEnumerator<Modifier<T>> IEnumerable<Modifier<T>>.GetEnumerator() => modifiers.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => modifiers.GetEnumerator();

    private int GetModifierPos(Modifier<T> modifier) {
      return types.IndexOf(modifier.GetType());
    }
  }
}
