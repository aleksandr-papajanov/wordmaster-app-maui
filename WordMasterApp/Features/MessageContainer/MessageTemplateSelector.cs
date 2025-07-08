using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordMasterApp.Features.MessageContainer.Messages;

namespace WordMasterApp.Features.MessageContainer
{
    public class MessageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? ErrorTemplate { get; set; }
        public DataTemplate? NotificationTemplate { get; set; }
        public DataTemplate? WordBasicCompletionConfirmationTemplate { get; set; }
        public DataTemplate? WordUsagesCompletionConfirmationTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return item switch
            {
                ErrorMessage => ErrorTemplate!,
                ConfirmRelatedWordBasicCompletionMessage => WordBasicCompletionConfirmationTemplate!,
                ConfirmRelatedWordUsagesCompletionMessage => WordUsagesCompletionConfirmationTemplate!,
                NotificationMessage => NotificationTemplate!,
                _ => NotificationTemplate!
            };
        }
    }
}
