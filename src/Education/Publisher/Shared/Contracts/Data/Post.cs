namespace Crudspa.Education.Publisher.Shared.Contracts.Data;

public class Post : Observable, IValidates, ICountable, INamed
{
    public enum PostTypes { Success, InProcess, Stuck }

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Name
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ForumId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ParentId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? Pinned
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Body
    {
        get;
        set => SetProperty(ref field, value);
    }

    public AudioFile AudioFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public ImageFile ImageFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public PdfFile PdfFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public VideoFile VideoFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Guid? ById
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ByOrganizationName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? Posted
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? Edited
    {
        get;
        set => SetProperty(ref field, value);
    }

    public PostTypes Type
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? GradeId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? SubjectId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ByFirstName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ByLastName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? GradeName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? SubjectName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ReactionCharacter
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? CommentCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? TotalCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<Post> Children
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            //TEMP: The Innovator's Lab forum requires certain fields for top-level posts
            if (ParentId is null && ForumId.Equals(new Guid("ca7a86b5-00d3-487a-85a6-db3737af9bdf")))
            {
                if (GradeId is null)
                    errors.AddError("Grade is required.", nameof(GradeId));

                if (SubjectId is null)
                    errors.AddError("Subject is required.", nameof(SubjectId));
            }

            if (!Pinned.HasValue)
                errors.AddError("Pinned is required.", nameof(Pinned));

            if (Body.HasNothing())
                errors.AddError("Body is required.", nameof(Body));
        });
    }
}