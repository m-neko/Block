using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    enum GameState { OPENING, GAME, GAMEOVER}
    enum PauseState { OFF, KEY_RESUME, ON }

    // 定数
    const bool  HISCORE_RESET = false;  // 起動時にハイスコアをリセットするか
    const float RACKET_SPEED = 8.0f;    // ラケットの移動速度
    const float BALL_SPEED = 2.0f;      // Stage1のボール速度
    const int   BALL_COUNT = 3;         // ボール残数
    const int   STAGE_ADD_SCORE = 20;   // ステージが進むごとに加算する1ブロックあたりの得点
    const float STAGE_ADD_SPEED = 0.5f; // ステージが進むごとに増加するボールの速度
    const int   BLOCK_X = 5;            // 横方向のブロック数
    const int   BLOCK_Y = 3;            // 縦方向のブロック数

    // ゲーム管理
    bool        hiscoreReset;
    int         stage;
    int         score;
    int         hiscore;
    int         addscore;
    int         blockCount;
    int         ballCount;
    float       ballSpeed;
    GameState   gameState;
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

        ball.GetComponent<Renderer>().enabled = false;

        hiscoreReset = HISCORE_RESET;
        stage = 1;
        score = 0;
        ballSpeed = BALL_SPEED;
        blockCount = 0;
        ballCount = BALL_COUNT;
        addscore = STAGE_ADD_SCORE;
        gameState = GameState.OPENING;
        if(!hiscoreReset && PlayerPrefs.HasKey("HiScore")){
            hiscore = PlayerPrefs.GetInt("HiScore");
        }else{
            hiscore = 0;
        }

        txtMain.text = "";
        txtHiScore.text = hiscore.ToString();
        txtScore.text = score.ToString();
        GameObject.Find("Canvas").transform.Find("Aspect Controller")
            .transform.Find("Panel").gameObject.SetActive(true);
        
        GameResume();
    }

    void GameStart()
    {
        txtMain.text = "";
        for(int i=0; i<BLOCK_X; i++){
            for(int j=0; j<BLOCK_Y; j++){
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
        // ポーズが解除されている場合のみ入力を受け付ける
        if(pause == PauseState.OFF){
            switch(gameState){
                case GameState.OPENING:
                    if(Input.GetKey(KeyCode.Return)) OpeningEnter();
                    break;
                case GameState.GAME:
                    // ポーズ処理
                    if(Input.GetKeyDown(KeyCode.Space)){
                        txtMain.text = "PAUSE";
                        GamePause(true);
                    }
                    
                    // 入力処理
                    if(Input.GetKey(KeyCode.LeftArrow) && slider.value > slider.minValue){
                        slider.value -= RACKET_SPEED * Time.deltaTime;
                    }
                    if(Input.GetKey(KeyCode.RightArrow) && slider.value < slider.maxValue){
                        slider.value += RACKET_SPEED * Time.deltaTime;
                    }
                    break;
            }

        }else if(pause == PauseState.KEY_RESUME){
            if(Input.GetKeyDown(KeyCode.Space)){
                txtMain.text = "";
                GameResume();
            }
        }else if(pause == PauseState.ON){
            if(gameState == GameState.GAMEOVER && Input.GetKey(KeyCode.Return)) Initialize();
        }

        // 終了
        if(Input.GetKeyDown(KeyCode.Escape)) Application.Quit();

    }

    void OpeningEnter()
    {
        GameObject.Find("Panel").SetActive(false);
        gameState = GameState.GAME;
        ball.GetComponent<Renderer>().enabled = true;
        GameStart();
    }

    void GameClear()
    {
        txtMain.text = "STAGE CLEAR!!";
        GameObject.Find("SndClear").GetComponent<AudioSource>().Play();
        StartCoroutine(StageInit());
    }

    IEnumerator StageInit()
    {
        GamePause(false);
        yield return new WaitForSecondsRealtime(2);
        txtMain.text = "";
        stage++;
        ballSpeed += STAGE_ADD_SPEED;
        addscore += STAGE_ADD_SCORE; 
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
            StartCoroutine(BallInit());
        }else{
            txtMain.text = "GAME OVER";
            gameState = GameState.GAMEOVER;
            GamePause(false);
        }
    }

    IEnumerator BallInit()
    {
        yield return new WaitForSecondsRealtime(1);
        ball.transform.position = new Vector3(0.0f,1.0f,0.0f);
        ball.GetComponent<Rigidbody2D>().velocity = new Vector2(-1.0f*ballSpeed,-1.0f*ballSpeed);
    }

    public void OnPressButton(){
        switch(gameState){
            case GameState.OPENING:
                OpeningEnter();
                break;
            case GameState.GAME:
                if(pause == PauseState.OFF){
                    txtMain.text = "PAUSE";
                    GamePause(true);                    
                }else if(pause == PauseState.KEY_RESUME){
                    txtMain.text = "";
                    GameResume();
                }
                break;
            case GameState.GAMEOVER:
                Initialize();
                break;
        }
    }

}
