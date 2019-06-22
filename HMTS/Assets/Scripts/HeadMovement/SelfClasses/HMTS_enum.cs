
namespace HMTS_enum
{
    public enum GazeTarget { DefaultTarget, HideDetector };
    public enum GameModeEnum{ Default, GazeTest, EyeTest, Feedback_Learning, HC_FB_Learning,
        Jump_Learning, Training, NoReddot,StaticAcuity,DynamicAcuity,PostDynamicAcuity};
    public enum AcuityChangeMode { percent,acuity_list};
    public enum PostDelayModes { random,delay_list,percent};
}
