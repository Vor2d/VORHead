using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class ET_FileReader : MonoBehaviour {

    private readonly char[] file_spliter = new char[] { ' ' };
    private readonly string[] file_spliter_str = new string[] {"   "};
    private const string path = "dataMat";
    private const float angle = 20.0f;

    public float Left_k { get; set; }
    public float Left_b { get; set; }
    public float Right_k { get; set; }
    public float Right_b { get; set; }
    public List<float[]> data { get; set; }
    

	// Use this for initialization
	void Start () {
        this.data = new List<float[]>();
        this.Left_k = 0.0f;
        this.Left_b = 0.0f;
        this.Right_k = 0.0f;
        this.Right_b = 0.0f;

        int line_counter = 0;
        StreamReader reader = new StreamReader(path);
        while (!reader.EndOfStream)
        {
            line_counter++;
            string[] split_str = reader.ReadLine().Split(file_spliter_str,
                                                    StringSplitOptions.RemoveEmptyEntries);
            float[] data_arr = new float[7];
            for(int i = 0;i<7;i++)
            {
                data_arr[i] = 
                    float.Parse(split_str[i], System.Globalization.NumberStyles.Float);
            }
            data.Add(data_arr);
        }

        float center_med;
        float left_med;
        float right_med;

        calibrate_data(out center_med, out left_med, out right_med, 1);
        fit_linear_left(left_med, right_med, -20.0f, 20.0f);

        calibrate_data(out center_med, out left_med, out right_med, 4);
        fit_linear_right(left_med, right_med, -20.0f, 20.0f);
        //fit_linear_right(left_med, right_med, -20.0f, 20.0f);
        //fit_linear_right(center_med, right_med, 0.0f, 20.0f);

    }

    // Update is called once per frame
    void Update () {
		
	}

    private void calibrate_data(out float center_med, 
                                out float left_med, 
                                out float right_med,
                                int column)
    {
        List<float> center_list = new List<float>();
        List<float> right_list = new List<float>();
        List<float> left_list = new List<float>();

        center_med = 0.0f;
        left_med = 0.0f;
        right_med = 0.0f;

        for (int i = 0; i<data.Count;i++)
        {
            if(data[i][column] > 230f && data[i][column] < 270f)
            {
                center_list.Add(data[i][column]);
            }
            else if (data[i][column] > 290f && data[i][column] < 350f)
            {
                right_list.Add(data[i][column]);
            }
            else if (data[i][column] > 170f && data[i][column] < 230f)
            {
                left_list.Add(data[i][column]);
            }
        }
        center_med = get_median(center_list);
        left_med = get_median(left_list);
        right_med = get_median(right_list);

    }

    private float get_median(List<float> data_list)
    {
        data_list.Sort();
        
        if(data_list.Count % 2 != 0)
        {
            return data_list[data_list.Count / 2];
        }
        else
        {
            return (data_list[data_list.Count / 2] + data_list[data_list.Count / 2 - 1])
                            / 2.0f;
        }
    }

    private void fit_linear_left(float x1, float x2, float y1, float y2)
    {
        float delta_y = y1 - y2;
        float delta_x = x1 - x2;
        Left_k = delta_y / delta_x;
        Left_b = y1 - (Left_k * x1);
    }

    private void fit_linear_right(float x1, float x2, float y1, float y2)
    {
        float delta_y = y1 - y2;
        float delta_x = x1 - x2;
        Right_k = delta_y / delta_x;
        Right_b = y1 - (Right_k * x1);
    }

    public float excute_linear_left(float x)
    {
        return Left_k * x + Left_b; 
    }

    public float excute_linear_right(float x)
    {
        return Right_k * x + Right_b;
    }
}
