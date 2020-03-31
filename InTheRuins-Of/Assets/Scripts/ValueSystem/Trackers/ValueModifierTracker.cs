

namespace CharacterSystem {

  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;


  public static class ValueModifierTracker {

    public static readonly Dictionary<Type, List<object>> dict;

    /// <summary>
    /// Adds the ValueModifier to the dictionary if not already added
    /// </summary>
    public static void RegisterModifier<T>(ValueModifier<T> valueModifier) {
      if (!dict.ContainsKey(typeof(T)))
        dict.Add(typeof(T), new List<object>());

      if (!dict.TryGetValue(typeof(T), out var list) && !list.Contains(valueModifier))
        list.Add(valueModifier);
    }

    /// <summary>
    /// Returns an array of registered ValueModifier<T>s  
    /// </summary>
    public static ReadOnlyCollection<ValueModifier<T>> GetModifiers<T>() {
      if (dict.TryGetValue(typeof(T), out var list)) {
        return (list as List<ValueModifier<T>>).AsReadOnly();
      }
      return new List<ValueModifier<T>>(0).AsReadOnly();
    }
  }
}
