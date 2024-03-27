using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    enum GameState { OPENING, GAME, ENDING }
    enum PauseState { OFF, KEY_RESUME, ON }

    PauseState pause = PauseState.OFF;

    GameObject  racket;
    Slider      slider;

    void Start()
    {
        racket = GameObject.Find("Racket");
        slider = GameObject.Find("Slider").GetComponent<Slider>();
    }

    void Update()
    {
        KeyInput();
        racket.transform.position = new Vector3(slider.value,
            racket.transform.position.y, racket.transform.position.z);    
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

}
