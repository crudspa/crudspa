namespace Crudspa.Education.Student.Server.Hubs;

public partial class StudentHub
{
    public async Task<Response<IList<AssessmentAssignment>>> AssessmentRunFetchAssessments(Request request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await AssessmentRunService.FetchAssessments(request));
    }

    public async Task<Response<Assessment?>> AssessmentRunFetchAssessment(Request<AssessmentAssignment> request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await AssessmentRunService.FetchAssessment(request));
    }

    public async Task<Response> AssessmentRunAddVocabChoiceSelection(Request<VocabChoiceSelection> request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await AssessmentRunService.AddVocabChoiceSelection(request));
    }

    public async Task<Response> AssessmentRunAddListenChoiceSelection(Request<ListenChoiceSelection> request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await AssessmentRunService.AddListenChoiceSelection(request));
    }

    public async Task<Response> AssessmentRunAddReadChoiceSelection(Request<ReadChoiceSelection> request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await AssessmentRunService.AddReadChoiceSelection(request));
    }

    public async Task<Response> AssessmentRunAddVocabPartCompleted(Request<VocabPartCompleted> request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await AssessmentRunService.AddVocabPartCompleted(request));
    }

    public async Task<Response> AssessmentRunAddListenPartCompleted(Request<ListenPartCompleted> request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await AssessmentRunService.AddListenPartCompleted(request));
    }

    public async Task<Response> AssessmentRunAddReadPartCompleted(Request<ReadPartCompleted> request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await AssessmentRunService.AddReadPartCompleted(request));
    }

    public async Task<Response> AssessmentRunAddAssessmentCompleted(Request<AssessmentAssignment> request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await AssessmentRunService.AddAssessmentCompleted(request));
    }
}