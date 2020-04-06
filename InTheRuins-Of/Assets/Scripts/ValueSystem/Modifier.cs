
namespace ValueSystem {

  using System;
  using System.Collections.Generic;

  public abstract class Modifier<T> {

    public abstract T Modify(T current, T original);

    public Modifier() { }
  }

  // Does nothing pog
  public abstract class StaticModifier<T> : Modifier<T> {
    public override T Modify(T current, T original) => current;
  }
}