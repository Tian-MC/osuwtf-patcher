using OsuHook.OpcodeUtil;
using static System.Reflection.Emit.OpCodes;

// ReSharper disable InconsistentNaming

namespace OsuHook.Osu.Stubs
{
    /// <summary>
    ///     Original: <c>osu.GameModes.Play.Player</c>
    ///     b20240102.2: <c>#=znsd474wIu4GAJ8swgEdrqaxwLN4O</c>
    /// </summary>
    internal static class Player
    {
        /// <summary>
        ///     Original: <c>AllowDoubleSkip</c> (property getter)
        ///     b20240102.2: <c>#=zq3ahHKSwJLrHTBaQyw==</c>
        /// </summary>
        /// <returns></returns>
        public static readonly LazySignature AllowDoubleSkip_get = new LazySignature(
            "Player#AllowDoubleSkip.get",
            new[]
            {
                Neg,
                Stloc_0,
                Ldarg_0,
                Isinst,
                Brtrue_S,
                Ldsfld,
                Ldloc_0,
                Call,
                Brtrue_S,
                Ldc_I4_0,
                Br_S,
            }
        );
    }
}