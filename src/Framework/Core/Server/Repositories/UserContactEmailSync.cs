namespace Crudspa.Framework.Core.Server.Repositories;

public static class UserContactEmailSync
{
    public static void Apply(Contact contact, User user, Contact? existingContact = null, User? existingUser = null)
    {
        var email = user.Username;

        if (email.HasNothing() || !email.IsEmailAddress())
            return;

        existingContact ??= contact;

        var existingEmail = existingUser?.Username;

        Ensure(contact, email);

        var existingMatch = existingEmail.HasSomething() && existingEmail.IsEmailAddress()
            ? existingContact.Emails.SingleOrDefault(x => x.Email.IsBasically(existingEmail))
            : null;

        var replace = existingMatch is not null && !existingEmail!.IsBasically(email);

        if (replace)
            Replace(contact, email, existingEmail!, existingMatch!);
    }

    private static void Ensure(Contact contact, String email)
    {
        if (contact.Emails.Any(x => x.Email.IsBasically(email)))
            return;

        var nextOrdinal = contact.Emails
            .Select(x => x.Ordinal ?? -1)
            .DefaultIfEmpty(-1)
            .Max() + 1;

        contact.Emails.Add(new()
        {
            ContactId = contact.Id,
            Email = email,
            Ordinal = nextOrdinal,
        });
    }

    private static void Replace(Contact contact, String email, String existingEmail, ContactEmail existingMatch)
    {
        var existingId = existingMatch.Id;

        var newEmail = contact.Emails.SingleOrDefault(x => x.Email.IsBasically(email));
        var oldEmail = contact.Emails.SingleOrDefault(x => x.Email.IsBasically(existingEmail));

        if (newEmail is not null)
        {
            newEmail.Id = existingId;

            if (oldEmail is not null && !ReferenceEquals(oldEmail, newEmail))
                contact.Emails.Remove(oldEmail);
        }
        else if (oldEmail is not null)
        {
            oldEmail.Id = existingId;
            oldEmail.Email = email;
        }
        else
            contact.Emails.Add(new() { Id = existingId, ContactId = contact.Id, Email = email });
    }
}