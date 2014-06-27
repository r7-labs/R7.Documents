# Initial setup of packaging project

This template uses MSBuild to create install and source packages,
but due to template engine limitations it will require some changes 
to be made manually in a project file.

1. Install MSBuild.Community.Tasks extension from https://github.com/loresoft/msbuildtasks/releases

2. Open Packages.csproj in text editor or add it to project files.

3. Just after last '<Import Project="" />' directive insert this code:

<!-- Begin snippet -->
<Import Project="$(MSBuildExtensionsPath)\MSBuildCommunityTasks\MSBuild.Community.Tasks.Targets" />
<PropertyGroup>
	<PackageExtension>zip</PackageExtension>
	<PackageName>R7.Documents</PackageName>
	<PackageOutputPath>output</PackageOutputPath>
	<MSBuildDnnBinPath Condition="'$(MSBuildDnnBinPath)' == ''">..\..\..\bin</MSBuildDnnBinPath>
</PropertyGroup>
<Import Project="Install.targets" />
<Import Project="Source.targets" />
<Target Name="AfterBuild" DependsOnTargets="MakeInstallPackage;MakeSourcePackage" />
<!-- End snippet -->

4. Now switch to 'Release' configuration and execute 'Build All' command.

5. After that in a 'Packages\output' folder you should find two .zip archives - 
one for install and one for source packages. 

6. Now you can install extension from 'R7.Documents-01.00.00-Install.zip' package  
through DNN > Host > Extensions, as usual.

# Extending solution

Current scripts automatically add any new DNN project inside solution into source package
and Resources.zip in the install package. The general idea is that all projects in the solution 
is very dependant and must go to a single DNN install package.

So if you add new DNN extension to your solution using R7.DnnTemplates, you probably only need this:

1. Move Packages Project/EndProject entry in R7.Documents.sln file to the bottom, to ensure
that Packages project will build last. 

2. Add new <package> section to R7.Documents.dnn manifest. If you create new extension 
with R7.DnnTemplates, it shoud be pretty simple. Just copy entire <package> section from
newly created manifest and remove Scripts components. 

3. Merge .SqlDataProvider files of newly created extension with ones in the R7.Documents extension (if needed).

# General tips

* After external .sln, .csproj and .target file changes you probably need to reload solution. 


