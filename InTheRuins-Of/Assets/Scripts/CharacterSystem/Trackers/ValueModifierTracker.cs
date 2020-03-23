

namespace CharacterSystem {

  using System;
  using System.Collections;
  using System.Collections.Generic;


  public static class ValueModifierTracker {

    public static readonly List<ValueModifier<dynamic>> list;

    /// <summary>
    /// Adds the ValueModifier to the list if not already added
    /// </summary>
    /// <param name="valueModifier"></param>
    public static void RegisterModifier(ValueModifier<dynamic> valueModifier) {
      if (!list.Contains(valueModifier))
        list.Add(valueModifier);
    }
  }
}
