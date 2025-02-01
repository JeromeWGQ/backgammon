using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        
    }

    public AudioSource audioSource;
    public AudioClip dropSound; // 落子声音
    public AudioClip diceSound; // 掷骰子声音
    public AudioClip selectSound; // 选择
    public AudioClip cancelSound; // 取消

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayDropSound()
    {
        audioSource.clip = dropSound;
        audioSource.Play();
    }

    public void PlayDiceSound()
    {
        audioSource.clip = diceSound;
        audioSource.Play();
    }

    public void PlaySelectSound()
    {
        audioSource.clip = selectSound;
        audioSource.Play();
    }

    public void PlayCancelSound()
    {
        audioSource.clip = cancelSound;
        audioSource.Play();
    }

}
