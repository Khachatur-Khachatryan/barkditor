namespace BarkditorServer.Domain.Constants;

public static class FilePaths
{
    public static readonly string AppPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../"));
    public static readonly string UnitTestsPath = Path.Combine(AppPath, "BarkditorServer.UnitTests");
    public static readonly string TestFolderPath = Path.Combine(UnitTestsPath, "TestFolder");
    public static readonly string BusinessLogicPath = Path.Combine(AppPath, "BarkditorServer.BusinessLogic");
    public static readonly string ProtobufProjectPath = Path.Combine(AppPath, "../shared/Barkditor.Protobuf");
    public const string ObjDebugPath = "obj/Debug/net7.0";
    public const string BinDebugPath = "bin/Debug/net7.0";
    public static readonly string ProjectFilesPathTxtPath = Path.Combine(BusinessLogicPath, BinDebugPath, "projectFilesPath.txt");
}