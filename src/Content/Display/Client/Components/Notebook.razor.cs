namespace Crudspa.Content.Display.Client.Components;

public partial class Notebook : IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter, EditorRequired] public NotebookModel Model { get; set; } = null!;

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

public class NotebookModel(IScrollService scrollService, INotebookRunService notebookRunService)
    : ModalModel(scrollService), IHandle<ValidateBinder>
{
    public Shared.Contracts.Data.Notebook? Notebook
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<NotepageModel> NotepageModels
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public NotepageModel? CurrentPage
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean HasFetched
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean IsValidating
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean IsEmpty
    {
        get;
        set => SetProperty(ref field, value);
    } = true;

    public Boolean CanMoveBack
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean CanMoveNext
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32 PageCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32 PageNumber
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? PositionText
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ValidationText
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Task Handle(ValidateBinder payload)
    {
        if (IsValidating)
            Validate();

        return Task.CompletedTask;
    }

    public async Task Show(NoteElement? noteToAdd = null)
    {
        Reset();

        await base.Show();

        var response = await WithWaiting("Fetching...", () => notebookRunService.FetchNotebookByContact(new()));

        if (response.Ok)
        {
            Notebook = response.Value;

            foreach (var notepage in Notebook.Notepages)
                NotepageModels.Add(new(notepage, false));

            var currentPage = noteToAdd is not null ? FindOrAddNotePage(noteToAdd) : NotepageModels.LastOrDefault();

            CurrentPage = currentPage;

            HasFetched = true;
            IsEmpty = NotepageModels.Count == 0;
            PageCount = NotepageModels.Count;
            PageNumber = currentPage is null ? 1 : NotepageModels.IndexOf(currentPage) + 1;

            UpdateState();
        }
    }

    public async Task Save()
    {
        if (!Validate())
            return;

        WaitingOn = "Saving...";
        Waiting = true;

        foreach (var model in NotepageModels)
            if (model.IsDirty)
                if (model.IsNew)
                    await notebookRunService.AddNotepage(new(model.Notepage));
                else
                    await notebookRunService.SaveNotepage(new(model.Notepage));

        await Hide();
        Waiting = false;
    }

    public void Reset()
    {
        HasFetched = false;
        IsEmpty = true;
        Notebook = null;
        NotepageModels = [];
        CanMoveBack = false;
        CanMoveNext = false;
        PositionText = String.Empty;
        PageCount = 0;
        PageNumber = 0;
        UpdateState();
    }

    public void MoveBack()
    {
        if (PageNumber <= 1)
            return;

        PageNumber--;
        UpdateState();

        CurrentPage = NotepageModels[PageNumber - 1];
    }

    public void MoveNext()
    {
        if (PageNumber >= PageCount)
            return;

        PageNumber++;
        UpdateState();

        CurrentPage = NotepageModels[PageNumber - 1];
    }

    private Boolean Validate()
    {
        var valid = true;

        foreach (var notepageModel in NotepageModels)
        {
            if (notepageModel.IsDirty)
            {
                if (notepageModel.Notepage.Note!.RequireText == true && notepageModel.Notepage.Text.HasNothing())
                {
                    valid = false;
                    ValidationText = "Text is required";
                }

                if (notepageModel.Notepage.Note.RequireImageSelection == true && !notepageModel.ImageSelected)
                {
                    valid = false;
                    ValidationText = "Selection is required";
                }
            }
        }

        IsValidating = !valid;

        return valid;
    }

    private NotepageModel FindOrAddNotePage(NoteElement noteToAdd)
    {
        var notepageModel = NotepageModels.FirstOrDefault(x => x.Notepage.Note!.Id.Equals(noteToAdd.Id));

        if (notepageModel is not null)
            return notepageModel;

        var notepage = new Notepage
        {
            NotebookId = Notebook!.NotebookId,
            NoteId = noteToAdd.Id,
            Note = noteToAdd,
        };

        notepageModel = new(notepage, true);

        NotepageModels.Add(notepageModel);

        return notepageModel;
    }

    private void UpdateState()
    {
        CanMoveBack = PageNumber > 1;
        CanMoveNext = PageNumber < PageCount;
        PositionText = $"{PageNumber:D} of {PageCount:D}";

        if (CurrentPage is not null)
            CurrentPage.UpdateState();
    }
}

public class NotepageModel : Observable
{
    private Notepage _notepage;
    private Boolean _isNew;
    private Boolean _isDirty;
    private ImageFile? _selectedImage;

    public NotepageModel(Notepage notepage, Boolean isNew)
    {
        _notepage = notepage;
        _isNew = isNew;

        if (_notepage.Note?.RequireImageSelection == true)
            _selectedImage = _notepage.SelectedImageFile;

        _isDirty = _isNew;

        UpdateState();
    }

    public Notepage Notepage
    {
        get => _notepage;
        set => SetProperty(ref _notepage, value);
    }

    public Boolean IsNew
    {
        get => _isNew;
        set => SetProperty(ref _isNew, value);
    }

    public Boolean IsDirty
    {
        get => _isDirty;
        set => SetProperty(ref _isDirty, value);
    }

    public Boolean RequiresInteraction
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean ShowEdit
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ImageFile? SelectedImage
    {
        get => _selectedImage;
        set => SetProperty(ref _selectedImage, value);
    }

    public Boolean ImageSelected
    {
        get;
        set => SetProperty(ref field, value);
    }

    public void Edit()
    {
        IsDirty = true;
        SelectedImage = null;
        UpdateState();
    }

    public void SelectImage(Guid? id)
    {
        var noteImage = _notepage.Note!.NoteImages.FirstOrDefault(x => x.Id.Equals(id));

        if (noteImage is not null)
        {
            _notepage.SelectedImageFileId = noteImage.ImageFileId;
            SelectedImage = noteImage.ImageFile;
        }

        UpdateState();
    }

    public void UpdateState()
    {
        RequiresInteraction = _notepage.Note?.RequireText == true || _notepage.Note?.RequireImageSelection == true;

        ShowEdit = !_isNew && !IsDirty && (_notepage.Note?.RequireImageSelection != true || _notepage.SelectedImageFile is not null);

        ImageSelected = SelectedImage?.Id is not null;
    }
}