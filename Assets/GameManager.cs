using UnityEngine;

public class GameManager : MonoBehaviour
{
    enum GameState { OPENING, GAME, ENDING }
    enum PauseState { OFF, KEY_RESUME, ON }

    PauseState pause = PauseState.OFF;

    void Start()
    {

    }

    void Update()
    {
        transform.Rotate(new Vector3(0,0,5.0f*Time.deltaTime));

        KeyInput();        
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
        }
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
