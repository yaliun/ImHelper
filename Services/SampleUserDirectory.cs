using ImHelper.Models;

namespace ImHelper.Services;

public sealed class SampleUserDirectory : IUserDirectory
{
    private readonly List<DirectoryUser> contacts =
    [
        new("contact-a", "10001", "김현업", "정보기술부"),
        new("contact-b", "10002", "박담당", "경영지원부"),
        new("contact-c", "10003", "이담당", "정보기술부"),
        new("contact-d", "10004", "최담당", "사업운영부")
    ];

    private readonly List<ManagedUser> users =
    [
        new("user-001", "20001", "강사용자", "contact-a", "정보기술부"),
        new("user-002", "20002", "윤사용자", "contact-a", "정보기술부"),
        new("user-003", "20003", "장사용자", "contact-a", "정보기술부"),
        new("user-004", "20004", "조사용자", "contact-b", "경영지원부"),
        new("user-005", "20005", "임사용자", "contact-c", "정보기술부"),
        new("user-006", "20006", "한사용자", "contact-d", "사업운영부")
    ];

    public IReadOnlyList<DirectoryUser> SearchContacts(ContactSearchField field, string? searchText)
    {
        return contacts
            .Where(contact => Matches(field switch
            {
                ContactSearchField.Id => contact.Id,
                ContactSearchField.EmployeeNumber => contact.EmployeeNumber,
                ContactSearchField.UserName => contact.UserName,
                _ => throw new ArgumentOutOfRangeException(nameof(field), field, "지원하지 않는 검색 항목입니다.")
            }, searchText))
            .ToList();
    }

    public IReadOnlyList<ManagedUser> GetUsersForContact(string contactId)
    {
        return users
            .Where(user => string.Equals(user.ContactId, contactId, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    private static bool Matches(string value, string? searchText)
    {
        return string.IsNullOrWhiteSpace(searchText)
            || value.Contains(searchText.Trim(), StringComparison.OrdinalIgnoreCase);
    }
}
