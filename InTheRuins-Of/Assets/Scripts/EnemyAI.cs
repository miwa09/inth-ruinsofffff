

namespace global {

  using System.Collections;
  using System.Collections.Generic;

  using UnityEngine;
  using UnityEngine.AI;
  using ValueComponents;

  [RequireComponent(typeof(NavMeshAgent))]
  [RequireComponent(typeof(Health))]
  public class EnemyAI : MonoBehaviour {

    #region properties
    [SerializeField] Animator animator;
    GameObject target;
    NavMeshAgent agent;

    public LayerMask visionMask;

    public float animationSpeedMultiplier = 1;

    public float sightRange = 10;
    public float attackRange = 1;
    public float damage = 10;

    #region state
    float stateStart;
    float stateTime => Time.time - stateStart;

    State _state = State.idle;

    public State state {
      get => _state;
      set {
        if (_state == value) return;
        if (_state == State.dead) return;

        stateStart = Time.time;

        agent.isStopped = false;

        print($"State set to {value}");

        switch (value) {
          case State.idle:
            agent.isStopped = true;
            animator.SetTrigger("Idle");
            break;
          case State.chase:
            animator.SetTrigger("Move");
            break;
          case State.attack:
            agent.isStopped = true;
            var hp = target.GetComponent<Health>();
            hp.AddToValue(-damage);
            animator.SetTrigger("Attack");
            break;
          case State.search:
            animator.SetTrigger("Move");
            agent.destination = targetPos;
            break;
          case State.dead:
            agent.isStopped = true;
            animator.SetTrigger("Death");
            Destroy(this);
            break;
        }

        _state = value;
      }
    }
    public enum State {
      idle,
      chase,
      attack,
      search,
      dead,
    }
    #endregion

    Vector3 pos => transform.position;
    Vector3 targetPos => target.transform.position;
    #endregion


    #region Func
    void Start() {
      animator = animator ?? GetComponent<Animator>();
      agent = GetComponent<NavMeshAgent>();
      target = target ?? GameObject.FindWithTag("Player");
      GetComponent<Health>().onDeath.AddListener((health) => { state = State.dead; });
    }

    void Update() {
      UpdateState();
      animator.SetFloat("Speed", agent.velocity.magnitude * animationSpeedMultiplier);
    }

    void UpdateState() {
      var dist = Vector3.Distance(pos, targetPos);

      switch (state) {
        case State.idle:
          if (HasVision()) state = State.chase;
          break;

        case State.chase:
          agent.destination = targetPos;
          if (dist < attackRange) state = State.attack;
          else if (dist > sightRange) state = State.search;
          break;

        case State.attack:
          if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")) {
            state = State.search;
          }
          break;

        case State.search:
          if (HasVision()) state = State.chase;
          else if (agent.isStopped) state = State.idle;
          break;

        case State.dead:
          // Stay dead
          break;
      }
    }


    bool HasVision() {
      var dist = Vector3.Distance(pos, targetPos);
      return dist <= sightRange && !Physics.Raycast(pos, targetPos - pos, dist, visionMask);
    }

    void OnDrawGizmosSelected() {
      Gizmos.color = Color.yellow;
      Gizmos.DrawWireSphere(pos, sightRange);
      Gizmos.color = Color.red;
      Gizmos.DrawWireSphere(pos, attackRange);
    }
    #endregion
  }

}
