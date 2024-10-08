using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace OsuHook.OpcodeUtil
{
    internal static class OpCodeMatcher
    {
        /// <summary>
        ///     Search for a method inside the osu! assembly by an IL OpCode signature.
        /// </summary>
        /// <param name="signature">A set of sequential OpCodes to match.</param>
        /// <returns>The found method or null if none found.</returns>
        public static MethodInfo FindMethodBySignature(IReadOnlyList<OpCode> signature)
        {
            if (signature.Count <= 0) return null;

            foreach (var type in OsuModule.GetTypes())
            foreach (var method in type.GetMethods(BindingFlags.Instance
                                                   | BindingFlags.Static
                                                   | BindingFlags.Public
                                                   | BindingFlags.NonPublic))
            {
                var instructions = method.GetMethodBody()?.GetILAsByteArray();
                if (instructions == null) continue;

                if (InstructionsMatchesSignature(instructions, signature))
                    return method;
            }

            return null;
        }

        /// <summary>
        ///     Search for a constructor inside the osu! assembly by an IL OpCode signature.
        /// </summary>
        /// <param name="signature">A set of sequential OpCodes to match.</param>
        /// <returns>The found constructor (method) or null if none found.</returns>
        public static ConstructorInfo FindConstructorBySignature(IReadOnlyList<OpCode> signature)
        {
            if (signature.Count <= 0) return null;

            foreach (var type in OsuModule.GetTypes())
            foreach (var method in type.GetConstructors(BindingFlags.Instance
                                                        | BindingFlags.Static
                                                        | BindingFlags.Public
                                                        | BindingFlags.NonPublic))
            {
                var instructions = method.GetMethodBody()?.GetILAsByteArray();
                if (instructions == null) continue;

                if (InstructionsMatchesSignature(instructions, signature))
                    return method;
            }

            return null;
        }

        /// <summary>
        ///     Check if some IL byte instructions contain a certain set of OpCodes.
        /// </summary>
        /// <param name="ilInstructions">
        ///     Raw IL instruction byte data obtained through <see cref="MethodBody.GetILAsByteArray()" />
        ///     for example.
        /// </param>
        /// <param name="signature">A set of sequential OpCodes to search for in instructions.</param>
        /// <returns></returns>
        private static bool InstructionsMatchesSignature(
            IReadOnlyList<byte> ilInstructions,
            IReadOnlyList<OpCode> signature)
        {
            var sequentialMatching = 0;
            foreach (var instruction in new OpCodeReader(ilInstructions).GetOpCodes())
            {
                if (instruction == signature[sequentialMatching])
                    sequentialMatching++;
                else
                    sequentialMatching = 0;

                if (sequentialMatching == signature.Count)
                    return true;
            }

            return false;
        }

        #region osu! Assembly

        private static readonly Module OsuModule;

        /// <summary>
        ///     Cache the already-loaded osu!.exe .NET assembly to search in.
        /// </summary>
        static OpCodeMatcher()
        {
            var osuAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .SingleOrDefault(assembly => assembly.GetName().Name == "osu!");

            OsuModule = osuAssembly?.GetModules().SingleOrDefault()
                        ?? throw new Exception("Unable to find a loaded osu! assembly!");
        }

        #endregion
    }
}