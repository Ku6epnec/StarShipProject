using UnityEngine;
using UnityEngine.Rendering;

public class CameraRenderer : MonoBehaviour
{
    private ScriptableRenderContext _context;
    private Camera _camera;
    public void Render(ScriptableRenderContext context, Camera camera)
    {
        _camera = camera;
        _context = context;
    }
}
