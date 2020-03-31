

#if UNITY_EDITOR
namespace ModifiableValue {

  using UnityEngine;
  using UnityEditor;
  using System;
  using System.Linq;
  using System.Collections;
  using System.Collections.Generic;


  [CustomEditor(typeof(ValueController))]
  public class ValueControllerEditor : Editor {

    public override void OnInspectorGUI() {
      serializedObject.Update();

      DrawDefaultInspector();

      ValueController target = (ValueController)serializedObject.targetObject;

      foreach (var orderData in target.orders) {

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField(orderData.generic);

        var modifiers = orderData.modifiers;
        for (int i = 0; i < modifiers.Count; i++) {
          GUILayout.BeginHorizontal();

          EditorGUILayout.LabelField(modifiers[i]);

          if (i != 0 && GUILayout.Button("Up")) {
            var temp = modifiers[i];
            modifiers[i] = modifiers[i - 1];
            modifiers[i - 1] = temp;
            target.Validate();
          }
          if (i < modifiers.Count - 1 && GUILayout.Button("Down")) {
            var temp = modifiers[i];
            modifiers[i] = modifiers[i + 1];
            modifiers[i + 1] = temp;
            target.Validate();
          }

          GUILayout.EndHorizontal();
        }
      }

      if (GUILayout.Button("Refresh")) {
        target.Validate();
      }

      serializedObject.ApplyModifiedProperties();
    }
  }
}
#endif


namespace ModifiableValue {

  using System;
  using System.Linq;
  using System.Reflection;
  using System.Collections;
  using System.Collections.Generic;
  using UnityEngine;
  using MUC.Inspector;

  [System.Serializable]
  public class OrderList : List<OrderData> {
    public OrderData GetByGenericName(string generic) {
      var res = this.Find(v => v.generic == generic);
      if (res == null) {
        this.Add(new OrderData(generic));
        return this.Last();
      }
      return res;
    }
  }

  [System.Serializable]
  public class OrderData {
    public readonly string generic;
    public readonly List<string> modifiers = new List<string>();
    public OrderData(string generic) {
      this.generic = generic;
    }
  }

  [CreateAssetMenu(menuName = "ValueController")]
  public class ValueController : ScriptableObject {

    // Key: Generic type of modifier
    // Value: List of modifiers with that generic type
    public Dictionary<Type, List<Type>> types = new Dictionary<Type, List<Type>>();

    public OrderList orders = new OrderList();

    void Awake() => Validate();
    void OnValidate() => Validate();

    public void Validate() {
      RefreshDictionary();
      AddMissingNames();
      RemoveUnknownNames();
      RefreshOrders();
    }

    public List<Type> GetModifiers<T>() {
      var generic = typeof(T);
      if (types.TryGetValue(generic, out var modifiers))
        return modifiers;
      else
        return types[generic] = new List<Type>();
    }

    public void AddMissingNames() {
      foreach (var kv in types) {
        var order = orders.GetByGenericName(kv.Key.FullName);
        foreach (var modifiers in kv.Value) {
          var name = modifiers.FullName;
          if (!order.modifiers.Contains(name)) {
            order.modifiers.Insert(0, name);
          }
        }
      }
    }

    public void RemoveUnknownNames() {
      for (int i = 0; i < orders.Count; i++) {
        var order = orders[i];
        if (!types.Keys.Any(k => k.FullName == order.generic)) {
          orders.RemoveAt(i--);
        } else {
          for (int j = 0; j < order.modifiers.Count; j++) {
            var modifierName = order.modifiers[j];

            var key = types.Keys.Single(k => k.FullName == order.generic);
            var modifierTypes = types[key];

            if (!modifierTypes.Any(v => v.FullName == modifierName)) {
              order.modifiers.RemoveAt(j--);
            }
          }
        }
      }
    }

    public void RefreshOrders() {
      foreach (var kv in types.AsEnumerable()) {
        Type generic = kv.Key;
        string genericName = generic.FullName;
        IEnumerable<Type> list = kv.Value;
        List<string> names = list.Select(v => v.FullName).ToList();
        list.OrderBy(t => names.IndexOf(t.FullName));
      }
    }

    public void RefreshDictionary() {
      var types = GetModifierTypes();

      foreach (var type in types) {
        var generic = type.BaseType.GenericTypeArguments[0];

        if (this.types.TryGetValue(generic, out var modifiers)) {
          if (!modifiers.Contains(type)) modifiers.Add(type);
        } else {
          this.types[generic] = new List<Type>() { type };
        }
      }
    }

    private static IEnumerable<Type> GetModifierTypes() {
      var modifierT = typeof(Modifier<>);
      var assembly = modifierT.Assembly;
      var types = assembly.GetTypes();
      foreach (var type in types) {
        if (type.IsClass) {
          if (!type.IsAbstract) {
            if (type.BaseType.IsGenericType) {
              if (type.BaseType.GetGenericTypeDefinition() == modifierT) {
                yield return type;
              }
            }
          }
        }
      }
    }
  }

  public class StringModifierTrump : Modifier<string> { public StringModifierTrump(ValueController ctrl) : base(ctrl) { } }
  public class StringModifierHarbringer : Modifier<string> { public StringModifierHarbringer(ValueController ctrl) : base(ctrl) { } }
  public class StringModifierSource : Modifier<string> { public StringModifierSource(ValueController ctrl) : base(ctrl) { } }
  public class StringModifierCadence : Modifier<string> { public StringModifierCadence(ValueController ctrl) : base(ctrl) { } }

  public class IntModifier1 : Modifier<int> {
    public IntModifier1(ValueController ctrl) : base(ctrl) { }
  }
  public class FloatModifier1 : Modifier<float> {
    public FloatModifier1(ValueController ctrl) : base(ctrl) { }
  }

  public abstract class Modifier<T> {

    public readonly ValueController ctrl;
    public readonly List<Type> modifiers;

    public Modifier(ValueController ctrl) {
      this.ctrl = ctrl;
      modifiers = ctrl.GetModifiers<T>();
    }
  }


  abstract class Value<T> {

    public readonly ValueController ctrl;
    public readonly List<Type> modifiers;

    public Value(ValueController ctrl) {
      this.ctrl = ctrl;
      modifiers = ctrl.GetModifiers<T>();
    }
  }

}