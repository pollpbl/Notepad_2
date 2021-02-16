using Notepad.BusinessLogic.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Notepad.BusinessLogic.Services.Dialogs;
using Notepad.BusinessLogic.Services.Messaging;
using Notepad.Common;
using Notepad.Resources;
using Spooksoft.VisualStateManager.Conditions;

namespace Notepad.BusinessLogic.ViewModels.Main
{
    public class MainWindowViewModel : BaseViewModel
    {
        // Private fields -----------------------------------------------------

        public enum OperationType
        {
            CreateNewFile,
            OpenFile,
            SaveFile
        }

        private readonly IMainWindowAccess access;

        private readonly IDialogService dialogService;

        private readonly IMessagingService messagingService;

        private string documentFilename;

        private string documentVirtual;

        private string documentText;

        private bool documentChanged;

        private bool isSelected;

        private string copiedText;

        private int counter = 0;

        private bool copiedTextExist;

        // Public methods -----------------------------------------------------

        public void DoOpenFile()
        {
            if (DocumentChanged)
            {
                SaveDocumentIfUserWants(OperationType.OpenFile);
            }
            else
            {
                var dialogResult = dialogService.ShowOpenDialog(Strings.DefaultFilter);
                if (dialogResult.Result)
                {
                    if (File.Exists(dialogResult.FileName))
                    {
                        DocumentFilename = dialogResult.FileName;
                        DocumentText = File.ReadAllText(DocumentFilename);
                        DocumentChanged = false;
                    }
                }
            }
        }

        private void SaveDocumentIfUserWants(OperationType operationType)
        {
            var result = messagingService.AskYesNoCancel(Strings.SaveFileMessageQuestion);
            if (result == true)
            {
                if (!string.IsNullOrEmpty(DocumentVirtual))
                {
                    File.WriteAllText(DocumentVirtual, DocumentText);
                    DocumentFilename = string.Empty;
                    DocumentVirtual = string.Empty;
                    DocumentText = string.Empty;
                    DocumentChanged = false;
                }
            }
            else if (result == false)
            {
                DocumentText = string.Empty;
                DocumentChanged = false;
            }
            else
                return;

            switch (operationType)
            {
                case OperationType.CreateNewFile:
                    DoCreateNewFile();
                    break;
                case OperationType.OpenFile:
                    DoOpenFile();
                    break;
            }
        }

        public void DoSave()
        {
            if (!File.Exists(DocumentFilename))
                File.WriteAllText(DocumentFilename, DocumentText);
        }

        public void DoCreateNewFile()
        {
            if (DocumentChanged)
            {
                SaveDocumentIfUserWants(OperationType.CreateNewFile);
            }
            else
            {
                var dialogResult = dialogService.ShowSaveDialog(Strings.DefaultFilter, null, GenerateRandomFileName());
                if (dialogResult.Result)
                {
                    DocumentVirtual = dialogResult.FileName;
                }
            }
        }

        public void DoCopy()
        {
            CopiedText = access.GetSelectedText();
        }

        public void DoPaste()
        {
            DocumentText = DocumentText.Insert(access.GetSelectedIndex(), CopiedText);
        }

        public void DoCut()
        {
            var selectionIndex = access.GetSelectedIndex();
            var selectionLength = access.GetSelectionLength();
            CopiedText = access.GetSelectedText();
            DocumentText = DocumentText.Remove(selectionIndex, selectionLength);
        }

        private void HandlerTextCopiedChanged()
        {
            CopiedTextExist = !string.IsNullOrEmpty(CopiedText);
        }

        private string GenerateRandomFileName()
        {
            counter++;
            return $"New File {counter}.txt";
        }

        private readonly BaseCondition canBeSavedCondition;
        private readonly BaseCondition textSelectedCondition;
        private readonly BaseCondition textCanBePastedCondition;
        private readonly BaseCondition isVirtualBaseCondition;

        public MainWindowViewModel(IMainWindowAccess access, IDialogService dialogService,
            IMessagingService messagingService)
        {
            this.access = access;
            this.dialogService = dialogService;
            this.messagingService = messagingService;

            canBeSavedCondition = new PropertyWatchCondition<MainWindowViewModel>(this, vm => DocumentChanged, false);
            textSelectedCondition = new LambdaCondition<MainWindowViewModel>(this, vm => vm.IsSelected, false);
            isVirtualBaseCondition = new LambdaCondition<MainWindowViewModel>(this, vm => vm.IsVirtualFile, false);
            textCanBePastedCondition = new LambdaCondition<MainWindowViewModel>(this, vm => vm.CopiedTextExist, false);

            DoCreateNewFileCommand = new Spooksoft.VisualStateManager.Commands.AppCommand(obj => DoCreateNewFile());

            DoLoadFileCommand = new Spooksoft.VisualStateManager.Commands.AppCommand(obj => DoOpenFile());
            DoSaveFileCommand = new Spooksoft.VisualStateManager.Commands.AppCommand(obj => DoSave(), !isVirtualBaseCondition & canBeSavedCondition);
            DoCopyCommand = new Spooksoft.VisualStateManager.Commands.AppCommand(obj => DoCopy(), textSelectedCondition);
            DoPasteCommand = new Spooksoft.VisualStateManager.Commands.AppCommand(obj => DoPaste(), textCanBePastedCondition);
            DoCutCommand = new Spooksoft.VisualStateManager.Commands.AppCommand(obj => DoCut(), textSelectedCondition);
        }

        // Public properties --------------------------------------------------

        public ICommand DoLoadFileCommand { get; }
        public ICommand DoSaveFileCommand { get; }
        public ICommand DoCreateNewFileCommand { get; }
        public ICommand DoCopyCommand { get; }
        public ICommand DoPasteCommand { get; }
        public ICommand DoCutCommand { get; }

        public bool CopiedTextExist
        {
            get => copiedTextExist;
            set => Set(ref copiedTextExist, value);
        }

        public string CopiedText
        {
            get => copiedText;
            set => Set(ref copiedText, value, nameof(CopiedText), HandlerTextCopiedChanged);
        }

        public string DocumentFilename
        {
            get => documentFilename;
            set => Set(ref documentFilename, value);
        }

        public string DocumentVirtual
        {
            get => documentVirtual;
            set => Set(ref documentVirtual, value);
        }

        public string DocumentText
        {
            get => documentText;
            set => Set(ref documentText, value, nameof(DocumentText), HandleDocumentChanged);
        }

        private void HandleDocumentChanged()
        {
            DocumentChanged = true;
            DocumentFilename = string.Empty; //the file was changed and we don't want to keep the value still
        }

        public bool DocumentChanged
        {
            get => documentChanged;
            set => Set(ref documentChanged, value);
        }

        public bool IsSelected
        {
            get => isSelected;
            set => Set(ref isSelected, value);
        }

        public bool IsVirtualFile
        {
            get => string.IsNullOrEmpty(DocumentVirtual);
        }
    }
}