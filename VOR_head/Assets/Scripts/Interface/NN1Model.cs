using UnityEngine;

public class NN1Model : GeneralModel
{
    private AForge.Neuro.ActivationNetwork NNetwork;
    private int H_V_index;  //0 for horizontal, 1 for vertical;

    public NN1Model(int index) : base()
    {
        this.H_V_index = index;
    }

    public NN1Model(NN1Model other_NN1M)
    {
        this.H_V_index = other_NN1M.H_V_index;
        this.NNetwork = other_NN1M.NNetwork;
    }

    public override void fit_model(AForge.Neuro.ActivationNetwork _NNetwork)
    {
        NNetwork = _NNetwork;
    }

    public override float get_value(Vector2 x1_x2)
    {
        double[] input = new double[2] { x1_x2.x, x1_x2.y };
        return (float)NNetwork.Compute(input)[H_V_index];
    }

    public override string var_to_string()
    {
        string result = "";

        result += " 2 -> 2";

        return result;
    }


}
