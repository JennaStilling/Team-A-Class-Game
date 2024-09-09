using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class EmployeeTaskAnswerTheCall : Node
{
    private Transform _transform;
    private Animator _animator; // to add later
    private string _targetKey;
    private object _targetValue;

    public EmployeeTaskAnswerTheCall(Transform transform, string targetKey, object targetValue )
    {
        _transform = transform;
        _targetKey = targetKey;
        _targetValue = targetValue;
        // initialize animator
    }

    public override NodeState Evaluate()
    {
        SetData(_targetKey, _targetValue);
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