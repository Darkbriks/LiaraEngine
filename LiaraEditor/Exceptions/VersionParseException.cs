namespace LiaraEditor.Exceptions;

public class VersionParseException : Exception
{
    public enum VersionParseError
    {
        VersionStringIsEmpty,
        VersionStringIsInvalid,
        MajorVersionIsInvalid,
        MinorVersionIsInvalid,
        PatchVersionIsInvalid,
        BuildVersionIsInvalid,
        ReleaseLabelIsInvalid,
        UnknownError
    }
    
    private VersionParseError _error;
    
    public VersionParseException(VersionParseError error) : base(error.ToString())
    {
        _error = error;
    }
    
    public VersionParseError Error { get => _error; }
}