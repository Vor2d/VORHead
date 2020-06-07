using UnityEngine;

public class SS_GameController : MonoBehaviour
{

    public static SS_GameController IS { get; private set; }

    private void Awake()
    {
        IS = this;

        if (DataController.IS == null) {}
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
