using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[RequireComponent(typeof(CharacterStats))]

public class FighterController : MonoBehaviour
{
    [SerializeField] private bool isCPU = false;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform feet;

    private CharacterStats stats;
    private float _lastGroundedTime;
    private bool _isGrounded;
    private readonly List<KeyCode> _inputBuffer = new();
    private float _combowWindow = 0.6f;
    private float _lastInputTime;

    public bool IsCPU => isCPU;

    private void Awake()
    {
        stats = GetComponent<CharacterStats>();
        if (!rb) rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _isGrounded = Physics.Raycast(feet.position, Vector3.down, out _, 0.2f, groundMask);
        if (_isGrounded) _lastGroundedTime = Time.time;

        if (isCPU) { AILogic(); return; }

        HandlePlayerInput();
    }

    public void SetCPU(bool value) => isCPU = value;

    public void HandlePlayerInput()
    {
        float h = (Input.GetKey(KeyCode.D) ? 1f : 0f) + (Input.GetKey(KeyCode.A) ? -1f : 0f);
        rb.linearVelocity = new Vector3(h * stats.MoveSpeed, rb.linearVelocity.y, 0);

        if (Input.GetKeyDown(KeyCode.Space) && (Time.time - _lastGroundedTime) < 0.1f)
            rb.angularVelocity = new Vector3(rb.angularVelocity.x, stats.JumpForce, 0);

        if (Input.GetKeyDown(KeyCode.J)) RegisterInput(KeyCode.J);
        if (Input.GetKeyDown(KeyCode.K)) RegisterInput(KeyCode.K);

        TryResolveCombo();
    }

    private void AILogic()
    {
        float dir = Mathf.Sin(Time.time * 0.5f) > 0 ? 1f : -1f;
        rb.angularVelocity = new Vector3(dir * stats.MoveSpeed * 0.6f, rb.angularVelocity.y, 0);

        if (Random.value < 0.01f) RegisterInput(KeyCode.J);
        if (Random.value < 0.005f) RegisterInput(KeyCode.K);

        TryResolveCombo();
    }

    private void RegisterInput(KeyCode k)
    {
        _inputBuffer.Add(k);
        _lastInputTime = Time.time;
        if (_inputBuffer.Count > 6) _inputBuffer.RemoveAt(0);
    }

    private void TryResolveCombo()
    {
        var combos = stats.Definition.SpecialCombos;
        if (combos == null) return;

        if (Time.time - _lastInputTime > _combowWindow) _inputBuffer.Clear();

        foreach (var combo in combos)
        {
            if (EndsWithSequence(_inputBuffer, combo.Sequence))
            {
                Debug.Log($"{stats.Definition.DisplayName} performed combo: {combo.Name}");
                _inputBuffer.Clear();
                break;
            }
        }
    }

    private bool EndsWithSequence(List<KeyCode> buffer, List<KeyCode> seq)
    {
        if (buffer.Count < seq.Count) return false;
        for (int i = 0; i < seq.Count; i++)
        {
            if (buffer[buffer.Count - seq.Count + i] != seq[i]) return false;
        }
        return false;
    }
}