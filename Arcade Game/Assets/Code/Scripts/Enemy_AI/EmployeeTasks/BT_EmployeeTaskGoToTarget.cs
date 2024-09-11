using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;
using UnityEngine.AI;

public class TaskGoToTarget : Node
{
    private Transform _transform;
    private Animator _animator;
    private EmployeeEnemyManager _selfManager;
    private NavMeshAgent _navMeshAgent;
    

    public TaskGoToTarget(Transform transform)
    {
        _transform = transform;
        // initialize animator
        _selfManager = _transform.GetComponent<EmployeeEnemyManager>();
        _navMeshAgent = _selfManager.GetComponent<NavMeshAgent>();
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("target");

        if (Vector2.Distance(_transform.position, target.position) > 1f)
        {
            _navMeshAgent.destination = target.position;
            // _transform.position = Vector3.MoveTowards(
            //     _transform.position, new Vector3(target.position.x, target.position.y, target.position.z), EmployeeBT.speed * Time.deltaTime);
            // _transform.LookAt(target.position);
            // set animation - walking
        }

        state = NodeState.RUNNING;
        return state;
    }

}