using Kirara.TimelineAction;
using UnityEngine;

namespace Kirara
{
    public class MonsterAICtrl : MonoBehaviour
    {
        public float attackDistance;
        public float chaseDistance;

        public BoxCollider boxCollider;
        public SphereCollider sphereCollider;

        private Monster monster;

        private ActionPlayer actionPlayer;

        public enum State
        {
            Idle,
            Chase, // 追击
            Attack,
            Hit, // 受击
            Stun, // 硬直
        }

        public State _state;

        public float maxIdleColdTime = 5f;
        public float idleColdTime;

        private Vector3 hitFrom;

        public KiraraActionSO Idle;
        public KiraraActionSO Run;
        public KiraraActionSO Hit_Stay;
        public KiraraActionSO Hit_L_Front;
        public KiraraActionSO Hit_L_Back;
        public KiraraActionSO Attack_01;
        public KiraraActionSO Attack_02;

        private string[] attackNames;
        private KiraraActionSO[] attackActions;

        private void Awake()
        {
            monster = GetComponent<Monster>();
            actionPlayer = GetComponent<ActionPlayer>();

            boxCollider.enabled = false;
            sphereCollider.enabled = false;
        }

        public void GetHit(Vector3 hitFrom)
        {
            this.hitFrom = hitFrom;
            if (_state == State.Idle || _state == State.Hit)
            {
                EnterState(State.Hit);
            }
        }

        private void Start()
        {
            // attackNames = new[] {ActionName.Attack_01, ActionName.Attack_02};
            // attackActions = new[] {Attack_01, Attack_02};
            //
            // EnterState(State.Idle);
        }

        private void Update()
        {
            // UpdateState();
        }

        private void UpdateState()
        {
            switch (_state)
            {
                case State.Idle:
                {
                    idleColdTime += Time.deltaTime;
                    if (idleColdTime < maxIdleColdTime) return;

                    var chPos = PlayerSystem.Instance.FrontRoleCtrl.transform.position;
                    float dis = Vector3.Distance(chPos, transform.position);
                    if (dis < attackDistance)
                    {
                        EnterState(State.Attack);
                    }
                    else if (dis < chaseDistance)
                    {
                        EnterState(State.Chase);
                    }
                    break;
                }
                case State.Chase:
                {
                    var chPos = PlayerSystem.Instance.FrontRoleCtrl.transform.position;
                    transform.LookAt(chPos);
                    float dist = Vector3.Distance(chPos, transform.position);
                    if (dist < attackDistance)
                    {
                        EnterState(State.Attack);
                    }
                    else if (dist > chaseDistance)
                    {
                        EnterState(State.Idle);
                    }
                    break;
                }
            }
        }

        private void ChooseHitActionName(out string actionName, out KiraraActionSO actionSO)
        {
            string s = Vector3.Dot(hitFrom, transform.forward) > 0
                ? ActionName.Hit_L_Front : ActionName.Hit_L_Back;
            actionName = s;
            actionSO = s == ActionName.Hit_L_Front ? Hit_L_Front : Hit_L_Back;
        }

        public void EnterState(State state)
        {
            _state = state;
            switch (state)
            {
                case State.Idle:
                {
                    idleColdTime = 0f;
                    actionPlayer.Play(Idle, ActionName.Idle, 0.15f);
                    break;
                }
                case State.Chase:
                {
                    actionPlayer.Play(Run, ActionName.Run, 0.15f);
                    break;
                }
                case State.Attack:
                {
                    var chPos = PlayerSystem.Instance.FrontRoleCtrl.transform.position;
                    transform.LookAt(chPos);
                    int i = Random.Range(0, attackNames.Length);
                    actionPlayer.Play(attackActions[i], attackNames[i], 0.15f, () =>
                    {
                        EnterState(State.Idle);
                    });
                    break;
                }
                case State.Hit:
                {
                    ChooseHitActionName(out string actionName, out var actionSO);
                    actionPlayer.Play(actionSO, actionName, 0f, () =>
                    {
                        EnterState(State.Idle);
                    });
                    break;
                }
                case State.Stun:
                {
                    // curAniState.Speed = -2f;
                    // curAniState.Events.Clear();
                    // curAniState.Events.Add(0f, () =>
                    // {
                    //     EnterState(State.Idle);
                    // });
                    break;
                }
            }
        }
    }
}