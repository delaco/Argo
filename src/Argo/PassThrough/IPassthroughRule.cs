namespace Argo.PassThrough
{
    public interface IPassThroughRule
    {
        RequestContext RequestContext { get; }

        bool IsPassThrough { get; }
    }
}