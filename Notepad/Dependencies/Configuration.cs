using Notepad.BusinessLogic.Services.Messaging;
using Notepad.Services.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notepad.BusinessLogic.Services.Dialogs;
using Notepad.Services.DialogService;
using Unity;
using Unity.Lifetime;

namespace Notepad.Dependencies
{
    public static class Configuration
    {
        private static bool configured = false;

        public static void Configure(IUnityContainer container)
        {
            if (configured)
                return;

            container.RegisterType<IMessagingService, MessagingService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IDialogService, DialogService>(new ContainerControlledLifetimeManager());

            Notepad.BusinessLogic.Dependencies.Configuration.Configure(container);

            configured = true;
        }
    }
}
