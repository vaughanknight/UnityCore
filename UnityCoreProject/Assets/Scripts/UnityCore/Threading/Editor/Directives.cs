/// <summary>
/// We use this to check if we're in the editor as it only gets compiled. If 
/// the type exits, we're in the editor.  We don't use Application.isEditor and
/// Application.isPlaying because these can only be called on the main thread
/// which in turn means any multithreading won't be able to call them
/// </summary>

namespace UnityCore.Threading.Editor
{
    public class UNITY_CORE_EDITOR
    { }
}
