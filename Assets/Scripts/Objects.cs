using UnityEngine;


public class Coletaveis : MonoBehaviour
{
    [Header("UI")]

    public GameObject coinUI;
    public GameObject swordUI;
    public GameObject shieldUI;
    public GameObject coinPurseUI;
    public GameObject diamondUI;

    public GameObject coin;
    public GameObject sword;
    public GameObject shield;
    public GameObject coinPurse;
    public GameObject diamond;


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnControllerColliderHit(ControllerColliderHit collision)
    {
        Debug.Log("Colidiu");
        if (collision.gameObject.tag == "Coin")
        {
            Destroy(coin);
            if (coinUI != null)
            {
                if (!coinUI.activeSelf)
                {
                    coinUI.SetActive(true);
                }


            }
        }

        if (collision.gameObject.tag == "Sword")
        {
            Destroy(sword);
            if (coinUI != null)
            {
                if (!swordUI.activeSelf)
                {
                    swordUI.SetActive(true);
                }


            }
        }

        if (collision.gameObject.tag == "Shield")
        {
            Destroy(shield);
            if (shieldUI != null)
            {
                if (!shieldUI.activeSelf)
                {
                    shieldUI.SetActive(true);
                }


            }
        }
        if (collision.gameObject.tag == "CoinPurse")
        {
            Destroy(coinPurse);
            if (coinPurseUI != null)
            {
                if (!coinPurseUI.activeSelf)
                {
                    coinPurseUI.SetActive(true);
                }


            }
        }

        if (collision.gameObject.tag == "Diamond")
        {
            Destroy(diamond);
            if (diamondUI != null)
            {
                if (!diamondUI.activeSelf)
                {
                    diamondUI.SetActive(true);
                }


            }
        }


    }
}
