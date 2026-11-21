using ImHelper.Models;

namespace ImHelper.Services;

public interface IUserDirectory
{
    IReadOnlyList<DirectoryUser> SearchContacts(ContactSearchField field, string? searchText);

    IReadOnlyList<ManagedUser> GetUsersForContact(string contactId);
}
