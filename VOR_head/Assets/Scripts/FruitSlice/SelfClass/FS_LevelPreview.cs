using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FS_LevelPreview : MonoBehaviour 
{
    [SerializeField] private Transform Mesh_TRANS;
    [SerializeField] private Transform BG_mesh_TRANS;
    [SerializeField] private float Scale_thresh;    //Deactivate scale threshold;
    [SerializeField] private Transform DummyMesh;
    [SerializeField] private FS_StarPanel SP_script;
    [SerializeField] private bool Using_show_star;
    [SerializeField] private int Star_SO_offset;
    [SerializeField] private int Lock_SO_offset;
    [SerializeField] private int BG_SO_offset;
    [SerializeField] private bool Using_lock_dim;
    [SerializeField] private Color Lock_color_offset;
    [SerializeField] private FS_LockPanel LOP_script;
    [SerializeField] private Color Panel_color_offset;
    [SerializeField] private bool Lock_turn_off_star;
    
    public int Level_index { get; set; }
    private Texture2D texture;
    private int sorting_order;
    private bool activated;
    private bool unlocked;
    private int stars;
    private Color color;
    private int requ_star;

    private void Awake()
    {
        this.texture = null;
        this.sorting_order = 0;
        this.activated = false;
        this.unlocked = false;
        this.stars = -1;
        this.Level_index = 0;
        this.color = Color.white;
        this.requ_star = 0;
    }

    private void Start()
    {
        deactivate_debug();
    }

    private void deactivate_debug()
    {
        DummyMesh.gameObject.SetActive(false);
    }

    public void init_LP(Texture2D _texture, float mesh_size, float PPU, int _level_index, int star_max,
        int _requ_star)
    {
        texture = _texture;
        adjust_mesh(_texture, mesh_size, PPU);
        Level_index = _level_index;
        init_SP(star_max);
        //spawn_stars(0);
        init_LOP(_requ_star);
    }

    private void init_SP(int max_num)
    {
        SP_script.init_SP(max_num);
    }

    private void init_LOP(int _requ_star)
    {
        LOP_script.init_LP(_requ_star);
        requ_star = _requ_star;
    }

    private void adjust_mesh(Texture2D _texture, float mesh_size, float PPU)
    {
        float ratio = GeneralMethods.mesh_size_cal_ratio(_texture, mesh_size, PPU).Item2;
        Mesh_TRANS.GetComponent<SpriteRenderer>().sprite = GeneralMethods.texture_to_sprite(_texture);
        Mesh_TRANS.GetComponent<SpriteRenderer>().sortingLayerName = FS_SD.SortingLayer_GO;
        Mesh_TRANS.localScale *= ratio;
    }

    public void set_SO(int SO)
    {
        set_mesh_SO(SO);
        set_star_SO(SO + Star_SO_offset);
        set_lock_SO(SO + Lock_SO_offset);
        set_BG_SO(SO + BG_SO_offset);
        sorting_order = SO;
    }

    private void set_mesh_SO(int SO)
    {
        Mesh_TRANS.GetComponent<SpriteRenderer>().sortingOrder = SO;
    }

    private void set_star_SO(int SO)
    {
        SP_script.set_SO(SO);
    }

    private void set_lock_SO(int SO)
    {
        LOP_script.set_SO(SO);
    }

    private void set_BG_SO(int SO)
    {
        BG_mesh_TRANS.GetComponent<SpriteRenderer>().sortingOrder = SO;
    }

    public void activate()
    {
        if (activated) { return; }
        activated = true;
        turn_on_mesh();
    }

    private void turn_on_mesh()
    {
        gameObject.SetActive(true);
    }

    public void deactivate()
    {
        activated = false;
        turn_off_mesh();
    }

    private void turn_off_mesh()
    {
        gameObject.SetActive(false);
    }

    public void transfer_to(FS_PreviewFrameInfo PFI, float time)
    {
        activate();
        Vector3 pos = new Vector3(PFI.Posisiton.x, PFI.Posisiton.y, transform.position.z);
        StartCoroutine(transfer_to_coro(pos, PFI.Scale_size, PFI.Render_order, time, PFI.Global_color));
    }

    private IEnumerator transfer_to_coro(Vector3 pos, float scale, int RO, float time, Color _color)
    {
        Vector3 pos_step = (pos - transform.position) / time;
        float scale_step = (scale - transform.localScale.x) / time;
        float SO_step = (float)(RO - sorting_order) / time;
        Color color_step = (_color - color) / time;
        while(time > 0)
        {
            set_SO(Mathf.RoundToInt(sorting_order + SO_step * Time.deltaTime));
            transform.position += pos_step * Time.deltaTime;
            transform.localScale += (scale_step * Time.deltaTime) * Vector3.one;
            set_color(color + color_step * Time.deltaTime);
            time -= Time.deltaTime;
            yield return null;
        }
        TT_CB(scale);
    }

    /// <summary>
    /// Transfer to coroutine call back;
    /// </summary>
    private void TT_CB(float scale)
    {
        check_scale(scale);
    }

    private void set_color(Color _color)
    {
        Color target_color = _color;
        if (Using_lock_dim && !unlocked) { target_color -= Lock_color_offset; }
        set_mesh_color(target_color);
        set_star_caled_color(_color);
        set_lock_color(_color);
        set_BG_color(target_color + Panel_color_offset);
        color = _color;
    }

    private void set_mesh_color(Color _color)
    {
        Mesh_TRANS.GetComponent<SpriteRenderer>().color = _color;
    }

    private void set_star_caled_color(Color _color)
    {
        Color caled_color = unlocked ? _color : _color - Lock_color_offset;
        set_star_color(caled_color);
    }

    private void set_star_color(Color _color)
    {
        SP_script.set_color(_color);
    }

    private void set_lock_color(Color _color)
    {
        LOP_script.set_color(_color);
    }

    private void set_BG_color(Color _color)
    {
        BG_mesh_TRANS.GetComponent<SpriteRenderer>().color = _color;
    }


    /// <summary>
    /// Set tp PFI instantly;
    /// </summary>
    public void set_to(FS_PreviewFrameInfo PFI)
    {
        Vector3 pos = new Vector3(PFI.Posisiton.x, PFI.Posisiton.y, transform.position.z);
        transform.position = pos;
        transform.localScale = PFI.Scale_size * Vector3.one;
        set_SO(PFI.Render_order);
        set_color(PFI.Global_color);
    }

    public void check_scale()
    {
        if (transform.localScale.x < Scale_thresh) { deactivate(); }
        else { activate(); }
    }

    public void check_scale(float scale)
    {
        if (scale < Scale_thresh) { deactivate(); }
        else { activate(); }
    }

    public void update_info(FS_PlayerLevelInfo PLI)
    {
        unlocked = PLI.Unlocked;
        update_unlock(unlocked);
        int temp_stars = stars;
        stars = PLI.Stars;
        update_stars(stars, temp_stars);
    }

    private void update_unlock(bool _unlocked)
    {
        unlocked = _unlocked;
        if (_unlocked) { LOP_script.turn_off_mesh(); }
        else { LOP_script.turn_on_mesh(); }
        set_color(color);
    }

    private void update_stars(int _stars, int last_star)
    {
        if(!Using_show_star) { return; }
        spawn_stars(_stars);
    }

    private void spawn_stars(int _stars)
    {
        clear_stars();
        if(!Lock_turn_off_star || (Lock_turn_off_star && unlocked))
        {
            SP_script.spawn_star_inst(_stars);
            set_star_SO(sorting_order + Star_SO_offset);
            set_star_caled_color(color);
        }
    }

    private void clear_stars()
    {
        SP_script.clear_panel();
    }
}
