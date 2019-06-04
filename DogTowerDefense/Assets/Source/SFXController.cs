using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXController : MonoBehaviour
{
    public static AudioClip BulletSound, DoggoBarkingSound, DoggoHurtSound, EnemyDamageSound, PlacingSound, StartButtonSound;
    static AudioSource audioSrc;

    //TO CALL SOUND USE: SFXController.PlaySound("FileName");
    
    // Start is called before the first frame update
    void Start()
    {
        BulletSound = Resources.Load<AudioClip>("Bullet");
        DoggoBarkingSound = Resources.Load<AudioClip>("DoggoBarking");
        DoggoHurtSound = Resources.Load<AudioClip>("DoggoHurt");
        EnemyDamageSound = Resources.Load<AudioClip>("EnemyDamage");
        PlacingSound = Resources.Load<AudioClip>("Placing");
        StartButtonSound = Resources.Load<AudioClip>("StartButton");

        audioSrc = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void PlaySound (string clip)
    {
        switch (clip)
        {
            case "Bullet":
                audioSrc.PlayOneShot(BulletSound);
                break;
            case "DoggoBarking":
                audioSrc.PlayOneShot(DoggoBarkingSound);
                break;
            case "DoggoHurt":
                audioSrc.PlayOneShot(DoggoHurtSound);
                break;
            case "EnemyDamage":
                audioSrc.PlayOneShot(EnemyDamageSound);
                break;
            case "Placing":
                audioSrc.PlayOneShot(PlacingSound);
                break;
            case "StartButton":
                audioSrc.PlayOneShot(StartButtonSound);
                break;
        }

    }
}
