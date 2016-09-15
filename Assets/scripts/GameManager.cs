using UnityEngine;
using System.Collections;

public class GameManager {

    private static GameManager instance;

    public bool ViveConnected { get; set; }
    public bool OculusConnected { get; set; }
    public bool MotionControlsConnected { get; set; }

    private GameManager() {

    }

    public static GameManager getInstance() {
        if(instance == null) {
            instance = new GameManager();
        }
        return instance;
    }
}
