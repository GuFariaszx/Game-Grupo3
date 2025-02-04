using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;

/*
    Este arquivo possui uma versão comentada com detalhes sobre como cada linha funciona. 
    A versão comentada contém código mais fácil e simples de ler. Este arquivo está minimizado.
*/


/// <summary>
/// Script principal para o movimento em terceira pessoa do personagem no jogo.
/// Certifique-se de que o objeto que receberá este script (o jogador) 
/// tenha a tag Player e o componente Character Controller.
/// </summary>
public class PlayerScript1 : MonoBehaviour
{

    //"Velocidade com que o personagem se move. Não é afetado pela gravidade ou pelo pulo."
    public float velocity = 5f;
    //"Este valor é adicionado à velocidade enquanto o personagem está correndo."
    public float sprintAdittion = 10f;
    //"Quanto maior o valor, maior será o pulo do personagem."
    public float jumpForce = 18f;
    //"Tempo que o personagem fica no ar. Quanto maior o valor, mais tempo o personagem flutua antes de cair."
    public float jumpTime = 0.85f;
    //"Força que puxa o jogador para baixo. Alterar este valor afeta todos os movimentos, pulos e quedas."
    public float gravity = 9.8f;

    float jumpElapsedTime = 0;

    // Estados do jogador
    bool isJumping = false;
    bool isSprinting = false;
    bool isCrouching = false;

    // Entradas
    float inputHorizontal;
    float inputVertical;
    bool inputJump;
    bool inputCrouch;
    bool inputSprint;

    Animator animator;
    CharacterController cc;

    //objetos
    public int collectedObjects;
    public TextMeshProUGUI coinText;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // Mensagem informando que o usuário esqueceu de adicionar um animator
        if (animator == null)
            Debug.LogWarning("Ei amigo, você não tem o componente Animator no seu jogador. Sem ele, as animações não funcionarão.");
    }


    // O Update é usado aqui apenas para identificar as teclas pressionadas e ativar animações
    void Update()
    {

        // Verificação de entradas
        inputHorizontal = Input.GetAxis("Horizontal");
        inputVertical = Input.GetAxis("Vertical");
        inputJump = Input.GetAxis("Jump") == 1f;
        inputSprint = Input.GetAxis("Fire3") == 1f;
        // Infelizmente o GetAxis não funciona com GetKeyDown, então as entradas devem ser verificadas individualmente
        inputCrouch = Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl);

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
            float minimumSpeed = 5.0f;
            animator.SetBool("run", cc.velocity.magnitude > minimumSpeed);

            // Sprint
            isSprinting = cc.velocity.magnitude > minimumSpeed && inputSprint;
            animator.SetBool("sprint", isSprinting);

        }

        // Animação de pulo
        if (animator != null)
            animator.SetBool("air", cc.isGrounded == false);

        // Verifica se pode pular ou não
        if (inputJump && cc.isGrounded)
        {
            isJumping = true;
            // Desabilita agachar ao pular
            //isCrouching = false; 
        }

        HeadHittingDetect();

        //coletar os objetos
        coinText.text = collectedObjects.ToString();


    }


    // Com as entradas e animações definidas, o FixedUpdate é responsável por aplicar os movimentos e ações no jogador
    private void FixedUpdate()
    {

        // Aumento de velocidade ao correr ou desaceleração ao agachar
        float velocityAdittion = 3f;
        if (isSprinting)
            velocityAdittion = sprintAdittion;
        if (isCrouching)
            velocityAdittion = -(velocity * 0.60f); // -50% de velocidade

        // Movimento de direção
        float directionX = inputHorizontal * (velocity + velocityAdittion) * Time.deltaTime;
        float directionZ = inputVertical * (velocity + velocityAdittion) * Time.deltaTime;
        float directionY = 0;

        // Tratamento do pulo
        if (isJumping)
        {

            // Aplica inércia e suavidade ao subir o pulo
            // Não é necessário ao descer, pois a gravidade vai puxar gradualmente
            directionY = Mathf.SmoothStep(jumpForce, jumpForce * 0.30f, jumpElapsedTime / jumpTime) * Time.deltaTime;

            // Cronômetro do pulo
            jumpElapsedTime += Time.deltaTime;
            if (jumpElapsedTime >= jumpTime)
            {
                isJumping = false;
                jumpElapsedTime = 0;
            }
        }

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


    // Esta função faz com que o personagem finalize o pulo se bater a cabeça em algo
    void HeadHittingDetect()
    {
        float headHitDistance = 1.1f;
        Vector3 ccCenter = transform.TransformPoint(cc.center);
        float hitCalc = cc.height / 2f * headHitDistance;

        // Descomente essa linha para ver o raio desenhado na cabeça do personagem
        // Debug.DrawRay(ccCenter, Vector3.up * headHeight, Color.red);

        if (Physics.Raycast(ccCenter, Vector3.up, hitCalc))
        {
            jumpElapsedTime = 0;
            isJumping = false;
        }
    }

    public void AddObject()
    {
        this.collectedObjects++;

    }
}