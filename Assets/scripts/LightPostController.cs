﻿using UnityEngine;
using System.Collections;

public class LightPostController : MonoBehaviour {

    [SerializeField] private bool shouldFlicker = false;
    //duration of flickering
    [SerializeField] private float flickerInterval = 1;
    //duration of steady light
    [SerializeField] private float steadyInterval = 1.5f;
    //duration of individual flicker
    [SerializeField] private float maxFlickerDuration = 0.1f;

    private Renderer lightBulb;
    private Light light;
    
    private float lightIntensity;
    private bool flickeringActive;
    //time for individual flicker
    private float elapsedFlickerTime;
    //time for entire flicker interval duration
    private float elapsedFlickerInterval;
    //next time for individual flicker
    private float nextFlickerTime;
    //next interval to switch from flicker to steady
    private float nextFlickerInterval;

	// Use this for initialization
	void Start () {
        light = GetComponentInChildren<Light>();
        lightIntensity = light.intensity;
        flickeringActive = true;

        elapsedFlickerTime = 0;
        elapsedFlickerInterval = 0;
        nextFlickerTime = 0;
        nextFlickerInterval = Random.Range(0.1f, steadyInterval);
	}
	
	// Update is called once per frame
	void Update () {
        if(lightBulb == null) {
            lightBulb = transform.Find("Bulb").GetComponent<Renderer>();
        }
        Material material = lightBulb.material;
        
        if (shouldFlicker) {
            if (elapsedFlickerInterval > nextFlickerInterval) {
                if (flickeringActive) {
                    //light is on, flicker it off
                    flickeringActive = false;
                    //set interval to set steady next
                    nextFlickerInterval = Random.Range(0.1f, flickerInterval);
                    //set time to change next individual flicker
                    nextFlickerTime = Random.Range(0.025f, maxFlickerDuration);
                    //flicker the light
                    light.intensity = 0;
                    material.SetColor("_EmissionColor", Color.white);
                }
                else {
                    //light is off, set it to steady
                    flickeringActive = true;
                    //set interval to activate flickering next
                    nextFlickerInterval = Random.Range(0.1f, steadyInterval);
                }
                elapsedFlickerInterval = 0;
            }
            else if (!flickeringActive && elapsedFlickerTime > nextFlickerTime) {
                //flicker the light
                light.intensity = light.intensity == 0 ? lightIntensity : 0;
                material.SetColor("_EmissionColor", light.intensity == 0 ? Color.black : Color.white);
                //set the next flicker time
                nextFlickerTime = Random.Range(0.025f, maxFlickerDuration);
                elapsedFlickerTime = 0;
            }
            else {
                //update timers
                elapsedFlickerTime += Time.deltaTime;
                elapsedFlickerInterval += Time.deltaTime;
            }
        }
    }
}