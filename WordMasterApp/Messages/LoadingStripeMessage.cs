using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordMasterApp.Components.LoadingStripe;

namespace WordMasterApp.Messages
{
    public class LoadingStripeMessage
    {
        public bool IsAnimating { get; }
        public LoadingStripeType Type { get; }
        
        public LoadingStripeMessage(bool isAnimating, LoadingStripeType type = LoadingStripeType.HttpRequest)
        {
            IsAnimating = isAnimating;
            Type = type;
        }
    }
}
