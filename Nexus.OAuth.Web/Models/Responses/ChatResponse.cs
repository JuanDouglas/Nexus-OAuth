using Nexus.OAuth.Web.Models.Enums;

namespace Nexus.OAuth.Web.Models.Responses;

public class ChatResponse
{
    public int Status { get; set; }
    public object? Object { get; set; }
    public int NextStep { get; set; }
    public string PlaceHolder { get; set; }
    public string Type { get; set; }
    public ChatResponse(int status, RegisterStep nextStep, object? @object, string placeHolder, string type)
    {
        Status = status;
        Object = @object;
        NextStep = (int)nextStep;
        PlaceHolder = placeHolder;
        Type = type;
    }
}
