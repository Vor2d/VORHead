
using System;

public class FS_PlayerLevelInfo
{
    public bool Unlocked { get; private set; }
    public int Highest_score { get; private set; }
    public int Stars { get; private set; }

    public FS_PlayerLevelInfo()
    {
        this.Unlocked = false;
        this.Highest_score = 0;
        this.Stars = 0;
    }

    public void unlock_level()
    {
        Unlocked = true;
    }

    public void lock_level()
    {
        Unlocked = false;
    }

    public int set_highest_score(int score, bool update = false)
    {
        int score_temp = Highest_score;
        if (update) { Highest_score = Math.Max(Highest_score, score); }
        else { Highest_score = score; }
        return Highest_score - score_temp;
    }

    public int set_stars(int stars, bool update = false)
    {
        int star_temp = Stars;
        if (update) { Stars = Math.Max(Stars, stars); }
        else { Stars = stars; }
        return Stars - star_temp;
    }
}
