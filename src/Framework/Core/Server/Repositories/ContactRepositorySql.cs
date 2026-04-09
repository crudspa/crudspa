namespace Crudspa.Framework.Core.Server.Repositories;

public class ContactRepositorySql(ISessionService sessionService) : IContactRepository
{
    public async Task<IList<Contact>> SelectByIds(String connection, IEnumerable<Guid?> contactIds, Guid? portalId = null)
    {
        var contacts = await ContactSelectByIds.Execute(connection, contactIds);

        return contacts;
    }

    public async Task<Contact?> Select(String connection, Guid? contactId, Guid? portalId = null)
    {
        var contact = await ContactSelect.Execute(connection, contactId);

        return contact;
    }

    public async Task<Guid?> Insert(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Contact contact, Guid? portalId = null)
    {
        var id = await ContactInsert.Execute(connection, transaction, sessionId, contact);

        contact.Emails.Apply(x => x.ContactId = id);
        contact.Phones.Apply(x => x.ContactId = id);
        contact.Postals.Apply(x => x.ContactId = id);

        await SaveAddresses(connection, transaction, sessionId, contact, null);
        await sessionService.InvalidateAll();

        return id;
    }

    public async Task Update(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Contact contact, Guid? portalId = null)
    {
        var existing = await ContactSelect.Execute(connection, transaction, contact.Id);
        await ContactUpdate.Execute(connection, transaction, sessionId, contact);
        await SaveAddresses(connection, transaction, sessionId, contact, existing);
        await sessionService.InvalidateAll();
    }

    public async Task Delete(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Contact contact)
    {
        if (contact.Id is null)
            return;

        await ContactDelete.Execute(connection, transaction, sessionId, contact);
        await sessionService.InvalidateAll();
    }

    public async Task<List<Error>> Validate(String connection, Contact contact, Guid? portalId = null)
    {
        return await ErrorsEx.Validate(async errors =>
        {
            var ordinal = 1;

            foreach (var contactEmail in contact.Emails)
            {
                var emailErrors = contactEmail.Validate();
                emailErrors.PrependMessages($"(Email #{ordinal}) ");
                errors.AddRange(emailErrors);

                ordinal++;
            }

            ordinal = 1;

            foreach (var contactPhone in contact.Phones)
            {
                var phoneErrors = contactPhone.Validate();
                phoneErrors.PrependMessages($"(Phone #{ordinal}) ");
                errors.AddRange(phoneErrors);

                ordinal++;
            }

            ordinal = 1;

            foreach (var contactPostal in contact.Postals)
            {
                var postalErrors = contactPostal.Validate();
                postalErrors.PrependMessages($"(Postal #{ordinal}) ");
                errors.AddRange(postalErrors);

                ordinal++;
            }

            await Task.CompletedTask;
        });
    }

    private static async Task SaveAddresses(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Contact incoming, Contact? existing)
    {
        incoming.Emails.EnsureOrder();
        incoming.Phones.EnsureOrder();
        incoming.Postals.EnsureOrder();

        await SqlWrappersCore.MergeBatch(connection, transaction, sessionId,
            existing?.Emails ?? [],
            incoming.Emails,
            ContactEmailInsert.Execute,
            ContactEmailUpdate.Execute,
            ContactEmailDelete.Execute);

        await SqlWrappersCore.MergeBatch(connection, transaction, sessionId,
            existing?.Phones ?? [],
            incoming.Phones,
            ContactPhoneInsert.Execute,
            ContactPhoneUpdate.Execute,
            ContactPhoneDelete.Execute);

        await SqlWrappersCore.MergeBatch(connection, transaction, sessionId,
            existing?.Postals ?? [],
            incoming.Postals,
            ContactPostalInsert.Execute,
            ContactPostalUpdate.Execute,
            ContactPostalDelete.Execute);
    }
}