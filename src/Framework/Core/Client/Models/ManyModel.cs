using System.Collections.Specialized;

namespace Crudspa.Framework.Core.Client.Models;

public abstract class ManyModel<T> : ScreenModel
    where T : class, INamed, IObservable
{
    private void HandleFormsChanged(Object? sender, NotifyCollectionChangedEventArgs args)
    {
        if (!_updating) RaisePropertyChanged(nameof(Forms));
    }

    private void HandleEntityChanged(Object? sender, PropertyChangedEventArgs args)
    {
        if (!_updating) RaisePropertyChanged(nameof(Forms));
    }

    protected internal readonly IScrollService ScrollService;

    private ObservableCollection<FormModel<T>> _forms;
    private String? _filter;
    private Boolean _updating;
    private readonly Boolean _ascending;

    protected ManyModel(IScrollService scrollService, Boolean ascending = true)
    {
        ScrollService = scrollService;
        _forms = [];
        _forms.CollectionChanged += HandleFormsChanged;
        _ascending = ascending;
    }

    public override void Dispose()
    {
        _forms.CollectionChanged -= HandleFormsChanged;
        _forms.Reset(HandleEntityChanged);

        base.Dispose();
    }

    public ObservableCollection<FormModel<T>> Forms
    {
        get => _forms;
        set => SetProperty(ref _forms, value);
    }

    public String? Filter
    {
        get => _filter;
        set => SetProperty(ref _filter, value);
    }

    public void SetForms(IEnumerable<T>? list)
    {
        _updating = true;

        Forms.Reset(HandleEntityChanged, list!.Select(x => new FormModel<T>(x, ScrollService)));

        UpdateSort();
        ApplyFilter();
    }

    public async Task<FormModel<T>> CreateForm(T entity)
    {
        entity.PropertyChanged += HandleEntityChanged;

        var model = new FormModel<T>(entity, ScrollService, true);

        Forms.Add(model);

        UpdateSort();

        RaisePropertyChanged(nameof(Forms));

        await ScrollService.ToId(entity.Id!.Value);

        return model;
    }

    public void AddForm(T entity)
    {
        entity.PropertyChanged += HandleEntityChanged;

        Forms.Add(new(entity, ScrollService));

        UpdateSort();
        ApplyFilter();
    }

    public void ReplaceForm(FormModel<T> form, T entity)
    {
        form.Entity.PropertyChanged -= HandleEntityChanged;

        form.Entity = entity;
        form.ReadOnly = true;
        form.Alerts.Clear();

        form.Entity.PropertyChanged += HandleEntityChanged;

        UpdateSort();
        ApplyFilter();
    }

    public void RemoveForm(FormModel<T> form)
    {
        form.Entity.PropertyChanged -= HandleEntityChanged;
        Forms.Remove(form);
    }

    public async Task Replace(Guid? id, Guid? scopeId)
    {
        if (!InScope(scopeId))
            return;

        await Replace(id);
    }

    public async Task Replace(Guid? id)
    {
        var response = await Fetch(id);

        if (!response.Ok)
            return;

        var form = Forms.FirstOrDefault(x => Matches(id, x));

        if (form is null)
            AddForm(response.Value);
        else
            ReplaceForm(form, response.Value);
    }

    public async Task Rid(Guid? id, Guid? scopeId)
    {
        if (!InScope(scopeId))
            return;

        var form = Forms.FirstOrDefault(x => Matches(id, x));

        if (form is null)
            return;

        RemoveForm(form);

        await Task.CompletedTask;
    }

    public async Task Save(Guid? id)
    {
        var form = Forms.FirstOrDefault(x => Matches(id, x));

        if (form is null)
            return;

        if (form.IsNew)
        {
            var response = await form.WithWaiting("Adding...", () => Add(form));

            if (response.Ok)
                RemoveForm(form);
        }
        else
            await form.WithWaiting("Saving...", () => Save(form));
    }

    public async Task Cancel(Guid? id)
    {
        var form = Forms.FirstOrDefault(x => Matches(id, x));

        if (form is null)
            return;

        if (form.IsNew)
            RemoveForm(form);
        else
            await Replace(id);
    }

    public async Task Delete(Guid? id)
    {
        var form = Forms.FirstOrDefault(x => Matches(id, x));

        if (form is null)
            return;

        await form.ConfirmationModel.Hide();

        await form.WithWaiting("Deleting...", () => Remove(id));
    }

    // Required overrides

    public abstract Task Create();
    public abstract Task Refresh(Boolean resetAlerts = true);
    public abstract Task<Response<T?>> Fetch(Guid? id);
    public abstract Task<Response<T?>> Add(FormModel<T> form);
    public abstract Task<Response> Save(FormModel<T> form);
    public abstract Task<Response> Remove(Guid? id);

    // Optional overrides

    public virtual Boolean InScope(Guid? scopeId)
    {
        return true;
    }

    public virtual Boolean Matches(Guid? id, FormModel<T> form)
    {
        return form.Entity.Id.Equals(id);
    }

    public virtual String? OrderBy(FormModel<T> form)
    {
        return form.Entity.Name;
    }

    public virtual void ApplyFilter()
    {
        _updating = true;

        if (Filter.HasNothing())
            Forms.Apply(x => x.Hidden = false);
        else
            Forms.Apply(x => x.Hidden = !x.Entity.Name!.Contains(Filter, StringComparison.OrdinalIgnoreCase));

        _updating = false;

        RaisePropertyChanged(nameof(Forms));
    }

    public virtual void UpdateSort()
    {
        _updating = true;

        var index = 0;

        if (_ascending)
            foreach (var form in Forms.OrderBy(OrderBy))
                form.SortIndex = index++;
        else
            foreach (var form in Forms.OrderByDescending(OrderBy))
                form.SortIndex = index++;

        _updating = false;

        RaisePropertyChanged(nameof(Forms));
    }

    public void HandleFilterChanged(String? filter)
    {
        _filter = filter;
        ApplyFilter();
    }
}