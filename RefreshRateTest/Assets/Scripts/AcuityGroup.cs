using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AcuityGroup : MonoBehaviour
{
    [SerializeField] SpriteRenderer A_SR;

    private bool acuity_flashing;

    private void Awake()
    {
        this.acuity_flashing = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void change_A_sprite(int size)
    {
        Sprite sprite = RC.IS.AcuitySprites.Single(s => s.name == size.ToString());
        A_SR.sprite = sprite;
    }

    public void flash_acuity(float time)
    {
        if (!acuity_flashing) { StartCoroutine(flash_A_corotine(time)); }
    }

    private IEnumerator flash_A_corotine(float time)
    {
        acuity_flashing = true;
        A_SR.enabled = false;
        yield return new WaitForSeconds(1.0f);
        A_SR.enabled = true;
        yield return new WaitForSeconds(time);
        A_SR.enabled = false;
        yield return new WaitForSeconds(1.0f);
        A_SR.enabled = true;
        acuity_flashing = false;
    }
}
