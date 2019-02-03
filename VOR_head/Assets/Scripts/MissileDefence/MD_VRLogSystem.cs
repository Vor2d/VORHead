using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

public class MD_VRLogSystem : VRLogSystem
{
    [SerializeField] private MD_GameController MDGC_script;

    protected override void load_data_controller()
    {
        
    }

    public override void update_headline()
    {
        head_line = MDGC_script.var_to_string();
    }
}
