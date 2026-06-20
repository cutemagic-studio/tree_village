using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeAnimal_Movement : MonoBehaviour
{
    // Tham chiếu đến VT_Player để truy cập PlayerControls
    private CubeAnimal player;

    // Các thành phần cần thiết
    private CubeAnimal_InputAction controls;
    private CharacterController characterController;
    private Animator animator;

    [Header("Movement Infor")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float turnSpeed;
    private float speed;

    public Vector2 moveInput { get; private set; } // Biến để lưu trữ input di chuyển và ngắm bắn từ New Input System

    private Vector3 movementDirection; // Vector3 để lưu hướng di chuyển hiện tại

    private float verticalVelocity; // Biến để lưu trữ vận tốc theo phương thẳng đứng (để áp dụng trọng lực)

    private bool isRunning; // Biến để theo dõi trạng thái chạy

    private void Start()
    {
        // Lấy tham chiếu đến VT_Player để truy cập PlayerControls
        player = GetComponent<CubeAnimal>();

        // Lấy các thành phần cần thiết
        characterController = GetComponent<CharacterController>();
        // Giả sử Animator nằm trong một child của GameObject này
        animator = GetComponentInChildren<Animator>();

        speed = walkSpeed;

        // Gán sự kiện input
        AssignInputEvents();
    }

    private void Update()
    {
        
        // Áp dụng di chuyển và ngắm bắn mỗi frame
        ApplyMovement();
        // Chỉ ngắm bắn khi có input ngắm
        ApplyRotation();
        // Cập nhật các tham số của Animator dựa trên hướng di chuyển và trạng thái chạy
        AnimatorControllers();
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

    private void ApplyRotation()
    {
        //// Tính toán hướng nhìn từ vị trí nhân vật đến điểm va chạm
        //Vector3 lookingDirection = player.aim.GetMouseHitInfo().point - transform.position;

        //// Loại bỏ thành phần y để nhân vật chỉ quay trên mặt phẳng xz
        //lookingDirection.y = 0f;

        //// Chuẩn hóa hướng nhìn để có độ dài bằng 1
        //lookingDirection.Normalize();

        //// Quay nhân vật về hướng nhìn
        ////transform.forward = lookingDirection; // Cách này sẽ làm nhân vật quay ngay lập tức

        //Quaternion desiredRotation = Quaternion.LookRotation(lookingDirection);
        //transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, turnSpeed * Time.deltaTime);

    }

    private void ApplyMovement()
    {
        // Tạo vector di chuyển dựa trên input di chuyển
        movementDirection = new Vector3(moveInput.x, 0, moveInput.y);

        ApplyGravity();

        // Di chuyển nhân vật nếu có hướng di chuyển
        if (movementDirection.magnitude > 0)
        {
            characterController.Move(movementDirection * Time.deltaTime * speed);
        }
    }

    private void ApplyGravity()
    {
        if (characterController.isGrounded == false)
        {
            verticalVelocity = verticalVelocity - 9.81f * Time.deltaTime;
            movementDirection.y = verticalVelocity;
        }
        else
        {
            verticalVelocity = -.5f;
        }
    }


    #region New Input System
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


    #endregion
}
