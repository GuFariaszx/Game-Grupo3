using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;

/*
    Este arquivo possui uma vers�o comentada com detalhes sobre como cada linha funciona. 
    A vers�o comentada cont�m c�digo mais f�cil e simples de ler. Este arquivo est� minimizado.
*/


/// <summary>
/// Script principal para o movimento em terceira pessoa do personagem no jogo.
/// Certifique-se de que o objeto que receber� este script (o jogador) 
/// tenha a tag Player e o componente Character Controller.
/// </summary>
public class PlayerScript1 : MonoBehaviour
{

    //"Velocidade com que o personagem se move. N�o � afetado pela gravidade ou pelo pulo."
    public float velocity = 5f;
    //"Este valor � adicionado � velocidade enquanto o personagem est� correndo."
    public float sprintAdittion = 10f;
    //"Quanto maior o valor, maior ser� o pulo do personagem."
    public float jumpForce = 18f;
    //"Tempo que o personagem fica no ar. Quanto maior o valor, mais tempo o personagem flutua antes de cair."
    public float jumpTime = 0.85f;
    //"For�a que puxa o jogador para baixo. Alterar este valor afeta todos os movimentos, pulos e quedas."
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

        // Mensagem informando que o usu�rio esqueceu de adicionar um animator
        if (animator == null)
            Debug.LogWarning("Ei amigo, voc� n�o tem o componente Animator no seu jogador. Sem ele, as anima��es n�o funcionar�o.");
    }


    // O Update � usado aqui apenas para identificar as teclas pressionadas e ativar anima��es
    void Update()
    {

        // Verifica��o de entradas
        inputHorizontal = Input.GetAxis("Horizontal");
        inputVertical = Input.GetAxis("Vertical");
        inputJump = Input.GetAxis("Jump") == 1f;
        inputSprint = Input.GetAxis("Fire3") == 1f;
        // Infelizmente o GetAxis n�o funciona com GetKeyDown, ent�o as entradas devem ser verificadas individualmente
        inputCrouch = Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl);

        // Verifica se pressionou a tecla de agachar e altera o estado do jogador
        if (inputCrouch)
            isCrouching = !isCrouching;

        // Anima��o de correr e agachar
        // Se n�o tiver o componente animator, esse bloco n�o ser� executado
        if (cc.isGrounded && animator != null)
        {

            // Agachar
            // Nota: A anima��o de agachar n�o diminui o colisor do personagem
            animator.SetBool("crouch", isCrouching);

            // Correr
            float minimumSpeed = 5.0f;
            animator.SetBool("run", cc.velocity.magnitude > minimumSpeed);

            // Sprint
            isSprinting = cc.velocity.magnitude > minimumSpeed && inputSprint;
            animator.SetBool("sprint", isSprinting);

        }

        // Anima��o de pulo
        if (animator != null)
            animator.SetBool("air", cc.isGrounded == false);

        // Verifica se pode pular ou n�o
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


    // Com as entradas e anima��es definidas, o FixedUpdate � respons�vel por aplicar os movimentos e a��es no jogador
    private void FixedUpdate()
    {

        // Aumento de velocidade ao correr ou desacelera��o ao agachar
        float velocityAdittion = 3f;
        if (isSprinting)
            velocityAdittion = sprintAdittion;
        if (isCrouching)
            velocityAdittion = -(velocity * 0.60f); // -50% de velocidade

        // Movimento de dire��o
        float directionX = inputHorizontal * (velocity + velocityAdittion) * Time.deltaTime;
        float directionZ = inputVertical * (velocity + velocityAdittion) * Time.deltaTime;
        float directionY = 0;

        // Tratamento do pulo
        if (isJumping)
        {

            // Aplica in�rcia e suavidade ao subir o pulo
            // N�o � necess�rio ao descer, pois a gravidade vai puxar gradualmente
            directionY = Mathf.SmoothStep(jumpForce, jumpForce * 0.30f, jumpElapsedTime / jumpTime) * Time.deltaTime;

            // Cron�metro do pulo
            jumpElapsedTime += Time.deltaTime;
            if (jumpElapsedTime >= jumpTime)
            {
                isJumping = false;
                jumpElapsedTime = 0;
            }
        }

        // Aplica gravidade no eixo Y
        directionY = directionY - gravity * Time.deltaTime;


        // --- Rota��o do personagem --- 

        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        // Relaciona a frente com a dire��o Z (profundidade) e a direita com X (movimento lateral)
        forward = forward * directionZ;
        right = right * directionX;

        if (directionX != 0 || directionZ != 0)
        {
            float angle = Mathf.Atan2(forward.x + right.x, forward.z + right.z) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.15f);
        }

        // --- Fim da rota��o ---


        Vector3 verticalDirection = Vector3.up * directionY;
        Vector3 horizontalDirection = forward + right;

        Vector3 moviment = verticalDirection + horizontalDirection;
        cc.Move(moviment);

    }


    // Esta fun��o faz com que o personagem finalize o pulo se bater a cabe�a em algo
    void HeadHittingDetect()
    {
        float headHitDistance = 1.1f;
        Vector3 ccCenter = transform.TransformPoint(cc.center);
        float hitCalc = cc.height / 2f * headHitDistance;

        // Descomente essa linha para ver o raio desenhado na cabe�a do personagem
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