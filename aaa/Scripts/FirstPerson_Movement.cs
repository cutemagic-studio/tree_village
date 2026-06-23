using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPerson_Movement : MonoBehaviour
{
    private CubeAnimal player;

    private CubeAnimal_InputAction controls;
    private CharacterController controller;
    private Animator animator;

    [Header("Movement Infor")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 3f;
    [SerializeField] private float turnSpeed = 1f;
    public float speed = 2f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;

    bool isGrounded;


    //
    public Vector2 moveInput { get; private set; } // Biến để lưu trữ input di chuyển và ngắm bắn từ New Input System

    private Vector3 movementDirection; // Vector3 để lưu hướng di chuyển hiện tại

    private bool isRunning; // Biến để theo dõi trạng thái chạy


    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<CubeAnimal>();
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        AssignInputEvents();
    }

    // Update is called once per frame
    void Update()
    {
        //huongDanYoutube();

        //
        ApplyMovement();
        AnimatorControllers();
    }

    private void huongDanYoutube()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        movementDirection = move;

        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    private void AnimatorControllers()
    {
        // Tính toán vận tốc theo hướng x và z dựa trên hướng di chuyển và hướng của nhân vật
        float xVelocity = Vector3.Dot(movementDirection.normalized, transform.right);
        float zVelocity = Vector3.Dot(movementDirection.normalized, transform.forward);

        // Cập nhật các tham số của Animator với thời gian trễ để có chuyển động mượt mà hơn
        animator.SetFloat("VT_xVelocity", xVelocity, .1f, Time.deltaTime);
        animator.SetFloat("VT_zVelocity", zVelocity, .1f, Time.deltaTime);

        // Tránh việc nhân vật vẫn chạy khi đè shift nhưng không di chuyển
        bool isActuallyRunning = isRunning && movementDirection.magnitude > 0;

        // Cập nhật trạng thái chạy trong Animator
        animator.SetBool("VT_isRunning", isActuallyRunning);
    }

    private void ApplyMovement()
    {
        movementDirection = (transform.right * moveInput.x + transform.forward * moveInput.y).normalized * speed;

        ApplyGravityAndJump();

        controller.Move(movementDirection * Time.deltaTime);
    }

    private void ApplyGravityAndJump()
    {
        // BƯỚC 1: Kiểm tra nếu đang đứng trên đất
        if (controller.isGrounded)
        {
            // Nếu nhân vật đang rơi xuống và chạm đất, ghì nhẹ xuống để ổn định isGrounded
            if (velocity.y < 0)
            {
                velocity.y = -2f;
            }

            // BƯỚC 2: Chỉ khi đang trên đất mới cho phép check Nhảy
            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        else
        {
            // BƯỚC 3: Nếu đang trên không, áp dụng trọng lực tăng dần theo thời gian
            velocity.y += gravity * Time.deltaTime;
        }

        // BƯỚC 4: Gán vận tốc trục Y chung vào hướng di chuyển tổng 
        movementDirection.y = velocity.y;
    }

    private void AssignInputEvents()
    {
        // Lấy tham chiếu đến PlayerControls từ VT_Player
        controls = player.controls;

        //controls.VT_Character.Fire.performed += context => Shoot();

        // Di chuyển
        controls.CubeAnimal.Movement.performed += context => moveInput = context.ReadValue<Vector2>();
        controls.CubeAnimal.Movement.canceled += context => moveInput = Vector2.zero;

        // Chạy
        controls.CubeAnimal.Run.performed += context =>
        {
            speed = runSpeed;
            isRunning = true;
        };

        // Khi thả phím chạy, trở về tốc độ đi bộ
        controls.CubeAnimal.Run.canceled += context =>
        {
            speed = walkSpeed;
            isRunning = false;
        };
    }
}
