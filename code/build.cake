#load nuget:?package=Brierley.Cicd.Client.Build.Core&version=1.5.0

var target = Argument("target", "Default");

Task("Default")
    .IsDependentOn("SonarBegin")
    .IsDependentOn("Test")
    .IsDependentOn("SonarAnalyse")
    .IsDependentOn("Package");


RunTarget(target);