using System;
using UnityEngine;

public class Damageable : MonoBehaviour, IDamageable
{
    [SerializeField]
    private int _maxHealth;

    [SerializeField]
    private int _currentHealth;

    [field: SerializeField]
    protected float _destroyAtHealthRatio { get; private set; }

    public int MaxHealth
    {
        get
        {
            return _maxHealth;
        }
        set
        {
            if (value <= 0)
            {
                _currentHealth = 0;
            }
            else
            {
                _maxHealth = value;
            }
        }
    }

    public int CurrentHealth
    {
        get
        {
            return _currentHealth;
        }
        set
        {
            if (100 * value < 70 * _maxHealth)
            {
                _currentHealth = 0;
                OnDestroyed?.Invoke();
                OnDestroyedCallback();
            }
            else
            {
                _currentHealth = value;
            }
        }
    }

    public event Action OnDestroyed;

    public virtual void CurrentEqualsMax()
    {
        _currentHealth = _maxHealth;
    }

    public virtual void Damage(int damage)
    {
        CurrentHealth -= damage;
    }

    public virtual void SetCurrentHealth(int value)
    {
        CurrentHealth = value;
    }

    protected virtual void OnDestroyedCallback()
    {
        Destroy(gameObject);
    }

#if UNITY_EDITOR
    [ContextMenu("Destroy")]
#endif
    protected void DestroySelf()
    {
        Damage(MaxHealth);
    }
}
