using System.Collections.Generic;

public class FS_Player
{
    public int Total_score { get { return total_score; } }
    public int Total_star { get { return total_star; } }

    private int total_score;
    private int total_star;
    private Dictionary<int, FS_PlayerLevelInfo> level_infos;    //Level index, level info;

    public static FS_Player IS;

    public FS_Player()
    {
        IS = this;

        this.total_score = 0;
        this.total_star = 0;
        this.level_infos = new Dictionary<int, FS_PlayerLevelInfo>();
    }

    public void init_levels(int level_num)
    {
        for(int i = 0;i<level_num;i++)
        {
            force_new_empty_level(i);
        }
    }

    public void add_level_info(int level, int score, int stars)
    {
        check_level_overwrite(level);
        int SC = set_level_score(level, score);
        int SAC =set_level_stars(level, stars);
        update_total(SC, SAC);
    }

    /// <summary>
    /// Keep the highest scores;
    /// </summary>
    public void update_level_info(int level, int score, int stars)
    {
        check_level_overwrite(level);
        int SC = set_level_score(level, score, update: true);
        int SAC = set_level_stars(level, stars, update: true);
        update_total(SC, SAC);
    }

    public void update_total()
    {
        update_total_score();
        update_total_star();
    }

    public void update_total(int score_change, int star_change)
    {
        total_score += score_change;
        total_star += star_change;
    }

    private void check_level_overwrite(int level)
    {
        if (level_infos[level] == null) { force_new_empty_level(level); }
    }

    private void force_new_empty_level(int level)
    {
        level_infos[level] = new FS_PlayerLevelInfo();
    }

    public int get_level_score(int level)
    {
        if (level_infos[level] == null) { return 0; }
        return level_infos[level].Highest_score;
    }

    private int set_level_score(int level, int score, bool update = false)
    {
        if (level_infos[level] == null) { force_new_empty_level(level); }
        return level_infos[level].set_highest_score(score, update);
    }

    public int get_level_stars(int level)
    {
        if (level_infos[level] == null) { return 0; }
        return level_infos[level].Stars;
    }

    private int set_level_stars(int level, int stars, bool update = false)
    {
        if (level_infos[level] == null) { force_new_empty_level(level); }
        return level_infos[level].set_stars(stars, update);
    }

    public void unlock_level(int level)
    {
        if (level_infos[level] == null) { force_new_empty_level(level); }
        level_infos[level].unlock_level();
    }

    public bool get_level_lock_stat(int level)
    {
        if (level_infos[level] == null) { return false; }
        return level_infos[level].Unlocked;
    }

    private void update_total_score()
    {
        int score = 0;
        foreach(FS_PlayerLevelInfo PLI in level_infos.Values)
        {
            score += PLI.Highest_score;
        }
        total_score = score;
    }

    private void update_total_star()
    {
        int star = 0;
        foreach (FS_PlayerLevelInfo PLI in level_infos.Values)
        {
            star += PLI.Stars;
        }
        total_star = star;
    }

    /// <summary>
    /// Refresh and return score and star;
    /// </summary>
    /// <returns></returns>
    public (int, int) get_refreshed_SC_STA()
    {
        update_total();
        return (total_score, total_star);
    }

    public FS_PlayerLevelInfo get_level_info(int index)
    {
        return level_infos[index];
    }
}
