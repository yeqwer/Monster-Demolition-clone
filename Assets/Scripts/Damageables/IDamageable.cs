using System;

public interface IDamageable
{
    public int MaxHealth { get; }

    public int CurrentHealth { get; }

    public event Action OnDestroyed;

    public void Damage(int damage);
}
