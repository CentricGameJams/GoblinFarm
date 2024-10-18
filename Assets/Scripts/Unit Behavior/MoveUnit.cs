using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : MonoBehaviour , IMoveBehavior
{
    //Declarations
    private NavMeshAgent _navAgent;
    private bool _isMoving = false;

    private Vector3 _targetPosition;
    [SerializeField] private float _closeEnoughDistance = .5f;


    //Monobehaviours
    private void Awake()
    {
        _navAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        MoveUntilSatisfied();
    }


    //Internals
    private void MoveUntilSatisfied()
    {
        if (_isMoving) 
        {
            float remainingMagnitude = Vector3.Distance(_targetPosition, transform.position);
            Debug.Log($"remaining distance: {remainingMagnitude}");

            if (remainingMagnitude < _closeEnoughDistance)
                CancelCurrentMovement();
        }
    }




    //Externals
    public void CancelCurrentMovement()
    {
        if (_isMoving)
        {
            _navAgent.isStopped = true;
            _navAgent.ResetPath();

            _isMoving = false;
            _targetPosition = transform.position;
        }
    }

    public bool IsMoving()
    {
        return _isMoving;
    }

    public void MoveToLocation(Vector3 newPosition)
    {
        if (!_isMoving)
        {
            //returns false if the location isn't valid
            _isMoving = _navAgent.SetDestination(newPosition);

            if (_isMoving)
                _targetPosition = newPosition;
        }

        else
        {
            CancelCurrentMovement();
            MoveToLocation(newPosition);
        }
        
    }
}
