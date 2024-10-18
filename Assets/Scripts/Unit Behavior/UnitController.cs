using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum UnitState
{
    unset,
    idling,
    approachingTarget,
    movingToLocation,
    fighting,
    dead
}

public enum Faction
{
    unset,
    Goblin,
    Human
}

public interface IUnit
{
    UnitState GetState();

    GameObject GetGameObject();

    Faction GetFaction();


}

public interface IAttackBehavior
{
    bool IsAttacking();

    void AttackTarget(Transform target);

    void CancelCurrentAttack();

}

public interface IMoveBehavior
{
    bool IsMoving();

    void MoveToLocation(Vector3 newPosition);

    void CancelCurrentMovement();
}

public interface IVisualFeedbackController
{
    void SetHoverFeedback(bool newState);
}



public class UnitController : MonoBehaviour, IUnit , IInteractable
{
    //Declarations
    [SerializeField] private Faction _faction = Faction.unset;
    [SerializeField] private UnitState _currentState = UnitState.unset;
    [SerializeField] private Transform _currentTarget;
    private IMoveBehavior _moveBehavior;
    private IAttackBehavior _attackBehavior;
    private IVisualFeedbackController _visualFeedbackController;

    public delegate void UnitControllerEvent();
    public UnitControllerEvent OnStateChanged;



    //monobehaviours
    private void Awake()
    {
        InitializeReferences();
    }

    private void OnEnable()
    {
        AddEventSubscriptions();
    }

    private void OnDisable()
    {
        RemoveEventSubscriptions();
    }




    //Internals
    private void InitializeReferences()
    {
        _moveBehavior = GetComponent<IMoveBehavior>();
        _attackBehavior = GetComponent<IAttackBehavior>();
        _visualFeedbackController = GetComponent<IVisualFeedbackController>();
    }

    private void AddEventSubscriptions()
    {
        OnStateChanged += _moveBehavior.CancelCurrentMovement;
        OnStateChanged += _attackBehavior.CancelCurrentAttack;
    }

    private void RemoveEventSubscriptions()
    {
        OnStateChanged -= _moveBehavior.CancelCurrentMovement;
        OnStateChanged -= _attackBehavior.CancelCurrentAttack;
    }




    private void DetermineActionAndStateBasedOnNewTarget(Transform newTarget)
    {
        if (newTarget != _currentTarget)
        {


            
        }
    }

    private void MoveToLocation(Vector3 location)
    {
        _moveBehavior.MoveToLocation(location);
    }

    private void UpdateState(UnitState newState)
    {
        OnStateChanged?.Invoke();
        _currentState = newState;
    }






    //Externals
    public UnitState GetState()
    {
        return _currentState;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public Faction GetFaction()
    {
        return _faction;
    }

    public void SetHoverFeedback(bool newState)
    {
        _visualFeedbackController.SetHoverFeedback(newState);
    }

    public void SetTarget(Transform newTarget)
    {
        DetermineActionAndStateBasedOnNewTarget(newTarget);
    }

    public void SetMoveOrder(Vector3 position)
    {
        UpdateState(UnitState.movingToLocation);
        MoveToLocation(position);
    }


}
