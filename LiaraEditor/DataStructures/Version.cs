using System.Runtime.Serialization;
using System.Text;
using LiaraEditor.Exceptions;

namespace LiaraEditor.DataStructures
{
    /// <summary>
    /// This enum represents the type of version release.
    /// </summary>
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
    
    /// <summary>
    /// Represents a version number with major, minor, patch, build, release label, and build metadata components.
    /// </summary>
    /// <remarks>
    /// This type is immutable.
    /// </remarks>
    /// <example>
    /// Version v = new Version(1, 2, 3, 4, ReleaseType.Alpha, "build+123");
    /// v.ToString() returns "1.2.3.4-alpha-build+123"
    /// </example>
    [DataContract]
    public struct Version
    {
        /// <summary>
        /// Gets or sets the major version number.
        /// </summary>
        [DataMember]
        public int Major { get; set; }

        /// <summary>
        /// Gets or sets the minor version number.
        /// </summary>
        [DataMember]
        public int Minor { get; set; }

        /// <summary>
        /// Gets or sets the patch version number.
        /// </summary>
        [DataMember]
        public int Patch { get; set; }

        /// <summary>
        /// Gets or sets the build version number.
        /// </summary>
        [DataMember]
        public int Build { get; set; }

        /// <summary>
        /// Gets or sets the release label for this version.
        /// </summary>
        /// <remarks>
        /// The release label is used to indicate an alpha version, a "LTS" version, or any other label.
        /// </remarks>
        [DataMember]
        public ReleaseType ReleaseLabel { get; set; }

        /// <summary>
        /// Gets or sets the build metadata for this version.
        /// </summary>
        /// <remarks>
        /// The build metadata is used to indicate the build date or other information, like the name of a custom release.
        /// </remarks>
        [DataMember]
        public string BuildMetadata { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Version"/> struct with the specified components.
        /// </summary>
        /// <param name="major">The major version number.</param>
        /// <param name="minor">The minor version number.</param>
        /// <param name="patch">The patch version number.</param>
        /// <param name="build">The build version number (optional).</param>
        /// <param name="releaseLabel">The release label (optional).</param>
        /// <param name="buildMetadata">The build metadata (optional).</param>
        /// <remarks>
        /// The build version number, release label, and build metadata are optional.
        /// If the build version number is not specified, it is set to 0.
        /// If the release label is not specified, it is set to <see cref="ReleaseType.Custom"/>.
        /// If the build metadata is not specified, it is set to null.
        /// </remarks>
        public Version(int major, int minor, int patch, int build = 0, ReleaseType releaseLabel = ReleaseType.Custom, string buildMetadata = null)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            Build = build;
            ReleaseLabel = releaseLabel;
            BuildMetadata = buildMetadata;
        }
        
        /// <summary>
        /// Converts the version components to a string representation.
        /// </summary>
        /// <returns>A string representation of the version.</returns>
        /// <seealso cref="Parse(string)"/>
        /// <seealso cref="ToString()"/>
        /// <seealso cref="Version"/>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Major);
            sb.Append('.');
            sb.Append(Minor);
            sb.Append('.');
            sb.Append(Patch);
            sb.Append('.');
            sb.Append(Build);
            sb.Append('-');
            sb.Append(ReleaseLabel.ToString().ToLowerInvariant());
            if (!string.IsNullOrEmpty(BuildMetadata))
            {
                sb.Append('+');
                sb.Append(BuildMetadata);
            }
            return sb.ToString();
        }
        
        /// <summary>
        /// Converts the string representation of a version to its <see cref="Version"/> equivalent.
        /// </summary>
        /// <param name="version">A string containing a version to convert.</param>
        /// <returns>A <see cref="Version"/> equivalent to the version contained in <paramref name="version"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="version"/> is null.</exception>
        /// <exception cref="System.FormatException"><paramref name="version"/> is not in the correct format.</exception>
        /// <exception cref="VersionParseException"><paramref name="version"/> is not in the correct format.</exception>
        /// <seealso cref="Parse(string)"/>
        /// <seealso cref="ToString()"/>
        /// <seealso cref="Version"/>
        public static Version Parse(string version)
        {
            if (version == null)
            {
                throw new VersionParseException(VersionParseException.VersionParseError.VersionStringIsEmpty);
            }

            // Split the version string into components
            string[] versionComponents = version.Split('.', '-');

            // Check if the number of components is valid
            if (versionComponents.Length < 3 || versionComponents.Length > 6)
            {
                throw new VersionParseException(VersionParseException.VersionParseError.VersionStringIsInvalid);
            }

            // Parse each component
            if (!int.TryParse(versionComponents[0], out int major))
            {
                throw new VersionParseException(VersionParseException.VersionParseError.MajorVersionIsInvalid);
            }
            if (!int.TryParse(versionComponents[1], out int minor))
            {
                throw new VersionParseException(VersionParseException.VersionParseError.MinorVersionIsInvalid);
            }
            if (!int.TryParse(versionComponents[2], out int patch))
            {
                throw new VersionParseException(VersionParseException.VersionParseError.PatchVersionIsInvalid);
            }
            if (!int.TryParse(versionComponents[3], out int build))
            {
                build = 0;
            }

            // Parse release label and build metadata if present
            ReleaseType releaseLabel = ReleaseType.Experimental;
            string buildMetadata = null;
            if (versionComponents.Length >= 5)
            {
                if (!Enum.TryParse(versionComponents[4], true, out releaseLabel))
                {
                    throw new VersionParseException(VersionParseException.VersionParseError.ReleaseLabelIsInvalid);
                }
            }
            if (versionComponents.Length == 6)
            {
                buildMetadata = versionComponents[5];
            }

            // Create and return a new Version instance
            return new Version(major, minor, patch, build, releaseLabel, buildMetadata);
        }
        
        /// <summary>
        /// Converts the version components to a string representation.
        /// </summary>
        /// <returns>A string representation of the version.</returns>
        /// <param name="version">The version to convert.</param>
        /// <seealso cref="ToString()"/>
        /// <seealso cref="Parse(string)"/>
        /// <seealso cref="Version"/>
        public static implicit operator string(Version version)
        {
            return version.ToString();
        }
        
        /// <summary>
        /// Converts the string representation of a version to its <see cref="Version"/> equivalent.
        /// </summary>
        /// <param name="version">A string containing a version to convert.</param>
        /// <returns>A <see cref="Version"/> equivalent to the version contained in <paramref name="version"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="version"/> is null.</exception>
        /// <exception cref="System.FormatException"><paramref name="version"/> is not in the correct format.</exception>
        /// <exception cref="VersionParseException"><paramref name="version"/> is not in the correct format.</exception>
        /// <seealso cref="Parse(string)"/>
        /// <seealso cref="ToString()"/>
        /// <seealso cref="Version"/>
        public static implicit operator Version(string version)
        {
            return Parse(version);
        }
        
        /// <summary>
        /// Returns the hash code for this version.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return Major.GetHashCode() ^ Minor.GetHashCode() ^ Patch.GetHashCode() ^ Build.GetHashCode() ^ ReleaseLabel.GetHashCode() ^ BuildMetadata.GetHashCode();
        }
        
        /// <summary>
        /// Determines whether this version and the specified <see cref="Version"/> object have the same value.
        /// </summary>
        /// <param name="obj">The <see cref="Version"/> object to compare to this instance.</param>
        /// <returns>A boolean indicating if the two versions are equal.</returns>
        /// <seealso cref="Equals(Version)"/>
        /// <seealso cref="operator ==(Version, Version)"/>
        /// <seealso cref="operator !=(Version, Version)"/>
        public override bool Equals(object obj)
        {
            if (obj is Version version)
            {
                return Equals(version);
            }
            return false;
        }
        
        /// <summary>
        /// Determines whether this version and the specified <see cref="Version"/> object have the same value.
        /// </summary>
        /// <param name="other">The <see cref="Version"/> object to compare to this instance.</param>
        /// <returns>A boolean indicating if the two versions are equal.</returns>
        /// <seealso cref="Equals(object)"/>
        /// <seealso cref="operator ==(Version, Version)"/>
        /// <seealso cref="operator !=(Version, Version)"/>
        public bool Equals(Version other)
        {
            return Major == other.Major && Minor == other.Minor && Patch == other.Patch && ReleaseLabel == other.ReleaseLabel;
        }
        
        /// <summary>
        /// Compares two <see cref="Version"/> objects.
        /// </summary>
        /// <param name="version1">The first version to compare.</param>
        /// <param name="version2">The second version to compare.</param>
        /// <returns>A boolean indicating if the two versions are equal.</returns>
        /// <seealso cref="Equals(Version)"/>
        /// <seealso cref="operator !=(Version, Version)"/>
        public static bool operator ==(Version version1, Version version2)
        {
            return version1.Equals(version2);
        }
        
        /// <summary>
        /// Compares two <see cref="Version"/> objects.
        /// </summary>
        /// <param name="version1">The first version to compare.</param>
        /// <param name="version2">The second version to compare.</param>
        /// <returns>A boolean indicating if the two versions are not equal.</returns>
        /// <seealso cref="Equals(Version)"/>
        /// <seealso cref="operator ==(Version, Version)"/>
        public static bool operator !=(Version version1, Version version2)
        {
            return !version1.Equals(version2);
        }
        
        /// <summary>
        /// Compares two <see cref="Version"/> objects.
        /// </summary>
        /// <param name="version1">The first version to compare.</param>
        /// <param name="version2">The second version to compare.</param>
        /// <returns>A boolean indicating if the first version is greater than the second version.</returns>
        public static bool operator >(Version version1, Version version2)
        {
            if (version1.Major != version2.Major) { return version1.Major > version2.Major; }
            if (version1.Minor != version2.Minor) { return version1.Minor > version2.Minor; }
            if (version1.Patch != version2.Patch) { return version1.Patch > version2.Patch; }
            if (version1.ReleaseLabel != version2.ReleaseLabel) { return version1.ReleaseLabel > version2.ReleaseLabel; }
            return false;
        }
        
        /// <summary>
        /// Compares two <see cref="Version"/> objects.
        /// </summary>
        /// <param name="version1">The first version to compare.</param>
        /// <param name="version2">The second version to compare.</param>
        /// <returns>A boolean indicating if the first version is greater than or equal to the second version.</returns>
        public static bool operator >=(Version version1, Version version2)
        {
            if (version1.Major != version2.Major) { return version1.Major > version2.Major; }
            if (version1.Minor != version2.Minor) { return version1.Minor > version2.Minor; }
            if (version1.Patch != version2.Patch) { return version1.Patch > version2.Patch; }
            if (version1.ReleaseLabel != version2.ReleaseLabel) { return version1.ReleaseLabel > version2.ReleaseLabel; }
            return true;
        }
        
        /// <summary>
        /// Compares two <see cref="Version"/> objects.
        /// </summary>
        /// <param name="version1">The first version to compare.</param>
        /// <param name="version2">The second version to compare.</param>
        /// <returns>A boolean indicating if the first version is less than the second version.</returns>
        public static bool operator <(Version version1, Version version2)
        {
            if (version1.Major != version2.Major) { return version1.Major < version2.Major; }
            if (version1.Minor != version2.Minor) { return version1.Minor < version2.Minor; }
            if (version1.Patch != version2.Patch) { return version1.Patch < version2.Patch; }
            if (version1.ReleaseLabel != version2.ReleaseLabel) { return version1.ReleaseLabel < version2.ReleaseLabel; }
            return false;
        }
        
        /// <summary>
        /// Compares two <see cref="Version"/> objects.
        /// </summary>
        /// <param name="version1">The first version to compare.</param>
        /// <param name="version2">The second version to compare.</param>
        /// <returns>A boolean indicating if the first version is less than or equal to the second version.</returns>
        public static bool operator <=(Version version1, Version version2)
        {
            if (version1.Major != version2.Major) { return version1.Major < version2.Major; }
            if (version1.Minor != version2.Minor) { return version1.Minor < version2.Minor; }
            if (version1.Patch != version2.Patch) { return version1.Patch < version2.Patch; }
            if (version1.ReleaseLabel != version2.ReleaseLabel) { return version1.ReleaseLabel < version2.ReleaseLabel; }
            return true;
        }
        
        /// <summary>
        /// Compares two <see cref="Version"/> objects.
        /// </summary>
        /// <param name="version1">The first version to compare.</param>
        /// <param name="version2">The second version to compare.</param>
        /// <returns>A boolean indicating if the first version is greater than the second version.</returns>
        public static bool GreaterThan(Version version1, Version version2)
        {
            return version1 > version2;
        }
        
        /// <summary>
        /// Compares two <see cref="Version"/> objects.
        /// </summary>
        /// <param name="version1">The first version to compare.</param>
        /// <param name="version2">The second version to compare.</param>
        /// <returns>A boolean indicating if the first version is greater than or equal to the second version.</returns>
        public static bool GreaterThanOrEqual(Version version1, Version version2)
        {
            return version1 >= version2;
        }
    }
}
