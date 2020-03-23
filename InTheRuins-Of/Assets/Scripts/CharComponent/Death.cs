using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using MUC.Inspector;


namespace CharacterComponentSystem {
  public class Death : MonoBehaviour {

    [SerializeField]
    protected bool _dead = false;
    public bool dead {
      get => _dead;
      protected set {
        if (_dead == value) return;
        if (value) onDeath.Invoke(gameObject);
        else onRevive.Invoke(gameObject);
        _dead = value;
      }
    }

    public HP hp;

    [Tooltip("Negate healing when dead")]
    public bool restrictHealing = true;

    public OnDeathStateChangeEvent onDeath;
    public OnDeathStateChangeEvent onRevive;

    [System.Serializable]
    public class OnDeathStateChangeEvent : UnityEvent<GameObject> { }

    // Start is called before the first frame update
    void Start() {
      if (hp == null) hp = GetComponent<HP>();
      if (hp == null) throw new UnityException("No health component specified and none found on the GameObject");
      hp.onChange.AddListener(CheckDeath);
    }

    void CheckDeath(HP hp) {
      if (!dead && hp <= 0) Kill();
    }

    [Button]
    /// <summary> Kills the unit if alive. Health is set to 0 if it is not negative </summary>
    /// <returns> Whether or not the unit was killed. False if already dead </returns>
    public bool Kill() {
      if (!dead) {
        if (hp.health > 0) hp.Set(0);
        dead = true;
        onDeath.Invoke(gameObject);
        return true;
      } else {
        return false;
      }
    }

    [Button]
    /// <summary> Resurrects the unit if dead. Health is set to 1 if it is not positive </summary>
    /// <returns> Whether or not the unit was resurrected. False if already alive </returns>
    public bool Resurrect() {
      if (dead) {
        if (hp.health <= 0) hp.Set(1);
        dead = false;
        onRevive.Invoke(gameObject);
        return true;
      } else {
        return false;
      }
    }
  }
}