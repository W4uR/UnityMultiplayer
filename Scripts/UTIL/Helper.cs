using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;


public static class Helper
{

    private static Matrix4x4 isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
    private static Regex ip = new Regex(@"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$");
    public static Vector3 ToIso(this Vector3 input) => isoMatrix.MultiplyPoint3x4(input);
    public static Dictionary<ulong, GameObject> players;
    public static bool IsIP(this string input) => ip.IsMatch(input);


}
