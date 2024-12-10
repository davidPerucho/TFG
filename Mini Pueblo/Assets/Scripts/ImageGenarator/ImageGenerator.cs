using UnityEngine;
using UnityEngine.Networking;
using Python.Runtime;
using UnityEditor.Scripting.Python;
using System.IO;

public class ImageGenerator : MonoBehaviour
{
    /**
    [SerializeField]
    string apiKey = "TU_CLAVE_API";
    string apiEndpoint = "";**/

    [SerializeField]
    string theme;

    void Start()
    {
        GenerateImage();
    }

    public void GenerateImage()
    {
        /**PythonRunner.EnsureInitialized();
        using (Py.GIL())
        {
            var pythonGeneratorScript = Py.Import("StableDifusion");
            PyString prompt = new PyString(theme);
            pythonGeneratorScript.InvokeMethod("generateImage", new PyObject[] {prompt});
        }**/

        string dir = __DIR__();
        PythonRunner.EnsureInitialized();

        using (Py.GIL())
        {
            PyObject sys = Py.Import("sys");
            PyObject path = sys.GetAttr("path");
            if (!path.InvokeMethod("count", new PyObject[] { new PyString(dir) }).As<int>().Equals(1))
            {
                path.InvokeMethod("append", new PyObject[] { new PyString(dir) });
            }
        }

        PythonRunner.RunString($@"
            import StableDifusion
            StableDifusion.generateImage(""{theme}"")
        ");
    }

    /// <summary>
    /// Hack to get the current file's directory
    /// </summary>
    /// <param name="fileName">Leave it blank to the current file's directory</param>
    /// <returns></returns>
    private static string __DIR__([System.Runtime.CompilerServices.CallerFilePath] string fileName = "")
    {
        return Path.GetDirectoryName(fileName);
    }

}
