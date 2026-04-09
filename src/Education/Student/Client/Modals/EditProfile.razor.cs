namespace Crudspa.Education.Student.Client.Modals;

public partial class EditProfile : IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter, EditorRequired] public EditProfileModel Model { get; set; } = null!;

    protected override Task OnInitializedAsync()
    {
        Model.PropertyChanged += HandleModelChanged;
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class EditProfileModel : ModalModel
{
    private void HandleStudentChanged(Object? sender, PropertyChangedEventArgs e) => RaisePropertyChanged(nameof(Student));
    private readonly IStudentAppService _studentAppService;

    public EditProfileModel(IScrollService scrollService, IStudentAppService studentAppService)
        : base(scrollService)
    {
        _studentAppService = studentAppService;

        AvatarStrings = Emoji.FunAvatars().Select(x => x.Character).ToList();
    }

    public Shared.Contracts.Data.Student? Student
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<String> AvatarStrings { get; }

    public override async Task Hide()
    {
        await base.Hide();
        await Refresh();
    }

    public void SelectAvatar(String avatar)
    {
        Student!.AvatarString = avatar;
    }

    public async Task Refresh()
    {
        var response = await WithWaiting("Fetching...", () => _studentAppService.FetchStudent(new()));

        if (response.Ok)
            SetStudent(response.Value);
    }

    public async Task Save()
    {
        if (Student is null)
            return;

        var response = await WithWaiting("Saving...", () => _studentAppService.SaveProfile(new(Student)));

        if (response.Ok)
            await Hide();
    }

    private void SetStudent(Shared.Contracts.Data.Student student)
    {
        if (Student is not null)
            Student.PropertyChanged -= HandleStudentChanged;

        Student = student;
        Student.PropertyChanged += HandleStudentChanged;
    }
}