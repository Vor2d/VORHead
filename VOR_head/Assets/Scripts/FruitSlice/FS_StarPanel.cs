using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FS_StarPanel : MonoBehaviour
{
    [SerializeField] private float Width;
    [SerializeField] private bool Local_width;
    [SerializeField] private bool Using_unfilled;

    private int max_num;
    private int curr_num;
    private float trans_time;
    private Dictionary<int, Vector3> frames_poss;    //Position for all stars; {index, pos}
    private List<Transform> star_pool;  //Spawned stars;
    private List<Transform> star_unfilled_pool; //Spawned unfilled stars;

    private void Awake()
    {
        this.max_num = 0;
        this.curr_num = 0;
        this.trans_time = 0.0f;
        this.frames_poss = new Dictionary<int, Vector3>();
        this.star_pool = new List<Transform>();
        this.star_unfilled_pool = new List<Transform>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void init_SP(int _max_num, float _trans_time = 0.0f)
    {
        max_num = _max_num;
        trans_time = _trans_time;
        frame_cal(_max_num);
    }

    private void frame_cal(int max_num)
    {
        Vector2[] poss = GeneralMethods.grid_generation(Width, 0.0f, 1, max_num);
        for(int i = 0;i < poss.Length;i++)
        {
            frames_poss[i] = (Vector3)poss[i];
        }
    }

    /// <summary>
    /// Spawn the star instantly;
    /// </summary>
    public void spawn_star_inst(int curr_star)
    {
        curr_num = curr_star;
        Transform temp_TRANS = null;
        for (int i = 0;i<curr_star;i++)
        {
            temp_TRANS = instantiate_star(frames_poss[i], FS_RC.IS.Star_Prefab);
            star_pool.Add(temp_TRANS);
        }
        if(Using_unfilled)
        {
            for(int i = curr_star;i<max_num;i++)
            {
                temp_TRANS = instantiate_star(frames_poss[i], FS_RC.IS.Star_unfilled_Prefab);
                star_unfilled_pool.Add(temp_TRANS);
            }
        }
    }

    private Transform instantiate_star(Vector3 pos, GameObject prefab)
    {
        Transform star_TRANS = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform).transform;
        star_TRANS.localPosition = pos;
        return star_TRANS;
    }

    public void clear_panel()
    {
        foreach(Transform star_TRANS in star_pool.ToArray())
        {
            Destroy(star_TRANS.gameObject);
        }
        star_pool.Clear();
        foreach (Transform star_un_TRANS in star_unfilled_pool.ToArray())
        {
            Destroy(star_un_TRANS.gameObject);
        }
        star_unfilled_pool.Clear();
    }

    public void spawn_star_time(int curr_star, float time = -1.0f)
    {
        if (time < 0) { time = trans_time; }
        StartCoroutine(spawn_star_coro(curr_star, max_num, time, FS_RC.IS.Star_Prefab,
            FS_RC.IS.Star_unfilled_Prefab, frames_poss));
    }

    private IEnumerator spawn_star_coro(int curr_star, int max_star, float time, GameObject star_prefab,
        GameObject star_un_prefab, Dictionary<int, Vector3> frames_poss)
    {
        float time_period = time / (float)max_star;
        float timer = 0.0f;
        int curr_index = 1;
        Transform temp_TRANS = null;
        while (timer < time)
        {
            timer += Time.deltaTime;
            if (timer >= time_period * curr_index)
            {
                if (curr_index - 1 < curr_star)
                {
                    temp_TRANS = instantiate_star(frames_poss[curr_index - 1], star_prefab);
                    star_pool.Add(temp_TRANS);
                }
                else 
                {
                    temp_TRANS = instantiate_star(frames_poss[curr_index - 1], star_un_prefab);
                    star_unfilled_pool.Add(temp_TRANS);
                }
                curr_index++;
            }
            yield return null;
        }
        SS_coro_CB();
    }

    private void SS_coro_CB()
    {

    }

    public void set_SO(int SO)
    {
        foreach(Transform star_TRANS in star_pool)
        {
            star_TRANS.GetComponent<SpriteRenderer>().sortingOrder = SO;
        }
        foreach(Transform star_un_TRANS in star_unfilled_pool)
        {
            star_un_TRANS.GetComponent<SpriteRenderer>().sortingOrder = SO;
        }
    }

    public void set_color(Color color)
    {
        foreach (Transform star_TRANS in star_pool)
        {
            star_TRANS.GetComponent<SpriteRenderer>().color = color;
        }
        foreach (Transform star_un_TRANS in star_unfilled_pool)
        {
            star_un_TRANS.GetComponent<SpriteRenderer>().color = color;
        }
    }
}
