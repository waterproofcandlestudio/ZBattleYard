using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSFXManager : MonoBehaviour
{
    /// Referencia para entrar a este script desde fuera
    public static PlayerSFXManager instance;
    // Asegura que la instancia del AudioManager esté linkeado a este script para poder acceder a el desde fuera
    void Awake() { instance = this; }

    // Regula si 1 sonido corta al anterior tras sonar...
    //private static bool cutPlayerSFX = true;
    // Regula mediante el largo d la pista q el "gameObject" del sonido se destruya aunq no destruya al q sonó antes...
    private static float playerSFXLength;

    /// SFX List
    [Header("SFX")]
    public string ª;
    public AudioClip sfx_ShotAk;

    /// Current SFX Object
    [Header("Current SFX")]
    public GameObject currentSFXObject;

    /// Sound Object
    [Header("Sound Reference")]
    public GameObject referenceSoundObject;

    public void PlaySFX(string sfxName)
    {
        //switch (sfxName)
        //{
        //    case "sfx_ShotAk": SoundObjectCreation(sfx_ShotAk); break;

        //    default: break;
        //}
    }

    /// Object Creations
    void SoundObjectCreation(AudioClip clip)
    {
        playerSFXLength = clip.length;
        // Creo un SoundObject gameobject dentro d la escena
        currentSFXObject = Instantiate(referenceSoundObject, transform);
        // Le asigno un "audioClip" a su "AudioSource"
        currentSFXObject.GetComponent<AudioSource>().clip = clip;
        // Ejecuto el audio
        currentSFXObject.GetComponent<AudioSource>().Play();
        // Revisar si hay otro "gameObject" de otro sonido. Si lo hay lo elimina
        if (currentSFXObject == true) { Destroy(currentSFXObject, playerSFXLength); }
    }

    /// EFECTOS / MODIFICACIONES
    // Hacer efecto
    public void Loop()  { currentSFXObject.GetComponent<AudioSource>().loop = true; }

    /* El false desactiva el destroy de "SoundObjectCreation" para q no corte el sonido anterior...
     * 
     * El "Destroy(currentSFXObject, playerSFXpLength);" elimina el sonido tras finalizar su duración
     *  && no corta al siguiente (Básicamente ==> Corte automático con el tiempo mediante la duración del sonido...)
    */
    //public void NoCut() { cutPlayerSFX = false;     Destroy(currentSFXObject, playerSFXLength); } 

    // Deshacer efecto
    public void NoLoop(){ currentSFXObject.GetComponent<AudioSource>().loop = false; }

    //public void Cut()   { cutPlayerSFX = false; }
    public void Stop() { Object.DestroyImmediate(currentSFXObject, true); }
    public void RePlay() { currentSFXObject.SetActive(true); }
}
