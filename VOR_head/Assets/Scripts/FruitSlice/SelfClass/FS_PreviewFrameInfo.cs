using UnityEngine;

public class FS_PreviewFrameInfo
{
    public float Scale_size { get; private set; }
    public Vector2 Posisiton { get; private set; }
    public int Render_order { get; private set; }
    public Color Global_color { get; private set; }

    public FS_PreviewFrameInfo()
    {
        this.Scale_size = 0.0f;
        this.Posisiton = new Vector2();
        this.Render_order = 0;
    }

    public FS_PreviewFrameInfo(float _scale_size, Vector2 _position, int _render_oreder)
    {
        this.Scale_size = _scale_size;
        this.Posisiton = _position;
        this.Render_order = _render_oreder;
        this.Global_color = Color.white;
    }

    public FS_PreviewFrameInfo(float _scale_size, Vector2 _position, int _render_oreder, Color _color)
    {
        this.Scale_size = _scale_size;
        this.Posisiton = _position;
        this.Render_order = _render_oreder;
        this.Global_color = _color;
    }

    public void set_SS(float _scale_size)
    {
        Scale_size = _scale_size;
    }

    public void set_pos(Vector2 pos)
    {
        Posisiton = pos;
    }

    public void set_RO(int RO)
    {
        Render_order = RO;
    }
}
