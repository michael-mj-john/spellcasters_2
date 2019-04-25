using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnnouncerEvents : MonoBehaviour {

    public AudioClip intro, countdown, roundStart, knockOut, vanquished, putOnHat, victory, defeat, nine, eight, seven, six, five, four, three, two, one;
    private AudioSource speaker;
	// Use this for initialization
	void Start () {
        speaker = this.GetComponent<AudioSource>();		
	}
    private void Awake()
    {
        if(speaker == null) speaker = this.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void PlaySound(string soundName)
    {
        AudioClip audioClip = null;
        switch (soundName)
        {
            case "intro":
                audioClip = intro;
                break;
            case "countdown":
                audioClip = countdown;
                break;
            case "roundStart":
                audioClip = roundStart;
                break;
            case "knockOut":
                audioClip = knockOut;
                break;
            case "vanquished":
                audioClip = vanquished;
                break;
            case "putOnHat":
                audioClip = putOnHat;
                break;
            case "victory":
                audioClip = victory;
                break;
            case "defeat":
                audioClip = defeat;
                break;
            case "nine":
                audioClip = nine;
                break;
            case "eight":
                audioClip = eight;
                break;
            case "seven":
                audioClip = seven;
                break;
            case "six":
                audioClip = six;
                break;
            case "five":
                audioClip = five;
                break;
            case "four":
                audioClip = four;
                break;
            case "three":
                audioClip = three;
                break;
            case "two":
                audioClip = two;
                break;
            case "one":
                audioClip = one;
                break;
        }

        if (audioClip != null)
        {
            speaker.Stop();
            speaker.clip = audioClip;
            speaker.Play();
        }
    }

    [PunRPC]
    public void PlaySoundToOthers(AudioClip audioClip)
    {
        
    }
}
