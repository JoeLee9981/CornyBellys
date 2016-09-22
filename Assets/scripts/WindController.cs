using UnityEngine;
using System.Collections;

public class WindController : MonoBehaviour {

    [SerializeField] private AudioClip windAudio;
    [SerializeField] private int windInterval = 10;
    private AudioSource audioSource;
    private int nextWindTime = 10;
    private float elapsedWindTime = 0;

    // Use this for initialization
    void Start () {
        audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (nextWindTime < elapsedWindTime) {
            audioSource.clip = windAudio;
            audioSource.PlayOneShot(audioSource.clip);
            elapsedWindTime += Time.fixedDeltaTime;
            nextWindTime = Random.Range(5, 15) * windInterval;
        }
        else {
            elapsedWindTime += Time.fixedDeltaTime;
        }
    }
}
