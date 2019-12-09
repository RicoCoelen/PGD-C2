using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioClip playerJump, playerLand, playerHit, chainHit, death, enemyShot, enemyHit;
    static AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        playerJump = Resources.Load<AudioClip>("Jump");
        playerLand = Resources.Load<AudioClip>("Ground");
        playerHit = Resources.Load<AudioClip>("player_hurt");
        chainHit = Resources.Load<AudioClip>("Chain_Hit");
        death = Resources.Load<AudioClip>("death");
        enemyShot = Resources.Load<AudioClip>("gun_shot");
        enemyHit = Resources.Load<AudioClip>("Hit_Enemy");


        audioSource = GetComponent<AudioSource>();
    }

    public static void PlaySound(string clip)
    {
        switch (clip)
        {
            case "Jump":
                audioSource.PlayOneShot(playerJump);
                break;
            case "Land":
                audioSource.PlayOneShot(playerLand);
                break;
            case "PlayerHit":
                audioSource.PlayOneShot(playerHit);
                break;
            case "ChainHit":
                audioSource.PlayOneShot(chainHit);
                break;
            case "Death":
                audioSource.PlayOneShot(death);
                break;
            case "EnemyShot":
                audioSource.PlayOneShot(enemyShot);
                break;
            case "EnemyHit":
                audioSource.PlayOneShot(enemyHit);
                break;
        }
    }
}
