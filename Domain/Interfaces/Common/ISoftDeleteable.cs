
namespace Domain.Interfaces.Common
{
    public interface ISoftDeleteable
    {
        bool IsDelete { get; set; }
        DateTime? DateDeleted { get; set; }

        void Delete()
        {
            IsDelete = true;
            DateDeleted = DateTime.UtcNow;
        }
        void UndoDelete()
        {
            IsDelete = false;
            DateDeleted = null;
        }
    }
}
