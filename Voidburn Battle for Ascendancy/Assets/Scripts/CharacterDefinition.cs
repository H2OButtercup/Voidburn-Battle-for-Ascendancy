using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "CharacterDefinition", menuName = "Scriptable Objects/CharacterDefinition")]
public class CharacterDefinition : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string id;
    [SerializeField] private string displayName;
    [SerializeField] private Sprite portrait;
    [SerializeField] private GameObject prefab;

    [Header("Stats")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float jumpForce = 8f;

    [Header("Combos")]
    [SerializeField] private List<SpecialCombo> specialCombos = new();

    public string Id => id;
    public string DisplayName => displayName;
    public Sprite Portrait => portrait;
    public GameObject Prefab => prefab;
    public float MaxHealth => maxHealth;
    public float MoveSpeed => moveSpeed;
    public float JumpForce => jumpForce;
    public List<SpecialCombo> SpecialCombos => specialCombos;

    [Serializable]
    public class SpecialCombo
    {
        [SerializeField] private string name;
        [SerializeField] private List<KeyCode> sequence = new();
        [SerializeField] private float damage = 20f;
        [SerializeField] private float cooldownSeconds = 2f;

        public string Name => name;
        public List<KeyCode> Sequence => sequence;
        public float Damage => damage;
        public float CooldownSeconds => cooldownSeconds;
    }
}