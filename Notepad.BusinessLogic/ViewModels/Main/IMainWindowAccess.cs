using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepad.BusinessLogic.ViewModels.Main
{
    public interface IMainWindowAccess
    {
        string GetSelectedText();

        int GetSelectedIndex();

        int GetSelectionLength();
    }
}
