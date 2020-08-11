using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FS_LockPanel : MonoBehaviour
{
    [SerializeField] private TextMesh TM;
    [SerializeField] private SpriteRenderer Lock_mesh;
    [SerializeField] private SpriteRenderer Star_mesh;
    [SerializeField] private int Star_TM_SO_offset;
    [SerializeField] private string TM_post_str;

    private int requ_star;

    private void Awake()
    {
        this.requ_star = 0;
        Star_mesh.sprite = FS_RC.IS.Star_Prefab.GetComponent<SpriteRenderer>().sprite;
    }

    public void init_LP(int _requ_star)
    {
        requ_star = _requ_star;
        TM.text = requ_star.ToString() + TM_post_str;
    }

    public void set_SO(int SO)
    {
        GeneralMethods.change_render_order(TM.transform, render_layer: FS_SD.SortingLayer_GO,
            sorting_order: SO + Star_TM_SO_offset);
        Lock_mesh.sortingOrder = SO;
        Star_mesh.sortingOrder = SO + Star_TM_SO_offset;
    }

    public void set_color(Color color)
    {
        TM.color = color;
        Lock_mesh.color = color;
        Star_mesh.color = color;
    }

    public void turn_on_mesh()
    {
        TM.GetComponent<MeshRenderer>().enabled = true;
        Lock_mesh.enabled = true;
        Star_mesh.enabled = true;
    }

    public void turn_off_mesh()
    {
        TM.GetComponent<MeshRenderer>().enabled = false;
        Lock_mesh.enabled = false;
        Star_mesh.enabled = false;
    }

}
