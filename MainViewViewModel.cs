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

namespace Module_5_6_2
{
    public class MainViewViewModel
    {
        private ExternalCommandData _commandData;

        public DelegateCommand SaveCommand { get; }
        public DelegateCommand GetPoin { get; }
        public List<FamilyType> FurnitureTypes { get; } = new List<FamilyType>();
        public List<Level> Levels { get; } = new List<Level>();
        public WallType SelectedFurniturelType { get; set; }
        public Level SelectedLevel { get; set; }
        public XYZ Point { get; set; }
        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            FurnitureTypes = FurnitureUtils.GetFurnitureType(commandData);
            Levels = LevelsUtils.GetLevels(commandData);
            GetPoin = new DelegateCommand(OnGetCommand);
            SaveCommand = new DelegateCommand(OnSaveCommand);
            Point = SelectionUtils.GetPoints(_commandData, "Выберите точки", ObjectSnapTypes.Endpoints);

        }

        private void OnGetCommand()
        {
            throw new NotImplementedException();
        }

        private void OnSaveCommand()
        {
            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            if (Points.Count < 2 ||
                SelectedWallType == null ||
                SelectedLevel == null)
                return;

            var curves = new List<Curve>();
            for (int i = 0; i < Points.Count; i++)
            {
                if (i == 0) continue;

                var prevPoint = Points[i - 1];
                var currentPoint = Points[i];

                Curve curve = Line.CreateBound(prevPoint, currentPoint);
                curves.Add(curve);
            }

            using (var ts = new Transaction(doc, "Creatte Wall"))
            {
                ts.Start();
                foreach (var curve in curves)
                {
                    Wall.Create(doc, curve, SelectedWallType.Id, SelectedLevel.Id,
                        UnitUtils.ConvertToInternalUnits(WallHeight, UnitTypeId.Millimeters),
                        0, false, false);
                }
                ts.Commit();
            }
            RaiseCloseRequest();
        }


        public event EventHandler CloseRequest;
        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }
    }
}
