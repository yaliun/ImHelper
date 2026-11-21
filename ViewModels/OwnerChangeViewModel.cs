using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ImHelper.Models;
using ImHelper.Services;

namespace ImHelper.ViewModels;

public sealed class OwnerChangeViewModel : INotifyPropertyChanged
{
    private readonly IUserDirectory userDirectory;
    private ContactSearchField oldContactSearchField = ContactSearchField.Id;
    private string oldContactSearchText = string.Empty;
    private ContactSearchField newContactSearchField = ContactSearchField.Id;
    private string newContactSearchText = string.Empty;
    private DirectoryUser? selectedOldContact;
    private DirectoryUser? selectedNewContact;

    public OwnerChangeViewModel(IUserDirectory userDirectory)
    {
        this.userDirectory = userDirectory;
        SearchOldContactsCommand = new DelegateCommand(SearchOldContacts);
        SearchNewContactsCommand = new DelegateCommand(SearchNewContacts);

        foreach (var contact in userDirectory.SearchContacts(ContactSearchField.Id, null))
        {
            OldContactResults.Add(contact);
            NewContactResults.Add(contact);
        }

        SelectedOldContact = OldContactResults.FirstOrDefault();
        SelectedNewContact = NewContactResults.Skip(1).FirstOrDefault();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<DirectoryUser> OldContactResults { get; } = [];

    public ObservableCollection<DirectoryUser> NewContactResults { get; } = [];

    public ObservableCollection<ManagedUserPreview> TargetUsers { get; } = [];

    public IReadOnlyList<ContactSearchOption> SearchOptions { get; } =
    [
        new("ID", ContactSearchField.Id),
        new("사번", ContactSearchField.EmployeeNumber),
        new("사용자명", ContactSearchField.UserName)
    ];

    public ICommand SearchOldContactsCommand { get; }

    public ICommand SearchNewContactsCommand { get; }

    public ContactSearchField OldContactSearchField
    {
        get => oldContactSearchField;
        set => SetProperty(ref oldContactSearchField, value);
    }

    public string OldContactSearchText
    {
        get => oldContactSearchText;
        set => SetProperty(ref oldContactSearchText, value);
    }

    public ContactSearchField NewContactSearchField
    {
        get => newContactSearchField;
        set => SetProperty(ref newContactSearchField, value);
    }

    public string NewContactSearchText
    {
        get => newContactSearchText;
        set => SetProperty(ref newContactSearchText, value);
    }

    public DirectoryUser? SelectedOldContact
    {
        get => selectedOldContact;
        set
        {
            if (SetProperty(ref selectedOldContact, value))
            {
                OnPropertyChanged(nameof(IsDepartmentChanged));
                RefreshTargetUsers();
            }
        }
    }

    public DirectoryUser? SelectedNewContact
    {
        get => selectedNewContact;
        set
        {
            if (SetProperty(ref selectedNewContact, value))
            {
                OnPropertyChanged(nameof(IsDepartmentChanged));
                RefreshTargetUsers();
            }
        }
    }

    public bool IsDepartmentChanged => SelectedOldContact is not null
        && SelectedNewContact is not null
        && !string.Equals(SelectedOldContact.Department, SelectedNewContact.Department, StringComparison.Ordinal);

    private void SearchOldContacts()
    {
        ReplaceResults(OldContactResults, userDirectory.SearchContacts(
            OldContactSearchField,
            OldContactSearchText));
        SelectedOldContact = null;
    }

    private void SearchNewContacts()
    {
        ReplaceResults(NewContactResults, userDirectory.SearchContacts(
            NewContactSearchField,
            NewContactSearchText));
        SelectedNewContact = null;
    }

    private void RefreshTargetUsers()
    {
        TargetUsers.Clear();
        if (SelectedOldContact is null || SelectedNewContact is null)
        {
            return;
        }

        foreach (var user in userDirectory.GetUsersForContact(SelectedOldContact.Id))
        {
            TargetUsers.Add(new ManagedUserPreview(
                user.UserId,
                user.EmployeeNumber,
                user.UserName,
                SelectedNewContact.Id,
                IsDepartmentChanged ? SelectedNewContact.Department : user.ContactDepartment));
        }
    }

    private static void ReplaceResults(ObservableCollection<DirectoryUser> target, IReadOnlyList<DirectoryUser> results)
    {
        target.Clear();
        foreach (var result in results)
        {
            target.Add(result);
        }
    }

    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private sealed class DelegateCommand(Action execute) : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter) => execute();
    }
}
