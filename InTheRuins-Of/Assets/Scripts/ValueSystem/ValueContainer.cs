

namespace CharacterSystem {

  using UnityEngine;


  [System.Serializable]
  public class ValueContainer : MonoBehaviour {

    [SerializeField]
    public Value<Object>[] values = new Value<Object>[0];
  }
}


#if UNITY_EDITOR
namespace CharacterSystem {

  using UnityEditor;

  [CustomEditor(typeof(ValueContainer))]
  [CanEditMultipleObjects]
  public class ValueContainerEditor : Editor {

    SerializedProperty values;

    void OnEnable() {
      values = serializedObject.FindProperty(nameof(values));
    }

    public override void OnInspectorGUI() {
      serializedObject.Update();

      EditorGUILayout.PropertyField(values);

      serializedObject.ApplyModifiedProperties();
    }
  }
}
#endif
