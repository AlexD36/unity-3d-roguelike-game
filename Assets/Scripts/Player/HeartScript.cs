using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HeartScript : MonoBehaviour
{

    public static void DrawHeart(Sprite Type, int num)
    {
        GameObject Heart = new GameObject("Heart");
        Image HeartImage = Heart.AddComponent<Image>();
        HeartImage.sprite = Type;
        RectTransform rectTransform = Heart.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(Player.HeartPanel.GetComponent<RectTransform>().sizeDelta.x / 10, Player.HeartPanel.GetComponent<RectTransform>().sizeDelta.x / 10);
        Animator animator = Heart.AddComponent<Animator>();
        animator.runtimeAnimatorController = Player.HeartAnimator;


        float XPos = 0;
        float YPos = -5;
        if (num <= 9)
        {
             XPos = num * Player.HeartPanel.GetComponent<RectTransform>().sizeDelta.x / 10;
        }
        else
        {
            XPos = (num-10) * Player.HeartPanel.GetComponent<RectTransform>().sizeDelta.x / 10;
            YPos = -5 - Player.HeartPanel.GetComponent<RectTransform>().sizeDelta.x / 10;
        }
        rectTransform.position = new Vector2(XPos, YPos);
        rectTransform.pivot = new Vector2(0f, 1f);
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax= new Vector2(0, 1);
        HeartImage.transform.SetParent(Player.HeartPanel.transform, false);

    }


    public static void DrawHearts()
    {

        for (int i = 0; i <= Player.Health - 1; i++) 
        {
            DrawHeart(Player.FullHeart, i);
        }

        if (Player.Health % 1 != 0)
        {
            DrawHeart(Player.HalfHeart, (int)Player.Health);
        }

        for (int i = (int)Player.Health; i <= Player.MaxHealth-1; ++i)
        {
            DrawHeart(Player.EmptyHeart, (int)i);
        }

    }

    public static IEnumerator Uninvincible()
    {
        yield return new WaitForSeconds(.5f);
        Player.Invincible = false;

    }

    public static IEnumerator Die()
    {
        Player.DamagePanel.SetActive(true);
        yield return new WaitForSeconds(0f);

        for (int i = 0; i < Player.HeartPanel.transform.childCount; ++i)
        {
            GameObject.Destroy(Player.HeartPanel.transform.GetChild(i).gameObject);
        }

        Player.animator.Play("Die");
        Player.State = "Dead";
        Player.Invincible = true;
        Destroy(Player.transform.GetComponent<PlayerMovement>());
        Destroy(Player.transform.GetComponent<PlayerAttack>());
        Destroy(Player.transform.GetComponent<ChangeRooms>());
        Destroy(Player.transform.GetComponent<BombScript>()); 
        Destroy(Player.transform.GetComponent<Rigidbody>());
        Destroy(Player.transform.GetComponent<CharacterController>());
        Player.DamagePanel.SetActive(false);
    }

    public static IEnumerator WaitAndRedrawHearts()
    {
        Player.DamagePanel.SetActive(true);
        yield return new WaitForSeconds(.1f);
        Player.DamagePanel.SetActive(false);

        yield return new WaitForSeconds(.4f);
        for (int i = 0; i < Player.HeartPanel.transform.childCount; ++i)
        {
            GameObject.Destroy(Player.HeartPanel.transform.GetChild(i).gameObject);
        }
        DrawHearts();
   
    }

    public static void TakeDamage(float damage)
    {
        if (!Player.Invincible)
        {
            Player.Invincible = true;
            CoroutineManager.Instance.StartCoroutine(Uninvincible());

            if((Player.Health - damage) <= 0)
            {
                CoroutineManager.Instance.StartCoroutine(Die());
                return;
            }
        


            for (int i = 1; i <= damage; ++i)
            {
                (Instantiate(Player.HeartPanel.transform.GetChild((int)Player.Health - i),
                    Player.HeartPanel.transform)).GetComponent<Animator>().Play("HeartAnimation");
                Player.HeartPanel.transform.GetChild((int)Player.Health - i).GetComponent<Image>().sprite = Player.EmptyHeart;

            }

            Player.Health -= damage;

          
            if(Player.Health > 0) CoroutineManager.Instance.StartCoroutine(WaitAndRedrawHearts());
            
        }
    }

    void Start()
    {
        DrawHearts();

        
    }


    //private void Update()
    //{


    //    //if(Input.GetKey(KeyCode.I))
    //    //{
    //    //    TakeDamage(1);
    //    //}

    //    //if (Input.GetKey(KeyCode.O))
    //    //{
    //    //    TakeDamage(2);
    //    //}

    //    //if (Input.GetKey(KeyCode.P))
    //    //{
    //    //    TakeDamage(3);
    //    //}

    //    //if (Input.GetKey(KeyCode.H))
    //    //{
    //    //    Player.Health = Player.MaxHealth;
    //    //    DrawHearts();
    //    //}
    //}



    public class CoroutineManager : MonoBehaviour
    {
        private static CoroutineManager _instance;

        public static CoroutineManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("CoroutineManager");
                    _instance = go.AddComponent<CoroutineManager>();
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            _instance = this;
        }

    }
}
