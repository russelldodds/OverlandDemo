using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeHandler : MonoBehaviour {
    public GameObject fadeImage;
    public float fadeSpeed = 1.0f;
    Image image;

    void Start() {
        image = fadeImage.GetComponent<Image>();
        StartCoroutine(FadeOut());
    }

    public IEnumerator FadeOut() {
        yield return new WaitForEndOfFrame();         
        
        // reset the image state
        image.gameObject.SetActive(true);
        image.canvasRenderer.SetAlpha(1f);

        // wait 1 frame so that the alpha is actually set
        yield return new WaitForEndOfFrame();         
        image.CrossFadeAlpha(0f, fadeSpeed, false);

        // once done inactivate again
        yield return new WaitForSeconds(fadeSpeed);
        image.gameObject.SetActive(false);
    }
}
