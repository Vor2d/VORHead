using UnityEngine;

public class GeneralChangSorting : MonoBehaviour
{
    [SerializeField] bool ChangeSorting;
    [SerializeField] private string LayerName;
    [SerializeField] private int OrderNum;

    // Start is called before the first frame update
    void Start()
    {
        if(ChangeSorting)
        {
            GetComponent<MeshRenderer>().sortingLayerName = LayerName;
            GetComponent<MeshRenderer>().sortingOrder = OrderNum;
        }
    }
}
