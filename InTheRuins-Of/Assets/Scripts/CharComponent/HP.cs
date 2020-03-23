using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using MUC.Collections;
using MUC.Inspector;

namespace CharacterComponentSystem {
  public class HP : MonoBehaviour {

    [SerializeField]
    protected float _health = 1;
    public float health { get => _health; protected set => _health = value; }
    public float prevHealth { get; protected set; }

    [Tooltip("Invoked in LateUpdate if health was changed. HP")]
    public OnChangeEvent onChange;
    [System.Serializable] public class OnChangeEvent : UnityEvent<HP> { }

    [Tooltip("Invoked after taking damage. HP, Damage, Modified damage")]
    public OnDamageEvent onDamage;
    [System.Serializable] public class OnDamageEvent : UnityEvent<HP, float, float> { }

    [Tooltip("Invoked after any heal. HP, Heal, Modified heal")]
    public OnHealEvent onHeal;
    [System.Serializable] public class OnHealEvent : UnityEvent<HP, float, float> { }

    [Tooltip("Invoked after health being set. HP, Value, Modified value")]
    public OnSetEvent onSet;
    [System.Serializable] public class OnSetEvent : UnityEvent<HP, float, float> { }


    public ModifierList<float> damageModifiers = new ModifierList<float>();
    public ModifierList<float> healModifiers = new ModifierList<float>();
    public ModifierList<float> setModifiers = new ModifierList<float>();

    public static implicit operator float(HP hp) => hp.health;

    void Start() {
      prevHealth = _health;
    }

    void LateUpdate() {
      if (prevHealth == health) return;
      onChange.Invoke(this);
      prevHealth = health;
    }

    [Button()]
    /// <summary> Substracts `damage` from health </summary>
    public void Damage(float damage) {
      var mod = damageModifiers.Apply(damage);
      health -= mod;
      onDamage.Invoke(this, damage, mod);
    }

    [Button(1)]
    /// <summary> Adds `healing` to health </summary>
    public void Heal(float healing) {
      var mod = healModifiers.Apply(healing);
      health += mod;
      onHeal.Invoke(this, healing, mod);
    }

    [Button(10)]
    /// <summary> Sets the value of health to `value` </summary>
    public void Set(float value) {
      var mod = setModifiers.Apply(value);
      health = mod;
      onSet.Invoke(this, value, mod);
    }
  }
}