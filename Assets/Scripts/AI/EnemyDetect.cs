using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Unit))]
public class EnemyDetect : MonoBehaviour
{
    [SerializeField] private float detectRadius;
    private LayerMask targetLayers;
    private bool hasDetected = false;
    private Unit unit;
    private Unit hitUnit;
    
    void Start()
    {
        LayerMask layerPlayer = LayerMask.GetMask("Player");

        unit = GetComponent<Unit>();
        targetLayers = layerPlayer;
    }

    // Update is called once per frame
    void Update()
    {
        //Check if player comes close

        if (unit.GetState() != Unit.UnitState.IDLE)
        {
            if (!hasDetected)
            {
                return;
            }
            
            Vector3 origin = unit.transform.position;
            Vector3 target = hitUnit.transform.position;

            float distance = Vector3.Distance(origin, target);
            
            if (distance > detectRadius)
            {
                unit.SetState(Unit.UnitState.IDLE);
                hitUnit.SetState(Unit.UnitState.IDLE);
                hitUnit = null;
                hasDetected = false;
            }
            
            return;
        }
        
        Collider[] hits = Physics.OverlapSphere(transform.position, detectRadius, targetLayers);
        if(hits.Length > 0)
        {
            foreach (var col in hits)
            {
                hitUnit = col.transform.root.GetComponent<Unit>();
                if (hitUnit == null)
                {
                    continue;
                }
                
                if (!hitUnit.IsEnemy())
                {
                    if (!hasDetected)
                    {
                        //Check if there is a wall between objects
                        Vector3 target = hitUnit.transform.position;
                        Vector3 origin = unit.transform.position;
                        Vector3 direction = new Vector3(target.x - origin.x, target.y - origin.y);

                        if (!Physics.Raycast(unit.transform.position, direction,
                                Vector3.Distance(origin, target), LayerMask.GetMask("Walls")))
                        {
                            hasDetected = true;
                            hitUnit.SetState(Unit.UnitState.COMBAT);
                            unit.SetState(Unit.UnitState.COMBAT);
                        }
                    }
                }
            }
        }
    }
}
