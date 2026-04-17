using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// A.K.A a class containing a static property list of colors that doesn't exist within Unity (like seriously, why *don't* they exist?)
public class ColorsExt
{
    /// <summary>
    /// Solid Orange (Or atleast it should be, according to Microsoft.)
    /// </summary>
    public static Color orange
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // Not sure what this does but I'm assuming it's important to have it in judging from the decompilation of Unity's Color class
        get
        {
            return new Color32(System.Drawing.Color.Orange.R, System.Drawing.Color.Orange.G, System.Drawing.Color.Orange.B, 255);
        }
    }
}
