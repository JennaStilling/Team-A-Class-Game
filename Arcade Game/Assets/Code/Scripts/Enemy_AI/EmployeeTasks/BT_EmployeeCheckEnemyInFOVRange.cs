using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class EmployeeCheckEnemyInFOVRange : Node
{
    private static int _enemyLayerMask = 1 << 6;
    //private static ContactFilter2D _enemyLayerMask = new ContactFilter2D();

    private Transform _transform;
    private Animator _animator; // tbd later - see commented areas below

   /* public void Awake()
    {
        _enemyLayerMask.SetLayerMask(6); // player needs to be on layer six
        _enemyLayerMask.useLayerMask = true;
    }*/

    public EmployeeCheckEnemyInFOVRange(Transform transform)
    {
        _transform = transform;
        // initialize animator
    }

    public override NodeState Evaluate()
    {
        Debug.Log("Evaluating if enemy in range");
        object t = GetData("target");
        if (t == null)
        {
            Collider[] colliders = Physics.OverlapSphere(
                _transform.position, EmployeeBT.fovRange, _enemyLayerMask);

            if (colliders.Length > 0)
            {
                parent.parent.SetData("target", colliders[0].transform);
                // set animation - walking
                state = NodeState.SUCCESS;
                return state;
            }

            state = NodeState.FAILURE;
            return state;
        }

        state = NodeState.SUCCESS;
        return state;
    }

}