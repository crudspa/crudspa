namespace Crudspa.Education.Publisher.Shared.Contracts.Behavior;

public interface IClassRecordingService
{
    Task<Response<IList<ClassRecording>>> Search(Request<ClassRecordingSearch> request);
}