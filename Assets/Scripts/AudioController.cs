using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioSource fireSound;
    public AudioSource reloadSound;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SoundOfReload()
    {
        reloadSound.Play();
    }
    public void SoundOfFire()
    {
        fireSound.Play();
    }
}
