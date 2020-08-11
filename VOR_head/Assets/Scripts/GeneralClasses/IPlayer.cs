
using System;

[Obsolete("Not a good system")]
public interface IPlayer
{
    void set_score(int score);
    float get_score();
    void increase_score(float increase);
    void unlock_level(int level);
    int[] get_unlocked_level();
    void remove_unlocked_level(int level);
    void clear_unlocked_level();
}
