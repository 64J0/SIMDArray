namespace System

open System.Reflection

[<assembly: AssemblyTitleAttribute("SIMDArray")>]
[<assembly: AssemblyProductAttribute("SIMDArray")>]
[<assembly: AssemblyDescriptionAttribute("SIMD enhanced Array operations for F#")>]
[<assembly: AssemblyVersionAttribute("2.0.0")>]
[<assembly: AssemblyFileVersionAttribute("2.0.0")>]
do ()

module internal AssemblyVersionInformation =
    [<Literal>]
    let AssemblyTitle = "SIMDArray"

    [<Literal>]
    let AssemblyProduct = "SIMDArray"

    [<Literal>]
    let AssemblyDescription = "SIMD enhanced Array operations for F#"

    [<Literal>]
    let AssemblyVersion = "2.0.0"

    [<Literal>]
    let AssemblyFileVersion = "2.0.0"
