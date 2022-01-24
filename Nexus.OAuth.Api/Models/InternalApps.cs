namespace Nexus.OAuth.Api.Models;
public class InternalApps
{
    public int[] Development { get; set; }
    public int[] Release { get; set; }
    public int[] Local { get; set; }

    public bool IsInternalApp(int id)
    {
#if DEBUG 
        return Development.Contains(id);
#elif LOCAL
        return Local.Contains(id);
#else   
        return Release.Contains(id);
#endif
    }
}
