using UnityEngine;
using System.Collections;

public class PlayerCharacter {

    public bool Exhausted;
    public float TimeExhausted;
    public bool Sprinting;
    public bool AltSprinting;
    public bool Sneaking;

    private float stamina;
    public float Stamina {
        get {
            return this.stamina;
        }
        set {
            if(value < 0) {
                this.stamina = 0;
            }
            else {
                this.stamina = value;
            }
            
            if(stamina == 0) {
                //if stamina set to 0, set Exhausted to true
                Exhausted = true;
                TimeExhausted = 0;
            }
        }
    }

    public PlayerCharacter(float stamina) {
        this.Stamina = stamina;
    }

    public bool IsSprinting() {
        return (Sprinting || AltSprinting) && !Exhausted;
    }

}
