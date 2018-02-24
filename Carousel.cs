using GrapeCity.Forguncy.CellTypes;
using GrapeCity.Forguncy.Commands;
using GrapeCity.Forguncy.Plugin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace CarouselCellType
{
    [Icon("pack://application:,,,/CarouselCellType;component/Resources/Icon.png")]
    [SupportUsingScope(PageScope.AllPage, ListViewScope.None)]
    public class Carousel : CellType, IReferenceCommand, IReferenceTable, INeedUploadFileByUser, IReferenceFormula, IDependenceCells
    {
        public Carousel()
        {
            AutoSlide = true;
            Interval = 5;
            Wrap = true;
            PauseWhenHover = true;
            ShowCaptions = true;
            ShowLeftRightControls = true;
            ShowIndicators = true;

            ImageInfos = new List<ImageInfo>();
            ImageInfos.Add(new ImageInfo() { Caption = string.Format(Resource.CarouselCellType_DefaultImageName, 1) });
            ImageInfos.Add(new ImageInfo() { Caption = string.Format(Resource.CarouselCellType_DefaultImageName, 2) });
            ImageInfos.Add(new ImageInfo() { Caption = string.Format(Resource.CarouselCellType_DefaultImageName, 3) });
        }

        [ResourcesDisplayName("CarouselCellType_IsBinding")]
        [OrderWeight(0)]
        public bool IsBinding
        {
            get; set;
        }

        [ResourcesDisplayName("CarouselCellType_EditImageInfos")]
        [OrderWeight(1)]
        public BindingImageInfo BindingImageInfo
        {
            get; set;
        }

        [ResourcesDisplayName("CarouselCellType_EditImageInfos")]
        [OrderWeight(1)]
        public List<ImageInfo> ImageInfos
        {
            get; set;
        }

        [ResourcesCategoryHeader("CarouselCellType_Options")]
        [ResourcesDisplayName("CarouselCellType_ShowLeftRightControls")]
        [OrderWeight(2)]
        [DefaultValue(true)]
        public bool ShowLeftRightControls
        {
            get; set;
        }
        
        [ResourcesDisplayName("CarouselCellType_ShowIndicators")]
        [OrderWeight(3)]
        [DefaultValue(true)]
        public bool ShowIndicators
        {
            get; set;
        }
        
        [ResourcesDisplayName("CarouselCellType_ShowDescriptionText")]
        [OrderWeight(4)]
        [DefaultValue(true)]
        public bool ShowCaptions
        {
            get; set;
        }
        
        [ResourcesDisplayName("CarouselCellType_Wrap")]
        [ResourcesDescription("CarouselCellType_WrapDescription")]
        [DefaultValue(true)]
        [OrderWeight(5)]
        public bool Wrap
        {
            get; set;
        }

        [ResourcesCategoryHeader("CarouselCellType_AutoSlide")]
        [ResourcesDisplayName("CarouselCellType_AutoSlide")]
        [DefaultValue(true)]
        [OrderWeight(6)]
        public bool AutoSlide
        {
            get; set;
        }

        [ResourcesDisplayName("CarouselCellType_Interval")]
        [ResourcesDescription("CarouselCellType_IntervalDescription")]
        [DefaultValue(5)]
        [OrderWeight(7)]
        public double Interval
        {
            get; set;
        }

        [ResourcesDisplayName("CarouselCellType_PauseWhenHover")]
        [ResourcesDescription("CarouselCellType_PauseWhenHoverDescription")]
        [DefaultValue(true)]
        [OrderWeight(8)]
        public bool PauseWhenHover
        {
            get;set;
        }
        
        public override string ToString()
        {
            return Resource.CarouselCellType_DisplayName;
        }
        public override EditorSetting GetEditorSetting(PropertyDescriptor property, IBuilderContext builderContext)
        {
            if (property.Name == "ImageInfos")
            {
                return new HyperlinkEditorSetting(new ShowEditImageHyperlinkCommand(builderContext));
            }

            if (property.Name == "BindingImageInfo")
            {
                return new HyperlinkEditorSetting(new ShowEditBindingImageHyperlinkCommand(builderContext));
            }
            
            return base.GetEditorSetting(property, builderContext);
        }

        public override bool GetDesignerPropertyVisible(string propertyName)
        {
            if (propertyName == "Interval" || propertyName == "PauseWhenHover")
            {
                if (AutoSlide)
                {
                    return true;
                }
                return false;
            }
            if (propertyName == "ImageInfos")
            {
                if (IsBinding)
                {
                    return false;
                }
                return true;
            }
            if (propertyName == "BindingImageInfo")
            {
                if (IsBinding)
                {
                    return true;
                }
                return false;
            }
            
            return base.GetDesignerPropertyVisible(propertyName);
        }

        public IEnumerable<LocatedObject<List<Command>>> GetCommandList(LocationIndicator location)
        {
            foreach (var item in ImageInfos)
            {
                if (item.CommandList != null)
                {
                    yield return new LocatedObject<List<Command>>(item.CommandList, location
                        .AppendProperty(Resource.CarouselCellType_EditImageInfos)
                        .AppendProperty(item.Caption)
                        .AppendProperty(Resource.CarouselCellType_EditCommands));
                }
            }
        }

        public IEnumerable<LocatedObject<TableCheckInfo>> GetTableInfo(LocationIndicator location)
        {
            if (IsBinding && BindingImageInfo != null)
            {
                return BindingImageInfo.GetTableInfo(location);
            }

            return Enumerable.Empty<LocatedObject<TableCheckInfo>>();
        }

        public void RenameTableColumnName(string tableName, string oldName, string newName)
        {
            if (BindingImageInfo != null)
            {
                BindingImageInfo.RenameTableColumnName(tableName, oldName, newName);
            }
        }

        public void RenameTableName(string oldName, string newName)
        {
            if (BindingImageInfo != null)
            {
                BindingImageInfo.RenameTableName(oldName, newName);
            }
        }

        public List<FileCopyInfo> GetAllFileSourceAndTargetPathsWhenImportForguncyFile(IFileUploadContext context)
        {
            var list = new List<FileCopyInfo>();
            if (ImageInfos != null)
            {
                foreach (var imageInfo in ImageInfos)
                {
                    if (string.IsNullOrEmpty(imageInfo.ImagePath))
                    {
                        continue;
                    }

                    list.Add(new FileCopyInfo()
                    {
                        SourceFileFolder = Path.Combine(context.GetForguncyUploadFilesRelativePath(context.SourceDocumentFolder), "CarouselCellType", "Images"),
                        SourceFileName = imageInfo.ImagePath,
                        TargetFileFolder = Path.Combine(context.GetForguncyUploadFilesFolderLocalPath(), "CarouselCellType", "Images"),
                        TargetFileName = imageInfo.ImagePath
                    });
                }
            }

            return list;
        }

        public FileUploadInfo GetUploadFileInfosWhenSaveFile(IFileUploadContext context)
        {
            var result = new FileUploadInfo()
            {
                UploadFileFolder = Path.Combine(context.GetForguncyUploadFilesFolderLocalPath(), "CarouselCellType", "Images")
            };

            if (ImageInfos != null)
            {
                foreach (var imageInfo in ImageInfos)
                {
                    if (string.IsNullOrEmpty(imageInfo.ImagePath))
                    {
                        continue;
                    }

                    result.AllUsedUploadFilePaths.Add(imageInfo.ImagePath);
                }
            }

            return result;
        }

        public override FrameworkElement GetDrawingControl(ICellInfo cellInfo, IDrawingHelper drawingHelper)
        {
            Grid grid = new Grid();
            Image image = new Image();
            image.Source = new BitmapImage(new Uri("pack://application:,,,/CarouselCellType;component/Resources/Preview.png", UriKind.RelativeOrAbsolute));
            image.Stretch = Stretch.Uniform;
            image.VerticalAlignment = VerticalAlignment.Center;
            image.HorizontalAlignment = HorizontalAlignment.Center;

            grid.Children.Add(image);

            return grid;
        }

        public IEnumerable<LocatedObject<object>> GetFormulaReferObjects(LocationIndicator location)
        {
            if (IsBinding && BindingImageInfo != null)
            {
                return BindingImageInfo.GetFormulaReferObjects(location);
            }

            return Enumerable.Empty<LocatedObject<object>>();
        }

        public IEnumerable<object> EnumDependenceCells(IBuilderContext context)
        {
            if (IsBinding && BindingImageInfo != null)
            {
                if (BindingImageInfo.QueryCondition != null)
                {
                    return context.EnumDependenceCells(BindingImageInfo.QueryCondition) ?? Enumerable.Empty<object>();
                }
            }

            return Enumerable.Empty<object>();
        }
    }

    public class ShowEditImageHyperlinkCommand : ICommand
    {
        private IBuilderContext _builderContext;
        public ShowEditImageHyperlinkCommand(IBuilderContext builderContext)
        {
            this._builderContext = builderContext;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var dataContext = parameter as IEditorSettingsDataContext;

            var window = new MyWindow();
            window.Title = Resource.CarouselCellType_ImageInfoSettings;
            
            var control = new ImagesEditor(this._builderContext);
            control.ViewModel.Model = dataContext?.Value as List<ImageInfo>;

            window.DialogControl = control;
            window.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
            
            window.Closed += (sender, e) =>
            {
                if (window.DialogResult == true)
                {
                    dataContext.Value = control.ViewModel.Model;
                }
            };

            window.ShowDialog();
        }
    }

    public class ShowEditBindingImageHyperlinkCommand : ICommand
    {
        private IBuilderContext _builderContext;
        public ShowEditBindingImageHyperlinkCommand(IBuilderContext builderContext)
        {
            this._builderContext = builderContext;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var dataContext = parameter as IEditorSettingsDataContext;

            var window = new MyWindow();
            window.Title = Resource.CarouselCellType_ImageInfoSettings;

            var control = new BindingImagesEditor(this._builderContext);
            control.ViewModel.Model = dataContext?.Value as BindingImageInfo;

            window.DialogControl = control;
            window.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;

            window.Closed += (sender, e) =>
            {
                if (window.DialogResult == true)
                {
                    dataContext.Value = control.ViewModel.Model;
                }
            };

            window.ShowDialog();
        }
    }


    public class ImageInfo: ICloneable
    {
        public List<Command> CommandList
        {
            get; set;
        }

        public string ImagePath
        {
            get; set;
        }

        public string Caption
        {
            get; set;
        }
        
        public string Description
        {
            get; set;
        }

        public object Clone()
        {
            var newObj = new ImageInfo()
            {
                Caption = Caption,
                Description = Description,
                ImagePath = ImagePath
            };

            if (CommandList != null)
            {
                newObj.CommandList = new List<Command>();
                foreach (var command in CommandList)
                {
                    newObj.CommandList.Add(command.Clone() as Command);
                }
            }

            return newObj;
        }
    }

    public class BindingImageInfo : ICloneable, IReferenceTable, IReferenceFormula
    {
        public string TableName
        {
            get; set;
        }

        public string ImageColumn
        {
            get; set;
        }

        public string CaptionColumn
        {
            get; set;
        }

        public string DescriptionColumn
        {
            get; set;
        }

        public object QueryCondition
        {
            get; set;
        }

        public object Clone()
        {
            var obj = this.MemberwiseClone() as BindingImageInfo;
            if (this.QueryCondition is ICloneable)
            {
                obj.QueryCondition = (this.QueryCondition as ICloneable).Clone();
            }
            else
            {
                obj.QueryCondition = this.QueryCondition;
            }

            return obj;
        }

        public IEnumerable<LocatedObject<TableCheckInfo>> GetTableInfo(LocationIndicator location)
        {
            var list = new List<LocatedObject<TableCheckInfo>>();

            var info = new TableCheckInfo(TableName);
            info.AddColumns(new string[] { ImageColumn, CaptionColumn, DescriptionColumn});

            list.Add(new LocatedObject<TableCheckInfo>(info, location.AppendProperty(Resource.CarouselCellType_DisplayName).AppendProperty(Resource.CarouselCellType_EditImageInfos)));

            if (this.QueryCondition is IReferenceTable)
            {
                list.AddRange((this.QueryCondition as IReferenceTable).GetTableInfo(location));
            }

            return list;
        }

        public void RenameTableColumnName(string tableName, string oldName, string newName)
        {
            if (this.TableName != null && string.Equals(this.TableName, tableName))
            {
                if (oldName == ImageColumn)
                {
                    ImageColumn = newName;
                }

                if (oldName == CaptionColumn)
                {
                    CaptionColumn = newName;
                }

                if (oldName == DescriptionColumn)
                {
                    DescriptionColumn = newName;
                }
            }

            if (this.QueryCondition is IReferenceTable)
            {
                (this.QueryCondition as IReferenceTable).RenameTableColumnName(tableName, oldName, newName);
            }
        }

        public void RenameTableName(string oldName, string newName)
        {
            if (this.TableName != null && string.Equals(this.TableName, oldName))
            {
                this.TableName = newName;
            }

            if (this.QueryCondition is IReferenceTable)
            {
                (this.QueryCondition as IReferenceTable).RenameTableName(oldName, newName);
            }
        }

        public IEnumerable<LocatedObject<object>> GetFormulaReferObjects(LocationIndicator location)
        {
            if (this.QueryCondition is IReferenceFormula)
            {
                foreach (var item in (this.QueryCondition as IReferenceFormula).GetFormulaReferObjects(location))
                {
                    yield return item;
                }
            }
        }
    }

    public class ResourcesDisplayNameAttribute : DisplayNameAttribute
    {
        public ResourcesDisplayNameAttribute(string displayName)
            : base(displayName)
        {

        }
        public override string DisplayName
        {
            get
            {
                return Resource.ResourceManager.GetString(base.DisplayName);
            }
        }
    }

    public class ResourcesDescriptionAttribute : DescriptionAttribute
    {
        public ResourcesDescriptionAttribute(string description)
            : base(description)
        {

        }

        public override string Description
        {
            get
            {
                return Resource.ResourceManager.GetString(base.Description);
            }
        }
    }

    public class ResourcesCategoryHeaderAttribute : CategoryHeaderAttribute
    {
        public ResourcesCategoryHeaderAttribute(string header)
            : base(header)
        {

        }

        public override string CategoryHeader
        {
            get
            {
                return Resource.ResourceManager.GetString(base.CategoryHeader);
            }
        }
        
    }
}
