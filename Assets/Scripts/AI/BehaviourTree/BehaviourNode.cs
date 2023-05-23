using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class BehaviourNode : ScriptableObject
{
   public enum BehaviourState
   {
      Running,
      Failure,
      Success
   }

   [HideInInspector] public BehaviourState behaviourState = BehaviourState.Running;
   [HideInInspector] public bool started = false;
   [HideInInspector] public string guid;
   [HideInInspector] public Vector2 position;
   [HideInInspector] public Blackboard blackboard;
   [HideInInspector] public Unit unit;
   [TextArea] public string description;
   public BehaviourState Update()
   {
      if (!started)
      {
         OnStart();
         started = true;
      }

      behaviourState = OnUpdate();

      if (behaviourState == BehaviourState.Failure || behaviourState == BehaviourState.Success)
      {
         OnStop();
         started = false;
      }

      return behaviourState;
   }
   
   public virtual BehaviourNode Clone()
   {
      return Instantiate(this);
   }

   public virtual void Init(){}
   
   protected abstract void OnStart();
   protected abstract void OnStop();
   protected abstract BehaviourState OnUpdate();

}
