# Liara Editor - Data Structure
## Table of Contents
- ### [Table of Contents](#table-of-contents)
- ### [Introduction](#Introduction)
- ### [Version](#version)
  - #### [Version Structure](#version-structure)
  - #### [Version Methods](#version-methods)
  - #### [Version Operators](#version-operators)
  - #### [Version Comparisons](#version-comparisons)
  - #### [Version Parsing](#version-parsing)
  - #### [Version Serialization](#version-serialization)
  - #### [Version List and Chanlog](#version-list-and-chanlog)
- ### [Project Templates](#project-templates)
  - #### [Project Template Types](#project-template-types)
  - #### [Project Template Structure](#project-template-structure)
  - #### [Project Template XML](#project-template-xml)
  - #### [Project Template .liaraproj](#project-template-liaraproj)
- ### [Project Files]
  - #### [Project Structure]
  - #### [Project .liaraproj]
- ### [Scene]
- ### [Creating your own Templates]

## Introduction
For full information on the Liara Editor, please see the **[Liara Editor Documentation](LiaraEditor.md)**.</br>
This document will cover the data structure of the Liara Editor, including the templates, projects, and scenes.</br>
At the moment, this document covers the **0.0.3** version of the Liara Editor features.</br>
***This document is subject to change as the Liara Editor is developed.***</br>
The objective of this document is to provide a reference for the Liara Editor datas structures, like the templates, projects, and scenes, and the `.liaraproj` files.</br>
***This document is not a tutorial on how to use the Liara Editor.***</br>

## Version
The Liara Editor uses a custom version system, that is based on the [Semantic Versioning](https://semver.org/) system.</br>

### Version Structure
The Liara Editor version is composed of 4 numbers, separated by a dot (`.`), like this : `0.0.0.1`.</br>
The first number is the major version, the second number is the minor version, the third number is the patch version, and the fourth number is the build version.</br>
The tree first numbers are required, but the fourth number is optional and not used for comparison, but it's used to get the Hash of the version.</br>
After this, you can add a `-` and a `ReleaseType` enum value, like this : `-beta`.</br>
The `ReleaseType` enum is the following :
```csharp
public enum ReleaseType
    {
        Experimental = 0, // Not stable
        Alpha = 1, // Stable enough to be released
        Beta = 2, // More stable than experimental, but not stable enough to be released
        Stable = 3, // Stable
        Lts = 4, // Long Term Support
        Obsolete = 5, // Obsolete and not supported anymore
        Custom = 6 // Custom release type, for example a version released by a modder. In this case, the build metadata should contain the name of this modifcation, for example "0.0.0.0-custom-MyMod0+0+1"
    }
```
The `ReleaseType` enum is used to indicate the stability of the version.</br>
The `ReleaseType` enum is not required, but if it's not present, the version is considered as a `Custom` version.</br>
After this, you can add a `-` and a `BuildMetadata` string, like this : `-MyMod0+0+1`, or `-build+2023+11+22`.</br>
The `BuildMetadata` string is used to indicate the build metadata of the version, and it's not used for comparison. As such, it's optional.</br>
The `BuildMetadata` can contain any character except the `.` and the `-` characters, because they are used to separate the values.</br>
We recommend to use the `+` character to separate the values, and use the following format : `+<Year>+<Month>+<Day>`, like this : `+2023+11+22`.</br>

### Version Methods
The Liara Editor version has a few methods, that are the following :
- **`public Version(int major, int minor, int patch, int build = 0, ReleaseType releaseType = ReleaseType.Custom, string buildMetadata = null)`** : The constructor of the version.
- **`public override string ToString()`** : Returns the string representation of the version.
- **`public Version Parse(string versionString)`** : Parses the string representation of the version.
- **`public string GetHash()`** : Returns the hash of the version.
- **`public bool Equals(Version other)`** : Returns true if the version is equal to the other version.
- **`public bool GreaterThan(Version other)`** : Returns true if the version is greater than the other version.
- **`public bool GreaterThanOrEqual(Version other)`** : Returns true if the version is greater than or equal to the other version.

*This list will be updated as the Liara Editor is developed.*

### Version Operators
The Liara Editor version has a few operators, that are the following :
- **`public static bool operator ==(Version left, Version right)`** : Returns true if the left version is equal to the right version.
- **`public static bool operator !=(Version left, Version right)`** : Returns true if the left version is not equal to the right version.
- **`public static bool operator >(Version left, Version right)`** : Returns true if the left version is greater than the right version.
- **`public static bool operator <(Version left, Version right)`** : Returns true if the left version is less than the right version.
- **`public static bool operator >=(Version left, Version right)`** : Returns true if the left version is greater than or equal to the right version.
- **`public static bool operator <=(Version left, Version right)`** : Returns true if the left version is less than or equal to the right version.

*This list will be updated as the Liara Editor is developed.*

### Version Comparisons
#### Equality
The equality of two versions is determined by the following rules :
```csharp
return Major == other.Major && Minor == other.Minor && Patch == other.Patch && ReleaseLabel == other.ReleaseLabel;
```
You can see that the build version and the build metadata are not used for the equality comparison.</br>

#### Greater Than
The greater than of two versions is determined by the following rules :
```csharp
if (version1.Major != version2.Major) { return version1.Major > version2.Major; }
if (version1.Minor != version2.Minor) { return version1.Minor > version2.Minor; }
if (version1.Patch != version2.Patch) { return version1.Patch > version2.Patch; }
if (version1.ReleaseLabel != version2.ReleaseLabel) { return version1.ReleaseLabel > version2.ReleaseLabel; }
return false;
```
You can see that the build version and the build metadata are not used for the greater than comparison.</br>

#### Greater Than Or Equal
The greater than or equal of two versions is determined by the following rules :
```csharp
if (version1.Major != version2.Major) { return version1.Major > version2.Major; }
if (version1.Minor != version2.Minor) { return version1.Minor > version2.Minor; }
if (version1.Patch != version2.Patch) { return version1.Patch > version2.Patch; }
if (version1.ReleaseLabel != version2.ReleaseLabel) { return version1.ReleaseLabel > version2.ReleaseLabel; }
return true;
```
You can see that the build version and the build metadata are not used for the greater than or equal comparison.</br>

### Version Parsing
The Liara Editor version can be parsed from a string, using the `Version.Parse(string versionString)` method.</br>
The string representation of the version must be in the following format :
 - `<int>.<int>.<int>.<int>-<ReleaseTypeEnumValue>-<string>`.</br>
 - `<int>.<int>.<int>.<int>-<ReleaseTypeEnumValue>`.</br>
 - `<int>.<int>.<int>.<int>`.</br>
 - `<int>.<int>.<int>`.</br>

The `ReleaseTypeEnumValue` is the value of the `ReleaseType` enum, like `beta`, `alpha`, `stable`, etc.</br>
The `string` is the build metadata of the version, and it's optional.</br>

### Version Serialization
The Liara Editor version can be serialized to a string, using the `Version.ToString()` method.</br>
The string representation of the version is in the following format :
 - `<int>.<int>.<int>.<int>-<ReleaseTypeEnumValue>-<string>`.</br>
 - `<int>.<int>.<int>.<int>-<ReleaseTypeEnumValue>`.</br>
 - `<int>.<int>.<int>.<int>-<string>`.</br>
 - `<int>.<int>.<int>.<int>`.</br>
 - `<int>.<int>.<int>`.</br>

The `ReleaseTypeEnumValue` is the value of the `ReleaseType` enum, like `beta`, `alpha`, `stable`, etc.</br>
The `string` is the build metadata of the version, and it's optional.</br>

### Version List and Chanlog
 - **0.0.1** : Experimental version, with the basic features of the Liara Editor.
   - **0.0.1.1-experimental** : Experimental version, with just a white window and two buttons for change between the New Project Window and the Open Project Window.
   - **0.0.1.2-experimental** : Experimental version, with added the project templates in the New Project Window.
   - **0.0.1.3-experimental** : Experimental version, with added a large part of the logic and the design of the New Project Window.
      - **0.0.1.3-experimental-build+2023+11+20** : Experimental version, with added the design of the New Project Window.
      - **0.0.1.3-experimental-build+2023+11+22** : Experimental version, with added the logic of the New Project Window.
 - **0.0.2** : Experimental version, with a large refactoring of the code
   - **0.0.2.1-experimental** : Experimental version, description will be added later.
      - **0.0.2.1-experimental-build+2023+11+25** : Experimental version, refactored the project templates system, add a basic version system.

## Project Templates
A project template is a base project that can be used to create new projects with a lot of assets already set up.</br>
The Liara Editor comes with a few project templates (see the [Project Templates](#project-templates) section below), but you can also create your own (see [Creating your own Templates](#creating-your-own-templates)).</br>

### Project Template Types
At the moment (version 0.0.3), Liara Editor is distributed with one template category, and tree sub-categories.</br>
Template :
- **GamesTemplate** : A category that contains templates for games.
  - **Blank** : A blank template, with no assets, if you want to start from scratch.
  - **FirstPerson** : A template for a first person game.
  - **ThirdPerson** : A template for a third person game.

*This list will be updated as the Liara Editor is developed.*

### Project Template Structure
A project template is a folder that contains all the assets that will be copied by the Liara Editor when creating a new project.</br>
The structure of a project template is the following :
- **`<TemplateName>.xml`** : The template XML file, that contains the main information about the template
- **`<TemplateName>.liaraproj`** : The template project file, that contains the project information for the project, like the name, the version, the path, etc.
- **`<TemplateName>.liaraproj.meta`** : The template project meta file, that contains the meta information for the project, like the GUID, the creation date, etc.
- **`<TemplateName>.png`** : The template icon file, that contains the icon for the template, that will be displayed in the Liara Editor Creation Window.
- **`<TemplateName>_Preview.png`** : The template preview file, that contains the preview image for the template, that will be displayed in the Liara Editor Creation Window.

*This list will be updated as the Liara Editor is developed.*

### Project Template XML
The template XML file contains the main information about the template for the Liara Editor Creation Window, like the name, the description, ect ; and the list of all the files that will be copied by the Liara Editor when creating a new project.</br>
The structure of the template XML file is the following :
```xml
<ProjectTemplate xmlns="http://schemas.datacontract.org/2004/07/LiaraEditor.GameProject" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
  <ProjectType>GAMES</ProjectType>
  <Description>A blank template, with no assets, if you want to start from scratch.</Description>
  <Name>Blank</Name>
  <Author>Liara Team</Author>
  <Version>0.0.1</Version>
  <IsBeta>false</IsBeta>
  <IsExperimental>true</IsExperimental>
  <IsHidden>false</IsHidden>
  <IsObsolete>false</IsObsolete>
  <MinRequiredVersion>0.0.1</MinRequiredVersion>
  <RecommendedVersion>
    <a:Version>0.0.1</a:Version>
    // ...
  </RecommendedVersion>
  <Folders xmlns:a="http://schemas.microsoft.com/2003/10/Serialization/Arrays">
		<a:string>Content</a:string>
		<a:string>Scripts</a:string>
		<a:string>Misc</a:string>
		<a:string>Misc/Icons</a:string>
		<a:string>Misc/Preview</a:string>
		// ...
	</Folders>
	<HiddenFolders xmlns:a="http://schemas.microsoft.com/2003/10/Serialization/Arrays">
		<a:string>.Liara</a:string>
		// ...
	</HiddenFolders>
</ProjectTemplate>
```
- **`<ProjectType>`** : The type of the template. At the moment, the only included type is `GAMES`, but you can create your own types (see [Creating your own Templates](#creating-your-own-templates)).
- **`<Description>`** : The description of the template, that will be displayed in the Liara Editor Creation Window.
- **`<Name>`** : The name of the template, that will be displayed in the Liara Editor Creation Window.
- **`<Author>`** : The author of the template, that will be displayed in the Liara Editor Creation Window.
- **`<Version>`** : The version of the template, that will be displayed in the Liara Editor Creation Window.
- **`<IsBeta>`** : If the template is in beta, that will be displayed in the Liara Editor Creation Window to warn the user.
- **`<IsExperimental>`** : If the template is experimental, that will be displayed in the Liara Editor Creation Window to warn the user. *(Note 1 : A template can be both experimental and beta)(Note 2 : Experimental templates is less stable than beta templates)*
- **`<IsHidden>`** : If the template is hidden, that will not be displayed in the Liara Editor Creation Window by default. *(Note : At the moment, Liara Editor doesn't have a way to display hidden templates, but it will be added in the future)*
- **`<MinRequiredVersion>`** : The minimum required version of the Liara Engine to use the template. If the version of the Liara Engine is lower than the minimum required version, the template will not be displayed in the Liara Editor Creation Window.
- **`<IsObsolete>`** : If the template is obsolete, that will be displayed in the Liara Editor Creation Window to warn the user. *(Note : Obsolete templates are templates that are not supported anymore, and will be removed in the future)*
- **`<Folders>`** : The list of all the folders that will be copied by the Liara Editor when creating a new project. The folders are relative to the template folder. The minimum required folders are the `Content` and `Scripts` folders. If the `Misc` folder and its sub-folders isn't present, the Editor use a default icon and preview image, but the propertie `Misc` must be set to `false` in the liaraproj file.
- **`<HiddenFolders>`** : The list of all the hidden folders that will be copied by the Liara Editor when creating a new project. The folders are relative to the template folder. The minimum required folder is the `.Liara` folder.

The struct that represent the template XML file is the following :
```csharp
[DataContract]
public class ProjectTemplate
{
    [DataMember]
    public string ProjectType { get; set; }
    [DataMember]
    public string Description { get; set; }
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public string Author { get; set; }
    [DataMember]
    public string Version { get; set; }
    [DataMember]
    public bool IsBeta { get; set; }
    [DataMember]
    public bool IsExperimental { get; set; }
    [DataMember]
    public bool IsHidden { get; set; }
    [DataMember]
    public bool IsObsolete { get; set; }
    [DataMember]
    public string MinRequiredVersion { get; set; }
    [DataMember]
    
    [DataMember]
    public List<string> Folders { get; set; }
    [DataMember]
    public List<string> HiddenFolders { get; set; }
}
```

*This list will be updated as the Liara Editor is developed.*

### Project Template .liaraproj
The template project file contains the project information for the project, like the name, the version, the path, etc.</br>
The structure of the template project file is the following :
```xml
<Game z:Id="i1" xmlns="http://schemas.datacontract.org/2004/07/LiaraEditor.GameProject" xmlns:i="http://www.w3.org/2001/XMLSchema-instance" xmlns:z="http://schemas.microsoft.com/2003/10/Serialization/">
<Name>Blank</Name>
<Version>0.0.1</Version>
<Autor>Liara Studio</Autor>
<CreationDate>2023-11-22</CreationDate>
<LastModificationDate>2023-11-22</LastModificationDate>
<LastModificationHour>12:00:00</LastModificationHour>
<Misc>true</Misc>
<Path>C:\Users\Liara\LiaraEditor\ProjectTemplates\GamesTemplate\Blank</Path>
</Game>
```
- **`<Name>`** : The name of the project. *(Note : At the moment, the Liara Editor doesn't support a way to change the name of a project, but it will be eventually added in the future)*
- **`<Version>`** : The version of the project.
- **`<Autor>`** : The autor of the project.
- **`<CreationDate>`** : The creation date of the project.
- **`<LastModificationDate>`** : The last modification date of the project.
- **`<LastModificationHour>`** : The last modification hour of the project.
- **`<Misc>`** : If the `Misc` folder and its sub-folders is present in the template folder. If it's true, the Editor use the icon and preview image of the project, else it use a default icon and preview image.
- **`<Path>`** : The path of the project.

*This list will be updated as the Liara Editor is developed.*