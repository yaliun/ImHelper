namespace ImHelper.Models;

public sealed record DirectoryUser(
    string Id,
    string EmployeeNumber,
    string UserName,
    string Department);

public sealed record ManagedUser(
    string UserId,
    string EmployeeNumber,
    string UserName,
    string ContactId,
    string ContactDepartment);

public sealed record ManagedUserPreview(
    string UserId,
    string EmployeeNumber,
    string UserName,
    string ContactId,
    string ContactDepartment);
