#region Enum - PathType
/// <summary>
/// Types of paths for different purposes
/// </summary>
public enum PathType
{
    /// <summary>Normal path between goals (Gold, Thick)</summary>
    Normal = 0,

    /// <summary>Return path to parking (Green, Thin, Dashed)</summary>
    Return = 1,

    /// <summary>Charging path to/from parking (LightBlue, Normal)</summary>
    Charging = 2
}
#endregion