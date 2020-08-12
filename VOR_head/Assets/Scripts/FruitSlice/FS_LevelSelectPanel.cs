using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FS_LevelSelectPanel : MonoBehaviour
{
    [SerializeField] private GameObject LP_prefab;
    [SerializeField] private Animator animator;
    [SerializeField] private int Sorting_order_base;
    [SerializeField] private int Sorting_order_scale;
    [SerializeField] private Color Last_color;

    private Dictionary<int, Transform> level_preview_TRANSs;
    private Dictionary<int, FS_PreviewFrameInfo> PFIs;  //Preview index, info;
    private Dictionary<int, int> PFI_to_LP; //Index link between PFI to LP;
    private int curr_index; //Current center LP index;
    private int side_number;
    private bool togglable;
    private bool selectable;
    private bool inited;
    private int level_num;

    public static FS_LevelSelectPanel IS;

    private void Awake()
    {
        IS = this;

        this.level_preview_TRANSs = new Dictionary<int, Transform>();
        this.PFIs = new Dictionary<int, FS_PreviewFrameInfo>();
        this.PFI_to_LP = new Dictionary<int, int>();
        this.curr_index = 0;
        this.side_number = 0;
        this.togglable = false;
        this.selectable = false;
        this.inited = false;
        this.level_num = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void init_panel(FS_TrialGroup[] levels_infos)
    {
        if (inited) { return; }
        level_num = levels_infos.Length;
        side_number = FS_Setting.IS.PreviewSideNumber;
        init_level_previews(levels_infos, side_number * 2 - 1);
        init_frame_info();
        inited = true;
    }

    private void init_frames()
    {
        curr_index = 0;
        LP_cal(curr_index, side_number);
        first_move_frame(side_number);
        update_info();
    }

    private void update_info()
    {
        int level_index = 0;
        foreach(Transform LP_TRANS in level_preview_TRANSs.Values)
        {
            level_index = LP_TRANS.GetComponent<FS_LevelPreview>().Level_index;
            LP_TRANS.GetComponent<FS_LevelPreview>().update_info(FS_RC.IS.player.get_level_info(level_index));
        }
    }

    private void first_move_frame(int side_num)
    {
        FS_PreviewFrameInfo PFI = null;
        Transform LP = null;
        for (int i = 0;i<side_num; i++)
        {
            PFI = PFIs[i];
            LP = level_preview_TRANSs[PFI_to_LP[i]];
            adjust_LP(LP, PFI);
            if(i != 0)
            {
                PFI = PFIs[-i];
                LP = level_preview_TRANSs[PFI_to_LP[-i]];
                adjust_LP(LP, PFI);
            }
        }
    }

    private void adjust_LP(Transform LP, FS_PreviewFrameInfo PFI)
    {
        LP.GetComponent<FS_LevelPreview>().set_to(PFI);
        LP.GetComponent<FS_LevelPreview>().check_scale();
    }

    /// <summary>
    /// Calculate which LP fit into which PFI;
    /// </summary>
    /// <param name="index">LP index for the center PFI;</param>
    private void LP_cal(int index, int side_num)
    {
        int LP_total_num = level_preview_TRANSs.Count;
        PFI_to_LP.Clear();
        for(int i = 0;i<side_num; i++)
        {
            PFI_to_LP[i] = GeneralMethods.index_mod(index + i, LP_total_num);
            if (i != 0) { PFI_to_LP[-i] = GeneralMethods.index_mod(index - i, LP_total_num); }
        }
    }

    private void init_frame_info()
    {
        int side_num = FS_Setting.IS.PreviewSideNumber;
        Vector2 panel_size = FS_Setting.IS.PreviewRotateSize;
        float max_M_size = 1.0f;
        Color last_color = Last_color;
        FS_PreviewFrameInfo temp_FI = null;
        for (int i = 0;i<side_num+1;i++)
        {
            Vector2 pos = frame_pos_cal(side_num, i, panel_size);
            float mesh_size = frame_size_cal(side_num, i, max_M_size);
            Color color = frame_color_cal(side_num, i, last_color);
            int RO = frame_renderoreder_cal(Sorting_order_base, i);
            temp_FI = new FS_PreviewFrameInfo(mesh_size, pos, RO, color);
            PFIs[i] = temp_FI;
            if(i != 0)
            {
                temp_FI = new FS_PreviewFrameInfo(mesh_size, -pos, RO, color);
                PFIs[-i] = temp_FI;
            }
        }
    }

    private Color frame_color_cal(int _side_num, int index, Color color)
    {
        Color step = (color - Color.white) / (float)_side_num;
        return Color.white + step * index;
    }

    /// <summary>
    /// Calculate the posision of each frame;
    /// </summary>
    /// <param name="side_num"></param>
    /// <param name="index"></param>
    /// <param name="size">{hori center to the right, vert center to the up}</param>
    /// <returns>pos{center to right, center to up}</returns>
    private Vector2 frame_pos_cal(int side_num, int index, Vector2 size)
    {
        Vector2 step = size / (float)(side_num - 1) ;
        return (float)index * step;
    }

    /// <summary>
    /// Calcualte the frame size based on index; last is size 0;
    /// </summary>
    /// <param name="side_num"></param>
    /// <param name="index"></param>
    /// <param name="size">Scale size;</param>
    /// <returns></returns>
    private float frame_size_cal(int side_num, int index, float size)
    {
        float step = size / (float)(side_num-1);
        return size - index * step;
    }

    private int frame_renderoreder_cal(int max, int index)
    {
        return max - index * Sorting_order_scale;
    }

    private void init_level_previews(FS_TrialGroup[] levels_infos, int frame_num)
    {
        int max_num = (frame_num / levels_infos.Length + 1) * levels_infos.Length;
        int level_index = 0;
        for (int i = 0;i < max_num; i++)
        {
            level_index = i % levels_infos.Length;
            Transform LP_TRANS = instantiate_LP(levels_infos[level_index].Texture, 
                FS_Setting.IS.PreviewMeshSize, LP_prefab, FS_RC.IS.PPU, level_index,
                FS_Setting.IS.MaxStar, levels_infos[level_index].Stars_to_unlock);
            level_preview_TRANSs[i] = LP_TRANS;
        }
    }

    private Transform instantiate_LP(Texture2D texture, float mesh_size, GameObject LP_prefab, int PPU, int LI,
        int star_max, int requ_star)
    {
        Transform LP_TRANS = Instantiate(LP_prefab, transform.position, Quaternion.identity, transform).
            transform;
        FS_LevelPreview LP_script = LP_TRANS.GetComponent<FS_LevelPreview>();
        LP_script.init_LP(texture, mesh_size, PPU, LI, star_max, requ_star);
        LP_script.deactivate();
        return LP_TRANS;
    }

    public void toggle_left_act()
    {
        if (!togglable) { return; }
        curr_index++;
        curr_index = GeneralMethods.index_mod(curr_index, level_preview_TRANSs.Count);
        int extra_index = GeneralMethods.index_mod(curr_index + (side_number - 1), level_preview_TRANSs.Count);
        FS_PreviewFrameInfo extra_PFI = PFIs[side_number-1];
        toggle_panel(extra_index, extra_PFI);
        toggle_ani();
    }

    public void toggle_right_act()
    {
        if (!togglable) { return; }
        curr_index--;
        curr_index = GeneralMethods.index_mod(curr_index, level_preview_TRANSs.Count);
        int extra_index = GeneralMethods.index_mod(curr_index - (side_number - 1), level_preview_TRANSs.Count);
        FS_PreviewFrameInfo extra_PFI = PFIs[-(side_number - 1)];
        toggle_panel(extra_index, extra_PFI);
        toggle_ani();
    }

    private void toggle_ani()
    {
        animator.SetTrigger(FS_SD.AniToggle_str);
    }

    /// <summary>
    /// Toggle panel after index changed;
    /// </summary>
    private void toggle_panel(int extra_index, FS_PreviewFrameInfo extra_PFI)
    {
        float time = FS_Setting.IS.PreviewTransTime;
        List<int> old_LPs = new List<int>();
        foreach(int LP_index in PFI_to_LP.Values)
        {
            old_LPs.Add(LP_index);
        }
        LP_cal(curr_index, FS_Setting.IS.PreviewSideNumber);
        FS_PreviewFrameInfo temp_PFI = null;
        FS_LevelPreview temp_LP = null;
        for(int i = 0;i<old_LPs.Count;i++)
        {
            int new_PFI = GeneralMethods.dict_find_int(PFI_to_LP, old_LPs[i]);
            if (new_PFI == Int32.MinValue) { continue; }
            temp_PFI = PFIs[new_PFI];
            temp_LP = level_preview_TRANSs[old_LPs[i]].GetComponent<FS_LevelPreview>();
            temp_LP.transfer_to(temp_PFI, time);
        }
        temp_LP = level_preview_TRANSs[extra_index].GetComponent<FS_LevelPreview>();
        temp_LP.set_to(extra_PFI);
    }

    public void ToInitPanel()
    {
        init_panel(FS_RC.IS.Level_infos.ToArray());
        deactivate_panel();
    }

    private void deactivate_panel()
    {
        foreach(Transform LP_TRANS in level_preview_TRANSs.Values)
        {
            LP_TRANS.GetComponent<FS_LevelPreview>().deactivate();
        }
    }

    public void ToInSelectPanel()
    {
        init_frames();
        animator.SetTrigger(FS_SD.AniNextStep_str);
    }

    public void ToIdle()
    {
        selectable = true;
        togglable = true;
    }

    public void LeaveIdle()
    {
        selectable = false;
        togglable = false;
    }

    public void ToInGame()
    {
        deactivate_panel();
    }

    public void reshow_panel()
    {
        animator.SetTrigger(FS_SD.AniShowPanel_str);
    }

    public void select_act()
    {
        if (!selectable) { return; }
        select();
    }

    private void select()
    {
        FS_GameController.IS.trial_selected(GeneralMethods.index_mod(curr_index, level_num));
    }

    public void enter_game()
    {
        animator.SetTrigger(FS_SD.AniSelect_str);
    }

    public void start_select_panel()
    {
        animator.SetTrigger(FS_SD.AniStart_str);
    }
}
