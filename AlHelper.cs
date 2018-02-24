using System;
using OalSoft.NET.OpenALSharp;

namespace OalSoft.NET
{
    internal static class AlHelper
    {
        internal static void AlAlwaysCheckError(string message = "")
        {
            var error = AL10.alGetError();
            if (error != AL10.AL_NO_ERROR)
                throw new InvalidOperationException(message + $" ({error})");
        }

        [System.Diagnostics.Conditional("DEBUG")]
        internal static void AlCheckError(string message = "")
        {
            var error = AL10.alGetError();
            if (error != AL10.AL_NO_ERROR)
                throw new InvalidOperationException(message + $" ({error})");
        }

        internal static void AlcAlwaysCheckError(IntPtr device, string message = "")
        {
            var error = ALC10.alcGetError(device);
            if (error != ALC10.ALC_NO_ERROR)
                throw new InvalidOperationException(message + $" ({error})");
        }

        [System.Diagnostics.Conditional("DEBUG")]
        internal static void AlcCheckError(IntPtr device, string message = "")
        {
            var error = ALC10.alcGetError(device);
            if (error != ALC10.ALC_NO_ERROR)
                throw new InvalidOperationException(message + $" ({error})");
        }
    }
}
