using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    enum GameState { OPENING, GAME, ENDING }
    enum PauseState { OFF, KEY_RESUME, ON }

    // ゲーム管理
    int         stage;
    int         score;
    int         hiscore;
    PauseState  pause = PauseState.OFF;

    // オブジェクト
    GameObject  ball;
    GameObject  racket;
    Text        txtHiScore;
    Text        txtScore;
    Text        txtStage;
    Slider      slider;

    void Start()
    {
        ball = GameObject.Find("Ball");
        racket = GameObject.Find("Racket");
        txtHiScore = GameObject.Find("HiScore").GetComponent<Text>();
        txtScore = GameObject.Find("Score").GetComponent<Text>();
        txtStage = GameObject.Find("Stage").GetComponent<Text>();
        slider = GameObject.Find("Slider").GetComponent<Slider>();
        stage = 1;
        score = 0;
        hiscore = 0;
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

    void GameStart()
    {
        ball.GetComponent<Rigidbody2D>().velocity = new Vector2(-2.0f,-2.0f);
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
        score += 100;
        if(score > hiscore) hiscore = score;
    }

}
