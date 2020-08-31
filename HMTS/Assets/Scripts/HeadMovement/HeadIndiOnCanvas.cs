using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadIndiOnCanvas : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Camera camara;
    [SerializeField] private bool Using_range;

    private SpriteRenderer sprite_renderer;
    private Transform HI_TRANS;
    private bool activate;
    private float range;


    private void Awake()
    {
        this.sprite_renderer = gameObject.GetComponent<SpriteRenderer>();
        this.HI_TRANS = null;
        this.activate = false;
        this.range = DataController.IS.SystemSetting.HIOCRange;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (activate) { updatge_HIOC_pos(); }
    }

    public void init_HIOC(Transform _HI_TRANS)
    {
        set_HI(_HI_TRANS);
        turn_on_sprite();
        activate = true;
    }

    private void updatge_HIOC_pos()
    {
        Vector3 pos =
            GeneralMethods.world_to_canvas(HI_TRANS.position, camara, canvas, boundary: true);
        pos = Vector3Int.RoundToInt(pos);
        if(Using_range && Vector3.Distance(pos, Vector3.zero) > range)
        { turn_off_sprite(); }
        else { turn_on_sprite(); }
        GetComponent<RectTransform>().localPosition = pos;
    }

    private void set_HI(Transform _HI_TRANS)
    {
        HI_TRANS = _HI_TRANS;
    }

    public void turn_on_sprite()
    {
        sprite_renderer.enabled = true;
    }

    public void turn_off_sprite()
    {
        sprite_renderer.enabled = false;
    }
}
