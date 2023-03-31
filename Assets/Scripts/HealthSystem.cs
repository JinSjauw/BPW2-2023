using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
   public event EventHandler OnDeath;
   
   [SerializeField] private int health = 100;
   
   public void Damage(int _damage)
   {
      health -= _damage;
      
      if (health <= 0)
      {
         //Die
         Die();
         health = 0;
      }
   }

   private void Die()
   {
      OnDeath?.Invoke(this, EventArgs.Empty);
   }
}
