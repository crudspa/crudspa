namespace Crudspa.Framework.Core.Client.Components;

public partial class ContactEdit
{
    [Parameter, EditorRequired] public Contact? Contact { get; set; }
    [Parameter, EditorRequired] public User? User { get; set; }
    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }

    public BatchModel<ContactEmail> EmailModel { get; set; } = new();
    public BatchModel<ContactPhone> PhoneModel { get; set; } = new();
    public BatchModel<ContactPostal> PostalModel { get; set; } = new();

    protected override void OnParametersSet()
    {
        if (Contact is null) return;

        if (!ReferenceEquals(EmailModel.Entities, Contact.Emails))
            EmailModel.Entities = Contact.Emails;

        if (!ReferenceEquals(PhoneModel.Entities, Contact.Phones))
            PhoneModel.Entities = Contact.Phones;

        if (!ReferenceEquals(PostalModel.Entities, Contact.Postals))
            PostalModel.Entities = Contact.Postals;
    }

    public void AddEmail()
    {
        Contact?.Emails.Add(new()
        {
            Id = Guid.NewGuid(),
            ContactId = Contact.Id,
            Ordinal = Contact?.Emails.Count ?? 0,
        });
    }

    public void AddPhone()
    {
        Contact?.Phones.Add(new()
        {
            Id = Guid.NewGuid(),
            ContactId = Contact.Id,
            SupportsSms = true,
            Ordinal = Contact?.Phones.Count ?? 0,
        });
    }

    public void AddPostal()
    {
        Contact?.Postals.Add(new()
        {
            Id = Guid.NewGuid(),
            ContactId = Contact.Id,
            Ordinal = Contact?.Postals.Count ?? 0,
        });
    }
}