using UnityEngine;

public class MoveAction : BaseAction
{
    private Vector3 targetPosition;
    private UnitData unitData;
    private string moveType = "";
    private bool isExecuting = false;
    
    public override void SetUnit(Unit _unit, object _obj)
    {
        unit = _unit;
        unitData = _unit.GetUnitData();
        targetPosition = LevelGrid.Instance.GetTargetGridPosition((Vector3)_obj);
    }

    public override void Execute()
    {
        if (unit == null)
        {
            Debug.Log($"MoveAction unit is null {this}");
        }
        //Move & look at targetPosition
        Vector3 moveDirection = (targetPosition - unit.transform.position).normalized;
        float distance = Vector3.Distance(targetPosition, unit.transform.position);

        if (distance >= 6 && !isExecuting)
        {
            moveType = "isRunning";
        }
        else if (!isExecuting)
        {
            moveType = "isWalking";
        }

        if (distance > unitData.stoppingDistance)
        {
            isExecuting = true;
            unitData.unitAnimator.SetBool(moveType, true);
            unit.transform.position += moveDirection * unitData.moveSpeed * Time.deltaTime;    
            
            //unit.transform.forward = Vector3.Lerp(unit.transform.forward, targetPosition, unitData.rotateSpeed * Time.deltaTime);
            unit.transform.rotation = Quaternion.RotateTowards(unit.transform.rotation,
                Quaternion.LookRotation(moveDirection), Time.deltaTime * unitData.rotateSpeed);
        }
        else
        {
            isExecuting = false;
            unitData.isActive = false;
            unitData.unitAnimator.SetBool(moveType, false);
        }
    }
}
