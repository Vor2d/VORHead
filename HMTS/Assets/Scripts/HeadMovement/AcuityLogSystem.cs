using UnityEngine;
using System.Text;
using System.IO;
using System.Threading;

public class AcuityLogSystem : GeneralLogSystem
{
    private const string _first_line =
                "Line_#\tSimulink_#\tTime\tAcuitySize\tRight_Wrong";

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        this.first_line = _first_line;
    }

    protected override void logging()
    {
        while (thread_state_flag) { Thread.Sleep(100); }

        write_file();
    }

    public override void write_file()
    {
        base.write_file();

        Debug.Log("Starting write file: " + file_name);
        StreamWriter file;
        try
        {
            // create log file if it does not already exist. Otherwise open it for appending new trial
            if (!File.Exists(file_name))
            {
                file = new StreamWriter(file_name);
                file.WriteLine(head_line);
                file.WriteLine(first_line);
            }
            else
            {
                file = File.AppendText(file_name);
            }


            file.WriteLine(Log_SB);
            file.Close();
            Log_SB = new StringBuilder();
            Debug.Log("Writing finished: " + file_name);
        }
        catch (System.Exception e)
        {
            Debug.Log("Error in accessing file: " + e);
        }
    }

    public void log_time(string start_end, uint simulink_sample,string game_mode)
    {
        total_time = stop_watch.Elapsed.TotalMilliseconds;

        Log_SB.Append(line_counter.ToString());
        Log_SB.Append("\t");
        Log_SB.Append(start_end);
        Log_SB.Append("\t");
        Log_SB.Append(simulink_sample.ToString());
        Log_SB.Append("\t");
        Log_SB.Append(total_time.ToString("F2"));
        Log_SB.Append("\t");
        Log_SB.Append(game_mode);
        Log_SB.AppendLine();

        line_counter++;
    }

    public void log_acuity(uint simulink_sample, int acuity_size, string right_wrong)
    {
        total_time = stop_watch.Elapsed.TotalMilliseconds;

        Log_SB.Append(line_counter.ToString());
        Log_SB.Append("\t");
        Log_SB.Append(simulink_sample.ToString());
        Log_SB.Append("\t");
        Log_SB.Append(total_time.ToString("F0"));
        Log_SB.Append("\t");
        Log_SB.Append(acuity_size.ToString());
        Log_SB.Append("\t");
        Log_SB.Append(right_wrong);
        Log_SB.AppendLine();

        line_counter++;
    }

    public void log_acuity_state(uint simulink_sample,string action)
    {
        log_acuity(simulink_sample, -1, action);
    }

    public void log_acuity_delay(uint simulink_sample, float delay)
    {
        log_acuity(simulink_sample, -2, delay.ToString("F2"));
    }

}
