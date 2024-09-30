using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class EmployeeTaskAttack : Node
{
    private Animator _animator; // tbd - see animation comments below

    private Transform _lastTarget;
    private PlayerMovement _enemyManager;

    private float _attackTime = 1f;
    private float _attackCounter = 0f;

    private Transform _transform;

    public EmployeeTaskAttack(Transform transform)
    {
        //Debug.Log("Reached attack task");
        _transform = transform;
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("target");
        if (target != _lastTarget)
        {
            _enemyManager = target.GetComponent<PlayerMovement>();
            Debug.Log(_enemyManager);
            _lastTarget = target;
        }

        Debug.Log("Attacking");
        _attackCounter += Time.deltaTime;
        if (_attackCounter >= _attackTime)
        {
            // set animation attacking
            _enemyManager.TakeDamage(_transform.GetComponent<EmployeeEnemyManager>().damage);
            Debug.Log("Enemy attacking player");
            bool enemyIsDead;

            if (_enemyManager.CurrentHealthProp <= 0)
                enemyIsDead = true;
            else
            {
                enemyIsDead = false;
            }
            
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