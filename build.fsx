#r "paket: groupref Build //"
#load "./.fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.Core.TargetOperators
open Fake.DotNet
open Fake.IO
open Fake.IO.Globbing.Operators

let buildDir = "./build/"
let tempDir = "./temp/"

let clientProject =
    "./src/Silencer.Extension.Frontend/Silencer.Extension.Frontend.fsproj"

let backgroundProject =
    "./src/Silencer.Extension.Background/Silencer.Extension.Background.fsproj"

let manifestFile = "./manifest.json"

let contentScriptFile = "./contentScript.js"

Target.create "Clean" (fun _ -> Shell.cleanDirs [ buildDir; tempDir ])

Target.create "Restore" (fun _ -> DotNet.restore id "")

Target.create "Build" (fun _ -> DotNet.build id "")

let determinProjectFolder (project: string) =
    project.Split('/')
    |> Array.last
    |> fun s -> s.Split('.')
    |> fun pts ->
        let len = Array.length pts
        pts.[len - 2]
    |> fun s -> s.ToLowerInvariant()

let publishExtension (project: string) isDebug =
    let projectfolder = determinProjectFolder project

    DotNet.publish (fun c ->
        { c with
              OutputPath = Some(tempDir + projectfolder)
              Configuration = if isDebug then DotNet.BuildConfiguration.Debug else DotNet.BuildConfiguration.Release
              Framework = Some "net5.0" }) project

Target.create "PublishExtensionDebug" (fun _ ->
    publishExtension clientProject true
    publishExtension backgroundProject true)

Target.create "PublishExtensionRelease" (fun _ ->
    publishExtension clientProject false
    publishExtension backgroundProject false)

let hackTogetherProject (isCursedUiHack: bool) (project: string) =
    let tempRoot = tempDir + project + "/wwwroot/"

    let filesWithoutCompression =
        !!(tempRoot + "**/*.*")
        -- (tempRoot + "**/*.br")
        -- (tempRoot + "**/*.gz")
        -- (tempRoot + "**/*.ico")

    let buildDir = sprintf "%s%s/" buildDir project

    filesWithoutCompression
    |> GlobbingPattern.setBaseDir tempRoot
    |> Shell.copyFilesWithSubFolder buildDir

    Shell.rename (buildDir + "framework") (buildDir + "_framework")

    let fwTarget =
        if isCursedUiHack then project + "/framework" else "framework"

    let replaceFw (s: string) = s.Replace("_framework", fwTarget)

    let replaceBuild (s: string) = s.Replace("_bin", "bin")

    let cssTarget =
        if isCursedUiHack then project + "/css/" else "css/"

    let replaceCss (s: string) = s.Replace("css/", cssTarget)

    let scriptsTarget =
        if isCursedUiHack then project + "/scripts/" else "scripts/"

    let replaceScripts (s: string) = s.Replace("scripts/", scriptsTarget)

    let webassemblyJs =
        buildDir + "framework/blazor.webassembly.js"

    let index = buildDir + "index.html"

    let replaceUnderscoreDirs file =
        File.readAsString file
        |> replaceFw
        |> replaceBuild
        |> replaceCss
        |> replaceScripts
        |> File.replaceContent file

    [ webassemblyJs; index ]
    |> List.iter replaceUnderscoreDirs

Target.create "HackTogetherExtensions" (fun _ ->
    determinProjectFolder clientProject
    |> hackTogetherProject true

    determinProjectFolder backgroundProject
    |> hackTogetherProject true

    Shell.copyFile buildDir manifestFile
    Shell.copyFile buildDir contentScriptFile)

Target.create "Default" ignore
Target.create "BuildDebug" ignore
Target.create "BuildRelease" ignore

"Clean" ==> "Restore" ==> "Build" ==> "Default"

"Clean"
==> "Restore"
==> "PublishExtensionDebug"
==> "HackTogetherExtensions"
==> "BuildDebug"

"Clean"
==> "Restore"
==> "PublishExtensionRelease"
==> "HackTogetherExtensions"
==> "BuildRelease"

Target.runOrDefault "Default"
