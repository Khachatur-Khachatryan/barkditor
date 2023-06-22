using Barkditor.Protobuf;
using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using BarkditorServer.Domain.Constants;

#pragma warning disable CS1591

namespace BarkditorServer.BusinessLogic.Services;

public class ProjectFilesService : ProjectFiles.ProjectFilesBase
{
    public override async Task<Empty> SetProjectPath(SetProjectPathRequest request, ServerCallContext ctx)
    {
        await File.WriteAllTextAsync(FilePaths.ProjectPathTxtPath, 
            request.Path);

        return new Empty();
    }

    public override async Task<GetProjectPathResponse> GetProjectPath(Empty empty, ServerCallContext ctx)
    {
        var path = await File.ReadAllTextAsync(FilePaths.ProjectPathTxtPath);
        var response = new GetProjectPathResponse
        {
            Path = path
        };

        return response;
    }
}
