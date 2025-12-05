namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("SIMDArray")>]
[<assembly: AssemblyProductAttribute("SIMDArray")>]
[<assembly: AssemblyDescriptionAttribute("SIMD enhanced Array operations for F#")>]
[<assembly: AssemblyVersionAttribute("2.0.0")>]
[<assembly: AssemblyFileVersionAttribute("2.0.0")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] AssemblyTitle = "SIMDArray"
    let [<Literal>] AssemblyProduct = "SIMDArray"
    let [<Literal>] AssemblyDescription = "SIMD enhanced Array operations for F#"
    let [<Literal>] AssemblyVersion = "2.0.0"
    let [<Literal>] AssemblyFileVersion = "2.0.0"