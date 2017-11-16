using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameController : MonoBehaviour {

    private const float FALL_STICK_SPEED = 4.5f;
    private const float HERO_SPEED = 3f;
    private const float LEFT_MARGIN = -2.5f;
    private const float MAX_STICK_SCALE = 550f;
    private const float MAX_COL_SCLAE = 1.5f;
    private const float MIN_COL_SCALE = 0.3f;

    private Vector3 SPAWN_VECTOR = new Vector3(5f, -3f, 0f);


    [SerializeField]
    private Text scoreLabel;
    [SerializeField]
    private Text bestScoreLabel;
    [SerializeField]
    private GameObject restartDialog;
    [SerializeField]
    private GameObject startDialoge;
    [SerializeField]
    private GameObject stickPrefab;
    [SerializeField]
    public GameObject hero;
    [SerializeField]
    private AudioManager audioManager;
    


    private Animator heroAnimator;
    private GameObject stick;
    private Transform prevColumn;
    private Transform nextColumn;
    private Transform currentColumn;
    private Transform heroTransform;   
    private Transform rootHero;
    

    private bool isRotateStick = false;
    private bool isHit = false;
    private bool isReady = true;
   
    private float angle = 0f;   
    private float heroPosX;
    private float heroPosY = -0.685f;    

    private int groundLayer;
    private int bonusLayer;
    private int stickLayer;

    private Vector3 currentColPoision;
    private Vector3 nextColPosition;
    private Vector3 prevColPosition;
    private Vector3 heroPosition;


    public bool isShift = false;
    public bool isGame = false;  


    private HeroState StateAnimation
    {
        get { return (HeroState)heroAnimator.GetInteger("State"); }
        set { heroAnimator.SetInteger("State", (int)value); }
    }  

  
    void Start ()
    {
        bonusLayer = LayerMask.GetMask("Bonus");
        groundLayer = LayerMask.GetMask("Ground");
        stickLayer = LayerMask.GetMask("Stick");

        heroAnimator = hero.GetComponent<Animator>();
        heroTransform = hero.transform;
        rootHero = heroTransform.Find("root");

        currentColumn = GameObject.Find("Column").GetComponent<Transform>();
        nextColumn = GameObject.Find("Column_1").GetComponent<Transform>();
        prevColumn = GameObject.Find("Column_2").GetComponent<Transform>();

        heroPosition = new Vector3(-3f, heroPosY, 0f);
        currentColPoision = new Vector3(-3f, -3f, 0f);
        prevColPosition = new Vector3(-6f, -3f, 0f);           

        nextColumn.Find("Colum").transform.localScale = new Vector3(Random.Range(0.3f, 1.5f), 1, 1);
        nextColPosition = new Vector3(Random.Range(0.5f, 1.5f), -3, 0);        
    }	

	
	void Update () {       
        if(isReady && isGame && !isShift)
        {                      
            if (Input.GetMouseButtonDown(0))
            {
                if (stick == null)
                {
                    
                    audioManager.Play(AudioState.StickGrow);
                    stick = Instantiate<GameObject>(stickPrefab);
                    stick.transform.position = prevColumn.Find("Colum").transform.Find("StickPosition").transform.position;
                }
            }
            if (stick != null)
            {
                if (Input.GetMouseButton(0))
                {
                    if(stick.transform.localScale.y < MAX_STICK_SCALE)
                    {
                        stick.transform.localScale += new Vector3(0, 4f, 0);
                    }                    
                }
                if (Input.GetMouseButtonUp(0))
                {
                    audioManager.Play(AudioState.Stop);
                    isRotateStick = true;
                    isReady = false;
                }
            }                   
        }
        if (isRotateStick)
        {
            StickRotate();
        }

        if (isShift)
        {
            currentColumn.position = Vector3.Lerp(currentColumn.position, currentColPoision, HERO_SPEED * Time.deltaTime);
            nextColumn.position = Vector3.Lerp(nextColumn.position, nextColPosition, HERO_SPEED * Time.deltaTime);
            prevColumn.position = Vector3.Lerp(prevColumn.position, prevColPosition, HERO_SPEED * Time.deltaTime);
            heroTransform.position = Vector3.Lerp(heroTransform.position, heroPosition, HERO_SPEED * Time.deltaTime);
        }  
                
        if (currentColumn.position.x < LEFT_MARGIN)
        {
            isShift = !isShift;
            SwapColumn();            
            isReady = true;
        }
    }


    void StickRotate()
    {
        stick.transform.Rotate(0, 0, -FALL_STICK_SPEED);
        angle += FALL_STICK_SPEED;
        if (angle >= 90)
        {                    
            angle = 0;
            isRotateStick = false;
            isHit = HasBeenHit();           
            StartCoroutine(MoveToPosition(heroPosition.x));                                 
        }
    } 


    void SwapColumn()
    {              
        Transform temp = currentColumn;
        currentColumn = nextColumn;
        nextColumn = prevColumn;
        prevColumn = temp;

        nextColumn.position = SPAWN_VECTOR;
        nextColumn.Find("Colum").transform.localScale = new Vector3(Random.Range(0.3f, 1.5f), 1, 1);
        float topRange = 2.5f - nextColumn.Find("Colum").transform.localScale.x;
        nextColPosition = new Vector3(Random.Range(-1.7f, topRange), -3, 0);
    }    


    bool HasBeenHit()
    {        
        Vector3 stickTopPos = stick.transform.Find("TopPosition").transform.position;
        var info = Physics2D.Raycast(stickTopPos, Vector2.down, 0.2f, groundLayer | bonusLayer);       
        var col = info.collider;
        if (col != null)
        {      
            if(col.tag == "Bonus")
            {
                audioManager.Play(AudioState.Bonus);
                ScoreManager.score += 2;
            }
            else if(col.tag == "Ground")
            {
                audioManager.Play(AudioState.FallStick);
                ScoreManager.score += 1;
            }            
            heroPosX = currentColumn.transform.position.x;
            heroPosition = new Vector3(heroPosX, heroPosY, 0);
            return true;
        }
        audioManager.Play(AudioState.FallStick);
        heroPosX = stick.transform.Find("TopPosition").transform.position.x;
        heroPosition = new Vector3(heroPosX, heroPosY, 0);           
        return false;
    }


    IEnumerator MoveToPosition(float targetPosX)
    {
        while (heroTransform.position.x < targetPosX)
        {
            StateAnimation = HeroState.Run;            
            var info = Physics2D.Raycast(rootHero.position, Vector2.down, 0.2f, groundLayer | stickLayer | bonusLayer);
            var col = info.collider;
            if (col != null)
            {
                heroTransform.Translate(Vector3.right * HERO_SPEED * Time.deltaTime);
            }        
            yield return null;
        }

        if (!isHit)
        {
            StateAnimation = HeroState.Jump;
            stick.GetComponent<Rigidbody2D>().isKinematic = false;
            stick.transform.Find("Stick").GetComponent<Collider2D>().isTrigger = true;           
            heroTransform.GetComponent<Collider2D>().isTrigger = true;            
        }
        else
        {
            StateAnimation = HeroState.Idle;
            audioManager.Play(AudioState.Score);
            heroPosition = new Vector3(-3f, heroPosY, 0f);            
            isShift = true;
            Destroy(stick);           
        }  
    }   


    void GameOver()
    {
       
        heroPosition = new Vector3(-3f, heroPosY, 0f); 
        if (ScoreManager.score > PlayerPrefs.GetInt("Score"))
        {
            PlayerPrefs.SetInt("Score", ScoreManager.score);
        }
        scoreLabel.text = "Score : " + ScoreManager.score.ToString();
        bestScoreLabel.text = "Best : " + PlayerPrefs.GetInt("Score").ToString();
        ScoreManager.score = 0;       
        isGame = false;
        restartDialog.SetActive(true);
        Destroy(stick);
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        StateAnimation = HeroState.Idle;
        audioManager.Play(AudioState.Death);       
        heroTransform.GetComponent<Collider2D>().isTrigger = false;        
        heroTransform.position = new Vector3(currentColumn.position.x, heroPosY, 0);
        GameOver();
    }
}

public enum HeroState
{
    Idle,
    Run,
    Jump
}

