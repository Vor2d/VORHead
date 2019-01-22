using System.Collections;
using System.Collections.Generic;

public class MD_WaveInfo
{
    public int WaveNumber
    {
        get { return wave_number; }
    }
    public List<int> WaveInfoList
    {
        get { return waveinfo_list; }
    }

    private int wave_number;
    private List<int> waveinfo_list;

    public MD_WaveInfo()
    {
        init();
    }

    public MD_WaveInfo(MD_WaveInfo other_MDWI)
    {
        init();
        set_data(other_MDWI.waveinfo_list);
    }

    private void init()
    {
        this.wave_number = 0;
        this.waveinfo_list = new List<int>();
    }

    public void set_data(List<int> other_WIL)
    {
        wave_number = other_WIL.Count;
        waveinfo_list = new List<int>();
        for (int i = 0; i < wave_number; i++)
        {
            waveinfo_list.Add(other_WIL[i]);
        }
    }




}
