using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class EmployeeTaskAttack : Node
{
    private Animator _animator; // tbd - see animation comments below

    private Transform _lastTarget;
    private EnemyManager _enemyManager;

    private float _attackTime = 1f;
    private float _attackCounter = 0f;

    public EmployeeTaskAttack(Transform transform)
    {
        // initialize animator
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("target");
        if (target != _lastTarget)
        {
            _enemyManager = target.GetComponent<EnemyManager>();
            _lastTarget = target;
        }
        Debug.Log("Attacking");
        _attackCounter += Time.deltaTime;
        if (_attackCounter >= _attackTime)
        {
            bool enemyIsDead = _enemyManager.TakeDamage();
            if (enemyIsDead)
            {
                ClearData("target");
                // set animation - walking, not attacking
            }
            else
            {
                _attackCounter = 0f;
            }
        }

        state = NodeState.RUNNING;
        return state;
    }

}