namespace Aster.Common.Data.Core.Predicates
{
    public interface ISort
    {
        string PropertyName { get; set; }
        bool Ascending { get; set; }
    }
}