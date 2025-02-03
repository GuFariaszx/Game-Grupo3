using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;

public class PlayerSCript1 : MonoBehaviour
{

    //Velocidade com que o personagem se move.Não é afetado pela gravidade ou pelo pulo.
    public float velocity = 5f;
    //Este valor é adicionado à velocidade enquanto o personagem está correndo.
    public float sprintAdittion = 3.5f;
    //Tempo que o personagem fica no ar. Quanto maior o valor, mais tempo o personagem flutua antes de cair.
    public float jumpTime = 0.85f;
    //Força que puxa o jogador para baixo. Alterar este valor afeta todos os movimentos, pulos e quedas.
    public float gravity = 9.8f;

    float jumpElapsedTime = 0;

    // Estados do jogador
    bool isSprinting = false;
    bool isCrouching = false;

    // Entradas
    float inputHorizontal;
    float inputVertical;
    bool inputCrouch;
    bool inputSprint;

    Animator animator;
    CharacterController cc;

    //objetos
    public int collectedObjects;
    public TextMeshProUGUI coinText;

    //spawnpoint
    public float threshold = -10f;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        cc.detectCollisions = true;


        // Mensagem informando que o usuário esqueceu de adicionar um animator
        if (animator == null)
            Debug.LogWarning("Ei amigo, você não tem o componente Animator no seu jogador. Sem ele, as animações não funcionarão.");
    }



    void Update()
    {

        // entradas
        inputHorizontal = Input.GetAxis("Horizontal");
        inputVertical = Input.GetAxis("Vertical");
        inputSprint = Input.GetAxis("Fire3") == 1f;
        inputCrouch = Input.GetKeyDown(KeyCode.LeftControl);

        // Verifica se pressionou a tecla de agachar e altera o estado do jogador
        if (inputCrouch)
            isCrouching = !isCrouching;

        // Animação de correr e agachar
        // Se não tiver o componente animator, esse bloco não será executado
        if (cc.isGrounded && animator != null)
        {

            // Agachar
            // Nota: A animação de agachar não diminui o colisor do personagem
            animator.SetBool("crouch", isCrouching);

            // Correr
            float minimumSpeed = 0.9f;
            animator.SetBool("run", cc.velocity.magnitude > minimumSpeed);

            // Sprint
            isSprinting = cc.velocity.magnitude > minimumSpeed && inputSprint;
            animator.SetBool("sprint", isSprinting);

        }
        //coletar os objetos
       

    }


    // Com as entradas e animações definidas, o FixedUpdate é responsável por aplicar os movimentos e ações no jogador
    private void FixedUpdate()
    {

        // Aumento de velocidade ao correr ou desaceleração ao agachar
        float velocityAdittion = 0;
        if (isSprinting)
            velocityAdittion = sprintAdittion;
        if (isCrouching)
            velocityAdittion = -(velocity * 0.50f); // -50% de velocidade

        // Movimento de direção
        float directionX = inputHorizontal * (velocity + velocityAdittion) * Time.deltaTime;
        float directionZ = inputVertical * (velocity + velocityAdittion) * Time.deltaTime;
        float directionY = 0;

      

        // Aplica gravidade no eixo Y
        directionY = directionY - gravity * Time.deltaTime;


        // --- Rotação do personagem --- 

        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        // Relaciona a frente com a direção Z (profundidade) e a direita com X (movimento lateral)
        forward = forward * directionZ;
        right = right * directionX;

        if (directionX != 0 || directionZ != 0)
        {
            float angle = Mathf.Atan2(forward.x + right.x, forward.z + right.z) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.15f);
        }

        // --- Fim da rotação ---
        Vector3 verticalDirection = Vector3.up * directionY;
        Vector3 horizontalDirection = forward + right;

        Vector3 moviment = verticalDirection + horizontalDirection;
        cc.Move(moviment);

    }

    public void AddObject()
    {
        this.collectedObjects++;
        
    }

    public GameObject respawn;



    private void OnControllerColliderHit(ControllerColliderHit collision)
    {
        if (respawn != null)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                transform.position = respawn.transform.position;
            }
        } else
        {
            Debug.Log("O objeto respawn não foi associado!");
        }
        
    }
}

