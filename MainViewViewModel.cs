using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI.Selection;
using RevitAPITrainingLibrary;
using Autodesk.Revit.DB.Mechanical;

namespace Module_5_6_2
{
    public class MainViewViewModel
    {
        private ExternalCommandData _commandData;

        public DelegateCommand SaveCommand { get; }
        public DelegateCommand GetPoint { get; }
        public List<FamilySymbol> FurnitureTypes { get; } = new List<FamilySymbol>();
        public List<Level> Levels { get; } = new List<Level>();
        public FamilySymbol SelectedFurniturelType { get; set; }
        public Level SelectedLevel { get; set; }
        public XYZ Point { get; set; }
        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            FurnitureTypes = FurnitureUtils.GetFurnitureType(commandData);
            Levels = LevelsUtils.GetLevels(commandData);
            GetPoint = new DelegateCommand(OnGetCommand);
            SaveCommand = new DelegateCommand(OnSaveCommand);
        }

        private void OnGetCommand()
        {
            RaiseHideRequest();
            Point = SelectionUtils.GetPoint(_commandData, "Выберите точки");
            
            RaiseShowRequest();
        }

        private void OnSaveCommand()
        {
            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            using (var ts = new Transaction(doc, "Create"))
            {
                ts.Start();
                //doc.Create.NewFamilyInstance(Point, SelectedFurniturelType, SelectedLevel, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                ts.Commit();
            }
            RaiseCloseRequest();

        }

        public event EventHandler HideRequest; //Эвент скрытия окна
        private void RaiseHideRequest()
        {
            HideRequest?.Invoke(this, EventArgs.Empty); //Если есть методы то буду запускать те методы которые привязаны к HideRequestRequest
        }

        public event EventHandler ShowRequest; //Эвент показа окна
        private void RaiseShowRequest()
        {
            ShowRequest?.Invoke(this, EventArgs.Empty); //Если есть методы то буду запускать те методы которые привязаны к ShowRequest
        }
        public event EventHandler CloseRequest;
        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }
    }
}
