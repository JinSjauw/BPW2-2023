using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
   public event EventHandler OnDeath;
   public event EventHandler OnHealthChanged;

   [SerializeField] private int healthMax;
   [SerializeField] private int health;

   private void Awake()
   {
      //health = healthMax;
   }

   public void Damage(int _damage)
   {
      health -= _damage;
      OnHealthChanged?.Invoke(this, EventArgs.Empty);
        
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

   public float GetHealthNormalized()
   {
      return (float)health / healthMax;
   }

   public void Heal(int _amount)
   {
      health += _amount;
      OnHealthChanged?.Invoke(this, EventArgs.Empty);
   }
}
