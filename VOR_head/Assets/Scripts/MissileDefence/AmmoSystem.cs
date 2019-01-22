using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoSystem : MonoBehaviour
{
    public int Current_ammo { get { return current_ammo; } }

    private int current_ammo;

    // Start is called before the first frame update
    void Start()
    {
        this.current_ammo = 0;
    }

    //true if ammo is enough, false if not;
    public bool ammo_spend()
    {
        if(current_ammo > 0)
        {
            current_ammo--;
            return true;
        }
        else
        {
            return false;
        }
        
    }

    public void set_ammo(int ammo_number)
    {
        current_ammo = ammo_number;
    }

}
