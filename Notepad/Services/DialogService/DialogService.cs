using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using Notepad.BusinessLogic.Models;
using Notepad.BusinessLogic.Services.Dialogs;
using Notepad.Resources;

namespace Notepad.Services.DialogService
{
    public class DialogService : IDialogService
    {
        public OpenDialogResult ShowOpenDialog(string filter = null, string title = null, string filename = null)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (filename != null)
                dialog.FileName = filename;

            if (filter != null)
                dialog.Filter = filter;
            else
                dialog.Filter = Strings.DefaultFilter;

            if (title != null)
                dialog.Title = title;
            else
                dialog.Title = Strings.DefaultDialogTitle;

            if (dialog.ShowDialog() == true)
                return new OpenDialogResult(true, dialog.FileName);
            else
                return new OpenDialogResult(false, null);
        }

        public SaveDialogResult ShowSaveDialog(string filter = null, string title = null, string filename = null)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            if (filename != null)
                dialog.FileName = filename;

            if (filter != null)
                dialog.Filter = filter;
            else
                dialog.Filter = Strings.DefaultFilter;

            if (title != null)
                dialog.Title = title;
            else
                dialog.Title = Strings.DefaultDialogTitle;

            if (dialog.ShowDialog() == true)
                return new SaveDialogResult(true, dialog.FileName);
            else
                return new SaveDialogResult(false, null);
        }
    }
}