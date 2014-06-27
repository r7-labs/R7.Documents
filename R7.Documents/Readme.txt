# Template usage

Due to template engine limitations we need to do some will require some changes 
to be made manually in a project file.

1. Update reference to DotNetNuke.dll and other referenced assemblies to a DNN "bin" folder - if needed.

2. Template assumes that project is created in DNN "DesktopModules" folder with a parent solution folder 
   (same name as project's), so "..\..\..\bin" path is used to reference assemblies in DNN "bin" folder.

3. Set project build output path to DNN "bin" folder in the project options.
   It can be "..\..\..\bin" for the case, described above.
   
4. Build solution to check it out. Then you done with this, Documents.dll is located in a DNN "bin" folder.
  
5. You may want to disable automatic updates of CodeBehind partial classes in project options
   under "ASP.NET" page. Automatic updates is not working correctly with third-party controls 
   in the MonoDevelop / Xamarin Studio anyway. This is true at least for version 4.2.3.

6. Add to your solution new project of type "DNN packaging project" and follow instructions 
   in it's Readme.txt to create install package.

# Solution stucture for development

DNN
|- bin
|- Documents.dll
	|- Documents_Next.dll
|- DesktopModules
	|- Documents (solution)
		|- Documents (primary extension project)
			|- Documents.dnn (manifest)
        |- Documents_Next (secondary extension project) 
		|- Packages (packaging project)
			|- *.targets

