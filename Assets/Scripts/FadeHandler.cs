using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeHandler : MonoBehaviour {

    public GameObject fadeImage;

    public float fadeSpeed =1.0f;

    // Start is called before the first frame update
    void Start() {
        fadeImage.SetActive(true);       
        fadeImage.GetComponent<Image>().CrossFadeAlpha(0f, fadeSpeed, false);
    }

    void OnDestroy() {
        fadeImage.GetComponent<Image>().CrossFadeAlpha(1f, 0, false);
        fadeImage.SetActive(false);
    }

}
