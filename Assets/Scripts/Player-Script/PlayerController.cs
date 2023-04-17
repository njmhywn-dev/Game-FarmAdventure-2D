using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;
    Animator animator;
    SpriteRenderer spriteRenderer;

    [SerializeField]
    private InputActionReference movement, attack, pointerPosition;

    private Vector2 pointerInput, movementInput;

    Rigidbody2D rb;
    List<RaycastHit2D> castCollision = new List<RaycastHit2D>();

    private WeaponParent weaponparent;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        weaponparent = GetComponentInChildren<WeaponParent>();
    }

    void FixedUpdate() {

        pointerInput = GetPointerInput();
        print(pointerInput);
        weaponparent.PointerPosition = pointerInput;


        if(movementInput != Vector2.zero){
            bool success = TryMove(movementInput);

            if(!success){
                success = TryMove(new Vector2(movementInput.x,0));
            }

            if(!success){
                success = TryMove(new Vector2(0,movementInput.y));
            }
            
            animator.SetBool("isMoving", true);
        }else{
            animator.SetBool("isMoving", false);
        }

        // if(movementInput.x < 0 ){
        //     spriteRenderer.flipX = true;
        // }else if(movementInput.x > 0 ){
        //     spriteRenderer.flipX = false;
        // }

        Vector2 direction = (pointerInput -(Vector2)transform.position).normalized;

        Vector2 scale = transform.localScale;

        if(direction.x < 0){
            scale.x = -1;
        }else if(direction.x > 0){
            scale.x = 1;
        }
        transform.localScale = scale;

        
    }

    private bool TryMove(Vector2 direction){
        if(direction != Vector2.zero){
            int count = rb.Cast(
                movementInput,
                movementFilter,
                castCollision,
                moveSpeed * Time.fixedDeltaTime + collisionOffset
            );

            if(count == 0){
                rb.MovePosition(rb.position + movementInput * moveSpeed * Time.fixedDeltaTime); 
                return true;   
            }else{
                return false;
            }
        }else{
            return false;
        }    
    }

    private Vector2 GetPointerInput()
    {
        Vector3 mousePos = pointerPosition.action.ReadValue<Vector2>();
        mousePos.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePos);

    }

    void OnMove(InputValue movementValue){
        movementInput = movementValue.Get<Vector2>();
    }
}
