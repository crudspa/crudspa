namespace Crudspa.Education.School.Shared.Contracts.Behavior;

public interface IClassRecordingService
{
    Task<Response<IList<ClassRecording>>> FetchAll(Request request);
    Task<Response<ClassRecording?>> Fetch(Request<ClassRecording> request);
    Task<Response<ClassRecording?>> Add(Request<ClassRecording> request);
    Task<Response> Save(Request<ClassRecording> request);
    Task<Response> Remove(Request<ClassRecording> request);
    Task<Response<IList<Orderable>>> FetchContentCategoryNames(Request request);
}