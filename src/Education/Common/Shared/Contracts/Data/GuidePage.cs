namespace Crudspa.Education.Common.Shared.Contracts.Data;

public class GuidePage : Observable, IUnique
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? GuideBinderId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? PageId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? ShowGuide
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public String? GuideText
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? GuideAudioId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public AudioFile GuideAudioFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public AudioFile GuideAudio
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Boolean? ShowNotebook
    {
        get;
        set => SetProperty(ref field, value);
    } = false;
}