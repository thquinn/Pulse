using System.Collections.Generic;
using UnityEngine;

public class SFXScript : MonoBehaviour {
    static SFXScript instance;

    public AudioSource audioSource, audioSourceBulletDie;
    public AudioClip sfxShoot, sfxPlayerHurt, sfxDash, sfxPulseDissolve;

    void Start() {
        instance = this;
    }

    void Update() {

    }

    public static void SFXShoot() {
        instance.audioSource.PlayOneShot(instance.sfxShoot, 0.1f);
    }
    public static void SFXPlayerHurt() {
        instance.audioSource.PlayOneShot(instance.sfxPlayerHurt, 1f);
    }
    public static void SFXDash() {
        instance.audioSource.PlayOneShot(instance.sfxDash, 0.05f);
    }
    public static void SFXPulseDissolve() {
        instance.audioSource.PlayOneShot(instance.sfxPulseDissolve, 1f);
    }
    public static void SFXBulletDie(Vector2 position) {
        instance.audioSourceBulletDie.transform.localPosition = position;
        instance.audioSourceBulletDie.PlayOneShot(instance.audioSourceBulletDie.clip);
    }
}
