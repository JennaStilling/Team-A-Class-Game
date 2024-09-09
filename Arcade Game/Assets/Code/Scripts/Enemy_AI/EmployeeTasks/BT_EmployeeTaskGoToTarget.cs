using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class TaskGoToTarget : Node
{
    private Transform _transform;
    private Animator _animator;

    public TaskGoToTarget(Transform transform)
    {
        _transform = transform;
        // initialize animator
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("target");

        if (Vector2.Distance(_transform.position, target.position) > 0.01f)
        {
            _transform.position = Vector3.MoveTowards(
                _transform.position, new Vector3(target.position.x, _transform.position.y), EmployeeBT.speed * Time.deltaTime);
            _transform.LookAt(target.position);
            // set animation - walking
        }

        state = NodeState.RUNNING;
        return state;
    }

}