using TMPro;
using UnityEngine;

public class GeneralTextController : MonoBehaviour
{
    [SerializeField] private Transform TM_TMP;
    [SerializeField] private Transform BG_TRANS;

    private TextMesh TM;
    private TextMeshPro TMP;
    private MeshRenderer TM_TMP_mesh;
    private MeshRenderer BG_mesh;
    private SpriteRenderer BG_SR;

    // Start is called before the first frame update
    void Start()
    {
        if(TM_TMP.GetComponent<TextMesh>() != null)
        {
            TM = TM_TMP.GetComponent<TextMesh>();
            TM_TMP_mesh = TM_TMP.GetComponent<MeshRenderer>();
        }
        if(TM_TMP.GetComponent<TextMeshPro>() != null)
        {
            TMP = TM_TMP.GetComponent<TextMeshPro>();
            TM_TMP_mesh = TM_TMP.GetComponent<MeshRenderer>();
        }
        if (BG_TRANS != null)
        {
            BG_mesh = BG_TRANS.GetComponent<MeshRenderer>();
            BG_SR = BG_TRANS.GetComponent<SpriteRenderer>();
        }
    }

    public void set_text(string text)
    {
        if (TM != null) { TM.text = text; }
        if (TMP != null) { TMP.text = text; }
    }

    public void turn_on(string text, bool turn_on_mesh = true)
    {
        set_text(text);
        if (turn_on_mesh) { turn_on(); }
    }

    public void turn_on()
    {
        if (TM_TMP_mesh != null) { TM_TMP_mesh.enabled = true; }
        if (BG_mesh != null) { BG_mesh.enabled = true; }
        if (BG_SR != null) { BG_SR.enabled = true; }
    }

    public void turn_off()
    {
        if (TM_TMP_mesh != null) { TM_TMP_mesh.enabled = false; }
        if (BG_mesh != null) { BG_mesh.enabled = false; }
        if (BG_SR != null) { BG_SR.enabled = false; }
    }
}
