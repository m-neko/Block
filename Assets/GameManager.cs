using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    enum GameState { OPENING, GAME, ENDING }
    enum PauseState { OFF, KEY_RESUME, ON }

    // ゲーム管理
    bool        hiscoreReset = true;
    int         stage;
    int         score;
    int         hiscore;
    int         addscore;
    int         blockCount;
    int         ballCount;
    float       ballSpeed;
    PauseState  pause = PauseState.OFF;

    // オブジェクト
    public GameObject   block_pf;
    List<Color>         blockColorList;
    GameObject          ball;
    GameObject          racket;
    Text                txtHiScore;
    Text                txtScore;
    Text                txtStage;
    Text                txtBallCount;
    Text                txtMain;
    Slider              slider;

    void Start()
    {
        Initialize();

        stage = 1;
        score = 0;
        ballSpeed = 2.0f;
        blockCount = 0;
        ballCount = 3;
        addscore = 20;
        if(!hiscoreReset && PlayerPrefs.HasKey("HiScore")){
            hiscore = PlayerPrefs.GetInt("HiScore");
        }else{
            hiscore = 0;
        }

        txtHiScore.text = hiscore.ToString();
        txtScore.text = score.ToString();

        GameStart();
    }

    void Update()
    {
        txtHiScore.text = hiscore.ToString();
        txtScore.text = score.ToString();
        txtStage.text = stage.ToString();

        KeyInput();
        racket.transform.position = new Vector3(slider.value,
            racket.transform.position.y, racket.transform.position.z);    
    }

    void Initialize()
    {
        blockColorList = new List<Color>();
        blockColorList.Add(Color.red);
        blockColorList.Add(Color.green);
        blockColorList.Add(Color.blue);
        blockColorList.Add(Color.cyan);
        blockColorList.Add(Color.yellow);
        blockColorList.Add(Color.gray);
        blockColorList.Add(Color.magenta);

        ball = GameObject.Find("Ball");
        racket = GameObject.Find("Racket");
        txtHiScore = GameObject.Find("HiScore").GetComponent<Text>();
        txtScore = GameObject.Find("Score").GetComponent<Text>();
        txtStage = GameObject.Find("Stage").GetComponent<Text>();
        txtBallCount = GameObject.Find("BallCount").GetComponent<Text>();
        txtMain = GameObject.Find("MainText").GetComponent<Text>();
        slider = GameObject.Find("Slider").GetComponent<Slider>();
    }

    void GameStart()
    {
        txtMain.text = "";
        for(int i=0; i<5; i++){
            for(int j=0; j<3; j++){
                GameObject block = Instantiate(block_pf, new Vector3(-2.0f+i,3.5f-j*0.28f,0.0f), Quaternion.identity);
                int colorIndex = Random.RandomRange(0, blockColorList.Count);
                block.GetComponent<SpriteRenderer>().color = blockColorList[colorIndex];
                blockCount++;
            }
        }
        ball.transform.position = new Vector3(0.0f,1.0f,0.0f);
        ball.GetComponent<Rigidbody2D>().velocity = new Vector2(-1.0f*ballSpeed,-1.0f*ballSpeed);
    }

    void KeyInput()
    {
        // ポーズ処理
        if(Input.GetKeyDown(KeyCode.Space) && pause == PauseState.OFF)
            GamePause(true);
        else if(Input.GetKeyDown(KeyCode.Space) && pause == PauseState.KEY_RESUME)
            GameResume();
        
        // ポーズが解除されている場合のみ入力を受け付ける
        if(pause == PauseState.OFF){
            // 入力処理
            if(Input.GetKey(KeyCode.LeftArrow) && slider.value > slider.minValue){
                slider.value -= 8.0f * Time.deltaTime;
            }
            if(Input.GetKey(KeyCode.RightArrow) && slider.value < slider.maxValue){
                slider.value += 8.0f * Time.deltaTime;
            }

        }

        // 終了
        if(Input.GetKeyDown(KeyCode.Escape)) Application.Quit();

    }

    void GameClear()
    {
        txtMain.text = "STAGE CLEAR!!";
        GameObject.Find("SndClear").GetComponent<AudioSource>().Play();
        StartCoroutine(BallInit());
    }

    IEnumerator BallInit()
    {
        GamePause(false);
        yield return new WaitForSecondsRealtime(2);
        txtMain.text = "";
        stage++;
        ballSpeed += 0.5f;
        addscore += 20; 
        GameStart();
        GameResume();
    }

    public void GamePause(bool keyResume)
    {
        if(keyResume)
            pause = PauseState.KEY_RESUME;
        else
            pause = PauseState.ON;
        Time.timeScale = 0.0f;
    }

    public void GameResume()
    {
        pause = PauseState.OFF;
        Time.timeScale = 1.0f;       
    }

    public void HitBlock()
    {
        score += addscore;
        blockCount--;
        ball.GetComponent<Rigidbody2D>().velocity = new Vector2(-1.0f*ballSpeed,-1.0f*ballSpeed);
        if(score > hiscore){
            hiscore = score;
            PlayerPrefs.SetInt("HiScore",hiscore);
        }
        if(blockCount<=0) GameClear();
    }

    public void HitRacket()
    {
        Vector3 vec = transform.GetComponent<Rigidbody2D>().velocity;
        transform.GetComponent<Rigidbody2D>().velocity = new Vector3(-1.0f*ballSpeed,1.0f*ballSpeed,0);
    }

    public void MissRacket()
    {
        GameObject.Find("SndMiss").GetComponent<AudioSource>().Play();
        ballCount--;
        txtBallCount.text = ballCount.ToString();
        if(ballCount>=1){
            ball.transform.position = new Vector3(0.0f,1.0f,0.0f);
            ball.GetComponent<Rigidbody2D>().velocity = new Vector2(-1.0f*ballSpeed,-1.0f*ballSpeed);
        }else{
            txtMain.text = "GAME OVER";
            GamePause(false);
        }
    }

}
