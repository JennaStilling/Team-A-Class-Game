using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class CheckEnemyInFOVRange : Node
{
    //private static int _enemyLayerMask = 1 << 6;
    private static ContactFilter2D _enemyLayerMask = new ContactFilter2D();

    private Transform _transform;
    private Animator _animator;

    public void Awake()
    {
        _enemyLayerMask.SetLayerMask(6);
        _enemyLayerMask.useLayerMask = true;
    }

    public CheckEnemyInFOVRange(Transform transform)
    {
        _transform = transform;
        _animator = transform.GetComponent<Animator>();
    }

    public override NodeState Evaluate()
    {
        object t = GetData("target");
        if (t == null)
        {
            List<Collider2D> colliders = new List<Collider2D>();
            int length = Physics2D.OverlapCircle(
                _transform.position, EnemyBT.fovRange, _enemyLayerMask, colliders);

            if (length > 0)
            {
                for (int i = 0; i < length; i++)
                {
                    Debug.Log(colliders[i].name);
                    if (colliders[i].name == "Player")
                    {
                        parent.parent.SetData("target", colliders[length - 1].transform);
                        //_animator.SetBool("Walking", true);
                        state = NodeState.SUCCESS;
                        return state;
                    }
                }
            }

            state = NodeState.FAILURE;
            return state;
        }

        state = NodeState.SUCCESS;
        return state;
    }

}