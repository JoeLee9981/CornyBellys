using UnityEngine;
using System.Collections;

public class GameManager {

    public delegate void GameOverHandler();

    private static GameManager instance;

    public event GameOverHandler OnGameOver;

    public bool VRActive { get; set; }
    public bool UseVR { get; set; }
    public bool ViveConnected { get; set; }
    public bool OculusConnected { get; set; }
    public bool MotionControlsConnected { get; set; }
    public GameObject VRControllerRight { get; set; }
    public GameObject VRControllerLeft { get; set; }
    public VRPlayerController PlayerCharacter { get; set; }
    public GameObject CellPhone { get; set; }
    public VRCustomInputManager InputManager { get; set; }
    public bool IsPaused { get; set; }
    public bool GameOver { get; set; }

    private GameManager() {
        IsPaused = false;
        GameOver = false;
    }

    public static GameManager getInstance() {
        if(instance == null) {
            instance = new GameManager();
        }
        return instance;
    }

    public bool IsGameAcitve() {
        return !IsPaused && !GameOver;
    }

    public void TriggerGameOver() {
        if(OnGameOver != null) {
            OnGameOver();
        }
    }
}
