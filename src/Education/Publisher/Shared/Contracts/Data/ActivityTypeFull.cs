namespace Crudspa.Education.Publisher.Shared.Contracts.Data;

public class ActivityTypeFull : Observable, INamed
{
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

    public String? Key
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? DisplayView
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? CategoryId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? StatusId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ContextGuidance
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ChoiceGuidance
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ExtraGuidance
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? SupportsContextText
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Boolean? RequiresContextText
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Boolean? SupportsContextAudio
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Boolean? RequiresContextAudio
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Boolean? SupportsContextImage
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Boolean? RequiresContextImage
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Boolean? SupportsExtraText
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Boolean? RequiresExtraText
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Boolean? SupportsChoices
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Boolean? RequiresCorrectChoices
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Boolean? SupportsAudioChoices
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Boolean? RequiresAudioChoices
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Boolean? RequiresDataChoices
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Boolean? RequiresImageChoices
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Boolean? SupportsTextChoices
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Boolean? RequiresTextChoices
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Boolean? RequiresLongerTextChoices
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Boolean? RequiresColumnOrdinal
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Boolean? RequiresTextOrImageChoices
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Int32? MinChoices
    {
        get;
        set => SetProperty(ref field, value);
    } = 0;

    public Int32? MaxChoices
    {
        get;
        set => SetProperty(ref field, value);
    } = 0;

    public Int32? MinCorrectChoices
    {
        get;
        set => SetProperty(ref field, value);
    } = 0;

    public Int32? MaxCorrectChoices
    {
        get;
        set => SetProperty(ref field, value);
    } = 0;

    public Boolean? ShuffleChoices
    {
        get;
        set => SetProperty(ref field, value);
    } = true;

    public String? CategoryKey
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? CategoryName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? StatusName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? CombinedName
    {
        get;
        set => SetProperty(ref field, value);
    }
}