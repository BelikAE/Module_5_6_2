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
        public DelegateCommand GetPoints { get; }
        public List<FamilySymbol> FurnitureTypes { get; } = new List<FamilySymbol>();
        public List<Level> Levels { get; } = new List<Level>();
        public FamilySymbol SelectedFurnitureType { get; set; }
        public Level SelectedLevel { get; set; }
        public List<XYZ> Points { get; set; } = new List<XYZ>();

        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            FurnitureTypes = FurnitureUtils.GetFurnitureType(commandData);
            Levels = LevelsUtils.GetLevels(commandData);
            GetPoints = new DelegateCommand(OnGetPoints);
            SaveCommand = new DelegateCommand(OnSaveCommand);
            
        }

        private void OnGetPoints()
        {
            RaiseHideRequest();
            Points = SelectionUtils.GetPoints(_commandData, "Выберите точки", ObjectSnapTypes.Endpoints);
            RaiseShowRequest();

        }

        private void OnSaveCommand()
        {
            if (Points.Count == 0)
            {
                TaskDialog.Show("Ошибка","Не выбраны точки вставки");
                return;
            }
            foreach(var point in Points)
            {
                FamilyInstanceUtils.CreateFamilyInstance(_commandData, SelectedFurnitureType, point, SelectedLevel);
            }

            RaiseCloseRequest();

        }

        public event EventHandler HideRequest;
        private void RaiseHideRequest()
        {
            HideRequest?.Invoke(this, EventArgs.Empty); 
        }

        public event EventHandler ShowRequest; 
        private void RaiseShowRequest()
        {
            ShowRequest?.Invoke(this, EventArgs.Empty); 
        }
        public event EventHandler CloseRequest;
        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }
    }
}
