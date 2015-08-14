#r "packages/FAKE.4.1.1/tools/FakeLib.dll"
#r "packages/NUnit.2.6.4/lib/nunit.framework.dll"

open Fake

RestorePackages()

// Properties
let buildDir = "./build/"
let testDir  = "./build/tests/"

// Targets
Target "Clean" (fun _ ->
    CleanDir buildDir
)

Target "BuildApp" (fun _ ->
    !! "/TvTamer.sln"
      |> MSBuildRelease buildDir "Build"
      |> Log "AppBuild-Output: "
)

Target "BuildTests" (fun _ ->
    !! "tests/**/*.csproj"
      |> MSBuildDebug testDir "Build"
      |> Log "TestBuild-Output: "
)

Target "RunUnitTests" (fun _ ->
    trace "Running Unit Tests"
    !!  (testDir + "*unittest*.dll")
      |> NUnit (fun p ->
          {p with
             ToolPath = "./packages/NUnit.Runners.2.6.4/tools"
             ToolName = "nunit-console.exe"
             DisableShadowCopy = true;
             OutputFile = testDir + "TestResults.xml" })
)

Target "Default" (fun _ ->
    trace "Hello World from FAKE"
)

// Dependencies
"Clean"
  ==> "BuildTests"
  ==> "BuildApp"
  ==> "RunUnitTests"
  ==> "Default"

// start build
RunTargetOrDefault "Default"