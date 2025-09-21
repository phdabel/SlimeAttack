using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    const int DAMAGE = 2;
    const int FAST_DAMAGE = 1;
    private GameManager _GameManager;
    private CharacterController controller;
    private Animator anim;
    private Vector3 direction;
    private Vector3 velocity;
    private bool isWalking;
    private bool isAttacking;

    [Header("Config Player")]
    public float movementSpeed = 3f;
    public float gravity = -9.81f;
    public float groundedGravity = -0.05f;
    public int HP;

    [Header("Attack Config")]
    public ParticleSystem fxAttack;
    public Transform hitBox;
    [Range(0.2f, 1f)]
    public float hitRange = 0.5f;
    public LayerMask hitMask;
    public Collider[] hitInfo;
    public GameObject CameraAerea;

    // Cache das referencias do Input para evitar alocações
    private Keyboard keyboard;
    private Gamepad gamepad;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        _GameManager = FindAnyObjectByType<GameManager>() as GameManager;
        
        // Cache das referências do Input
        keyboard = Keyboard.current;
        gamepad = Gamepad.current;
    }

    void Update()
    {
        // Verifica se o jogo está em estado de gameplay
        if (_GameManager.GameState != GameState.GAMEPLAY)
        {
            HandleGravity();
            ToggleCamera();
            return;
        }

        // Atualiza referências do Input (só se mudou)
        if (keyboard != Keyboard.current) keyboard = Keyboard.current;
        if (gamepad != Gamepad.current) gamepad = Gamepad.current;
        
        HandleMovement();
        HandleGravity();
        HandleHeavyAttack();
        HandleAttack();
        HandleDeffend();
        ToggleCamera();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TakeDamage"))
        {
            GetHit(1);
        }
    }

    void ToggleCamera()
    {
        if (keyboard != null && keyboard.cKey.wasPressedThisFrame)
        {
            CameraAerea.SetActive(!CameraAerea.activeSelf);
        }

        if (gamepad != null && gamepad.dpad.up.wasPressedThisFrame)
        {
            CameraAerea.SetActive(!CameraAerea.activeSelf);
        }
    }

    void HandleMovement()
    {
        Vector2 inputVector = GetMovementInput();
        
        // Inverte input se CameraAerea estiver ativa
        if (CameraAerea.activeSelf)
        {
            inputVector = -inputVector;
        }

        float horizontal = inputVector.x;
        float vertical = inputVector.y;

        direction = new Vector3(horizontal, 0, vertical).normalized;

        if (direction.magnitude > 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, targetAngle, 0);
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }

        controller.Move(direction * movementSpeed * Time.deltaTime);
        anim.SetBool("isWalking", isWalking);
    }

    // Método otimizado para obter input de movimento
    Vector2 GetMovementInput()
    {
        Vector2 inputVector = Vector2.zero;

        // Input do teclado
        if (keyboard != null)
        {
            Vector2 keyboardInput = Vector2.zero;
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) keyboardInput.y = 1;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) keyboardInput.y = -1;
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) keyboardInput.x = -1;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) keyboardInput.x = 1;

            inputVector = keyboardInput;
        }

        // Input do controle (só se não há input do teclado)
        if (gamepad != null && inputVector == Vector2.zero)
        {
            Vector2 stickInput = gamepad.leftStick.ReadValue();
            if (stickInput.magnitude > 0.1f)
            {
                inputVector = stickInput;
            }
        }

        return inputVector;
    }

    void HandleGravity()
    {
        if (controller.isGrounded)
        {
            velocity.y = groundedGravity;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        controller.Move(velocity * Time.deltaTime);
    }

    void HandleAttack()
    {
        bool attackInput = false;

        if (keyboard != null && keyboard.leftCtrlKey.wasPressedThisFrame)
        {
            attackInput = true;
        }

        if (gamepad != null && gamepad.xButton.wasPressedThisFrame)
        {
            attackInput = true;
        }

        if (attackInput && !isAttacking)
        {
            anim.SetTrigger("Attack");
            fxAttack.Emit(1);
            isAttacking = true;

            hitInfo = Physics.OverlapSphere(hitBox.position, hitRange, hitMask);
            foreach (Collider info in hitInfo)
            {
                info.SendMessage("GetHit", FAST_DAMAGE, SendMessageOptions.DontRequireReceiver);
            }

            Debug.Log("Ataque executado!");
        }
    }

    void HandleHeavyAttack()
    {
        bool heavyAttackInput = false;

        if (keyboard != null && keyboard.rightShiftKey.wasPressedThisFrame)
        {
            heavyAttackInput = true;
        }

        if (gamepad != null && gamepad.aButton.wasPressedThisFrame)
        {
            heavyAttackInput = true;
        }

        if (heavyAttackInput)
        {
            anim.SetTrigger("HeavyAttack");

            hitInfo = Physics.OverlapSphere(hitBox.position, hitRange, hitMask);
            foreach (Collider info in hitInfo)
            {
                info.SendMessage("GetHit", DAMAGE, SendMessageOptions.DontRequireReceiver);
            }

            Debug.Log("Ataque pesado executado!");
        }
    }

    void HandleDeffend()
    {
        bool defendInput = false;

        if (keyboard != null && keyboard.leftShiftKey.wasPressedThisFrame)
        {
            defendInput = true;
        }

        if (gamepad != null && gamepad.bButton.wasPressedThisFrame)
        {
            defendInput = true;
        }

        if (defendInput)
        {
            anim.SetTrigger("Defend");
            Debug.Log("Defesa executada!");
        }
    }

    void AttackDone()
    {
        isAttacking = false;
    }

    void GetHit(int amount)
    {
        HP -= amount;
        if (HP > 0)
        {
            anim.SetTrigger("Hit");
        }
        else
        {
            _GameManager.ChangeGameState(GameState.DIED);
            anim.SetTrigger("Die");
        }
    }

    void OnDrawGizmosSelected()
    {
        if (hitBox == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(hitBox.position, hitRange);
    }
}