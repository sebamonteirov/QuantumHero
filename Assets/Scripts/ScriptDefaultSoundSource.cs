using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptDefaultSoundSource : MonoBehaviour
{
    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        StartCoroutine("destroySound");
    }

    private IEnumerator destroySound()
    {
        AudioClip audioClip;

        audioClip = audioSource.clip;
        
        yield return new WaitForSeconds(audioClip.length);
        
        Destroy(gameObject);
    }
}
