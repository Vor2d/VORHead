// script to render explosion
using UnityEngine;
using System.Collections;

public class GraphicExplosion : MonoBehaviour {

    [SerializeField] private AudioSource explosion_SE;

    public float loopduration;

    private float ramptime=0;
    private float alphatime=1;

    private void Start()
    {
        this.explosion_SE = GetComponent<AudioSource>();

        StartCoroutine(SE_fadeout());
    }

    void Update () {
		Destroy(gameObject, loopduration);
        ramptime+=Time.deltaTime*2;
        alphatime-=Time.deltaTime;		
        float r = Mathf.Sin((Time.time / loopduration) * (2 * Mathf.PI)) * 0.5f + 0.25f;
        float g = Mathf.Sin((Time.time / loopduration + 0.33333333f) * 2 * Mathf.PI) * 0.5f + 0.25f;
        float b = Mathf.Sin((Time.time / loopduration + 0.66666667f) * 2 * Mathf.PI) * 0.5f + 0.25f;
        float correction = 1 / (r + g + b);
        r *= correction;
        g *= correction;
        b *= correction;
        GetComponent<Renderer>().material.SetVector("_ChannelFactor", new Vector4(r,g,b,0));
        GetComponent<Renderer>().material.SetVector("_Range", new Vector4(ramptime,0,0,0));
        GetComponent<Renderer>().material.SetFloat("_ClipRange", alphatime);


	}

    public IEnumerator SE_fadeout()
    {
        float timer = loopduration;

        while(timer >= 0.0f)
        {
            explosion_SE.volume = timer / loopduration;
            timer -= Time.deltaTime;

            yield return null;
        }
    }
}
