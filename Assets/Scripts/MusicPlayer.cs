using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour {

    public static MusicPlayer mp = null;

    void Awake()
    {
        if (mp != null)
        {
            Destroy(gameObject);
        }
        else
        {
            mp = this;
            GameObject.DontDestroyOnLoad(gameObject);
        }
    }


}
