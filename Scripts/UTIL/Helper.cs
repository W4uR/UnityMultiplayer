using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System;


public static class Helper
{

    private static Matrix4x4 isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
    private static Regex ip = new Regex(@"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$");
    public static Vector3 ToIso(this Vector3 input) => isoMatrix.MultiplyPoint3x4(input);

    public static bool IsIP(this string input) => ip.IsMatch(input);

    public static Color TransformH(Color col, float H)
    {
        float U = (float)Math.Cos(H * Math.PI / 180);
        float W = (float)Math.Sin(H * Math.PI / 180);

        return new Color(
            (.299f + .701f * U + .168f * W) * col.r
            + (.587f - .587f * U + .330f * W) * col.g
            + (.114f - .114f * U - .497f * W) * col.b,
            (.299f - .299f * U - .328f * W) * col.r
            + (.587f + .413f * U + .035f * W) * col.g
            + (.114f - .114f * U + .292f * W) * col.b,
            (.299f - .3f * U + 1.25f * W) * col.r
            + (.587f - .588f * U - 1.05f * W) * col.g
            + (.114f + .886f * U - .203f * W) * col.b
            );
    }

}
