namespace ImHelper.Models;

public enum ContactSearchField
{
    Id,
    EmployeeNumber,
    UserName
}

public sealed record ContactSearchOption(string DisplayName, ContactSearchField Field);
