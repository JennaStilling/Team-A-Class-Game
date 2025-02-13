using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class EmployeeCheckEnemyInAttackRange : Node
{
    private Transform _transform;
    private Animator _animator;

    public EmployeeCheckEnemyInAttackRange(Transform transform)
    {
        _transform = transform;
        // initialize animator
    }

    public override NodeState Evaluate()
    {
        Debug.Log("In check enemy attack range task");
        object t = GetData("target");
        if (t == null)
        {
            Debug.Log("t was null");
            state = NodeState.FAILURE;
            return state;
        }

        Transform target = (Transform)t;
        Debug.Log(Vector3.Distance(_transform.position, target.position));
        if (Vector3.Distance(_transform.position, target.position) <= EmployeeBT.attackRange)
        {
            Debug.Log("In attack range"); // DONT REACH HERE YET
            // set animations - attacking & not walking
            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }

}