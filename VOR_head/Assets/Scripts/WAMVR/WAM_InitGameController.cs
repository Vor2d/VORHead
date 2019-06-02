using UnityEngine;
using UnityEngine.SceneManagement;

public class WAM_InitGameController : MonoBehaviour
{
    [SerializeField] private GameObject DC_Prefab;

    // Start is called before the first frame update
    void Start()
    {
        if(GameObject.Find(WAMSD.DC_name) == null)
        {
            GameObject DC_OBJ = Instantiate(DC_Prefab, Vector3.zero, Quaternion.identity);
            DC_OBJ.name = WAMSD.DC_name;
        }

        SceneManager.LoadScene(WAMSD.GameScene_name);
    }


}
