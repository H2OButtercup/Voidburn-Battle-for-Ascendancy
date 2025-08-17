using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth = 100f;
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private CharacterDefinition def;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public float MoveSpeed => moveSpeed;
    public float JumpForce => jumpForce;
    public CharacterDefinition Definition => def;

    public void Apply(CharacterDefinition definition)
    {
        def = definition;
        maxHealth = definition.MaxHealth;
        currentHealth = MaxHealth;
        moveSpeed = definition.MoveSpeed;
        jumpForce = definition.JumpForce;
    }
}
