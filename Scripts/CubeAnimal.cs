using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class CubeAnimal : MonoBehaviour
{
    public CubeAnimal_InputAction controls { get; private set; } // Read-Only

    public CubeAnimal_Movement movement { get; private set; } // Read-Only

    public Animator anim { get; private set; } // Read-Only

    private void Awake()
    {
        controls = new CubeAnimal_InputAction();
        
        movement = GetComponent<CubeAnimal_Movement>();
        
        anim = GetComponentInChildren<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {

        controls.Disable();
    }
}
