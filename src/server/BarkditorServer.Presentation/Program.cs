using BarkditorServer.BusinessLogic.Services;
using BarkditorServer.BusinessLogic.Wrappers;
using BarkditorServer.Domain.Constants;
using Microsoft.OpenApi.Models;

namespace BarkditorServer.Presentation;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddGrpc()
            .AddServiceOptions<ProjectFilesService>(options =>
            {
                options.MaxReceiveMessageSize = null;
                options.MaxSendMessageSize = null;
            });
        builder.Services.AddGrpcSwagger();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1",
                new OpenApiInfo { Title = "Barkditor API", Version = "v1" });
            
            var filePath = Path.Combine(FilePaths.ProtobufProjectPath, FilePaths.ObjDebugPath, "documentation.xml");
            c.IncludeXmlComments(filePath);
            c.IncludeGrpcXmlComments(filePath, includeControllerXmlComments: true);
        });

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Barkditor API V1");
        });
        app.MapGrpcService<ProjectFilesService>();
        app.MapGrpcService<FilesService>();
        app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

        app.Run();
        DirectoryWrapper.Create(FilePaths.TempFolderPath);
        DirectoryWrapper.Create(FilePaths.TempCopiedFilesFolderPath);
    }
}