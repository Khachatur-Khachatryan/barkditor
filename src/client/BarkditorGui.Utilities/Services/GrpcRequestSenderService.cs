using System.ComponentModel;
using Grpc.Core;
using Gtk;

namespace BarkditorGui.Utilities.Services;

public static class GrpcRequestSenderService
{
    [Description("Use only when you need to show dialog window on error")]
    public static TResponse? SendRequest<TResponse>(Func<TResponse> requestFunc) 
        where TResponse : class
    {
        try
        {
            var response = requestFunc.Invoke();
            return response;
        }
        catch (RpcException e)
        {
            var errorDialog = new MessageDialog(
                null, DialogFlags.Modal,
                MessageType.Error, ButtonsType.Cancel,
                e.Status.Detail);
            errorDialog.Run();
            errorDialog.Destroy();
            return null;
        }
    }
}