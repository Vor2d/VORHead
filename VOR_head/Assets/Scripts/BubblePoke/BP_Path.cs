using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BP_Path : MonoBehaviour
{
    public int StationNumber = 5;
    public List<Transform> Stations_TRANSs;
    [SerializeField] private GameObject Station_Prefab;
    [SerializeField] private float InitZ = 10.0f;

    public BP_RC BPRC { get; private set; }
    private BP_DrawPath DP_script;
    private Color theme_color;
    private Vector3 start_pos;

    private void Awake()
    {
        this.theme_color = Color.white;
        this.DP_script = GetComponent<BP_DrawPath>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if(BPRC == null)
        {
            this.BPRC = GameObject.Find(BP_StrDefiner.RC_name).GetComponent<BP_RC>();
        }

        if (BPRC.GC_script.DrawPathFlag)
        {
            DP_script.draw_path(theme_color);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void init_path(BP_RC _BPRC)
    {
        BPRC = _BPRC;
        generate_stations();
    }

    public void init_path(BP_RC _BPRC,Color _theme_color)
    {
        BPRC = _BPRC;
        set_theme_color(_theme_color);
        generate_stations();
    }

    public void init_path(BP_RC _BPRC, Color _theme_color,Vector3 _start_pos)
    {
        BPRC = _BPRC;
        set_theme_color(_theme_color);
        start_pos = _start_pos;
        generate_stations();
    }


    private void generate_stations()
    {
        if(BPRC.GC_script.RandomTrailFlag)
        {
            Transform station_TRANS =
                Instantiate(Station_Prefab, start_pos, Quaternion.identity).transform;
            station_TRANS.SetParent(transform);
            Stations_TRANSs.Add(station_TRANS);

            Vector3 position;
            for (int i = 1; i < StationNumber;i++)
            {
                position = new Vector3(
                            Random.Range(-BPRC.GC_script.RandRangeX, BPRC.GC_script.RandRangeX),
                            Random.Range(-BPRC.GC_script.RandRangeY, BPRC.GC_script.RandRangeY),
                            InitZ);
                station_TRANS = 
                        Instantiate(Station_Prefab, position, Quaternion.identity).transform;
                station_TRANS.SetParent(transform);
                Stations_TRANSs.Add(station_TRANS);
            }
        }
    }

    public void destroy_path()
    {
        Destroy(gameObject);
    }

    public void set_theme_color(Color _theme_color)
    {
        theme_color = _theme_color;
    }
}
