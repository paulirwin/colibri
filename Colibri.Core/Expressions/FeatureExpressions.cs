using System.Runtime.InteropServices;

namespace Colibri.Core.Expressions;

public class FeatureExpressions
{
    public static readonly Lazy<IReadOnlyList<Symbol>> AllFeatures = new(InitializeFeatures, LazyThreadSafetyMode.PublicationOnly);
    
    public static object Features(object?[] args)
    {
        if (args.Length > 0)
        {
            throw new ArgumentException("features does not take any arguments");
        }

        return List.FromNodes(AllFeatures.Value);
    }

    private static IReadOnlyList<Symbol> InitializeFeatures()
    {
        // Reference R7RS-small Appendix B
        
        var featureSet = new List<Symbol>
        {
            "colibri",      // Indicates that this is using Colibri
            "r7rs",         // All R7RS Scheme implementations have this feature.
            // TODO: need to unit test for this: "exact-closed", // All algebraic operations except / produce exact values given exact inputs.
            // TODO: exact-complex support
            "ieee-float",   // Inexact numbers are IEEE 754 binary floating point values.
            "full-unicode", // All Unicode characters present in Unicode version 6.0 are supported as Scheme characters.
            "ratios",       // / with exact arguments produces an exact result when the divisor is nonzero.
            // TODO: detect posix compliance
            "clr",          // for .NET Common Language Runtime
        };

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            featureSet.Add("windows");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            featureSet.Add("darwin");
            featureSet.Add("bsd");
            featureSet.Add("macosx");
            featureSet.Add("macos");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
        {
            featureSet.Add("bsd");
            featureSet.Add("freebsd");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            featureSet.Add("linux");
            featureSet.Add("gnu-linux"); // HACK: Assume GNU for now
        }
        else if (Environment.OSVersion.Platform == PlatformID.Unix)
        {
            featureSet.Add("unix");
        }

        featureSet.Add(RuntimeInformation.ProcessArchitecture switch
        {
            Architecture.X86 => "i386",
            Architecture.X64 => "x86-64",
            Architecture.Arm => "arm32v7", // HACK: AFAIK, this is the only 32-bit ARM platform supported by .NET
            Architecture.Arm64 => "aarch64",
            Architecture.Wasm => "wasm",
            Architecture.S390x => "s390x",
            _ => throw new PlatformNotSupportedException($"Unknown process architecture {RuntimeInformation.ProcessArchitecture}")
        });

        featureSet.Add(RuntimeInformation.ProcessArchitecture switch
        {
            Architecture.S390x => "big-endian",
            Architecture.X86 => "little-endian",
            Architecture.X64 => "little-endian",
            Architecture.Arm => "little-endian",
            Architecture.Arm64 => "little-endian",
            Architecture.Wasm => "little-endian",
            _ => throw new PlatformNotSupportedException($"Unknown process architecture {RuntimeInformation.ProcessArchitecture}")
        });

        return featureSet;
    }
}