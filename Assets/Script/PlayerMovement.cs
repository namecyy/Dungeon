using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    
    [Header("移动")]
    [SerializeField] private float moveSpeed = 5f;
    private Vector2 moveInput;
    
    [Header("冲刺")]
    [SerializeField] private float dashForce = 20f;
    [SerializeField] private float dashDuration = 0.30f;
    [SerializeField] private float dashCooldown = 0.5f;
    private bool canDash = true;
    //private bool isDashing = false;

    //状态机
    enum playerState
    {
        idle,
        run,
        dash,
        dead
    }
    playerState currentState = playerState.idle;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.GetState() == GameState.Menu) return;
        PlayerTowardsMouse();
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        switch (currentState)
        {
            case playerState.idle:
            case playerState.run:
                // 能接受输入的状态：统一处理移动和冲刺
                currentState = moveInput.magnitude > 0.1f ? playerState.run : playerState.idle;
                
                if (Input.GetKeyDown(KeyCode.Space) && canDash)
                    StartCoroutine(Dash());
                break;

            case playerState.dash:
                // 冲刺由协程管，Update 不碰
                break;

            case playerState.dead:
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
            case playerState.idle:
                rb.velocity = Vector2.zero;
                break;
            case playerState.run:
                rb.velocity = moveInput.normalized * moveSpeed;
                break;
            case playerState.dash:
                break;
        }
    }

    private void PlayerTowardsMouse()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //mousePos.z = 0;

        Vector2 direction = (mousePos - rb.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        rb.SetRotation(angle);
    }

    private IEnumerator Dash()
    {

        currentState = playerState.dash;
        canDash = false;
        
        Vector2 dashDir = moveInput.magnitude > 0.1f ? moveInput.normalized : (Vector2)transform.right;
        Debug.Log(dashDir);
        rb.velocity = dashDir * dashForce;
        yield return new WaitForSeconds(dashDuration);

        //isDashing = false;
        currentState = playerState.idle;
        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(dashCooldown-dashDuration);

        canDash = true;
        
    }
}
