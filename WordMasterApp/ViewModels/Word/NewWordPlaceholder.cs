using WordMaster.Data.DTOs;

namespace WordMasterApp.ViewModels.Word
{
    public class NewWordPlaceholder : IDisblayable
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Text { get; } = "New Word";
    }
}

