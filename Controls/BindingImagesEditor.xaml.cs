using GrapeCity.Forguncy.CellTypes;
using GrapeCity.Forguncy.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CarouselCellType
{
    /// <summary>
    /// Interaction logic for BindingImagesEditor.xaml
    /// </summary>
    public partial class BindingImagesEditor : MyUserControl
    {
        IBuilderContext _context;
        public BindingImagesEditor(IBuilderContext context)
        {
            InitializeComponent();

            _context = context;
            this.DataContext = new BindingImagesEditorViewModel(context);
        }

        public BindingImagesEditorViewModel ViewModel
        {
            get
            {
                return this.DataContext as BindingImagesEditorViewModel;
            }
        }

        public override bool Validate()
        {
            return ViewModel.Validate();
        }

        private void EditQueryConditionHyperlink_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null)
            {
                return;
            }
            var window = this._context?.GetQueryConditionWindow(ViewModel.QueryCondition, ViewModel.TableName);
            if (window == null)
            {
                return;
            }

            window.Closed += (s, e2) =>
            {
                if (window.DialogResult == true)
                {
                    ViewModel.QueryCondition = window.QueryCondition;
                }
                this._context.ShowParentDialog(this);
            };

            this._context.HideParentDialog(this);
            window.ShowDialog();
        }
    }

    public class BindingImagesEditorViewModel : GrapeCity.Forguncy.CellTypes.CellTypeCommon.PropertyChangedObjectBase
    {
        private IBuilderContext _context;
        public BindingImagesEditorViewModel(IBuilderContext context)
        {
            _context = context;
        }

        public BindingImageInfo Model
        {
            get
            {
                return new BindingImageInfo()
                {
                    TableName = this.TableName,
                    CaptionColumn = this.CaptionColumn,
                    DescriptionColumn = this.DescriptionColumn,
                    ImageColumn = this.ImageColumn,
                    QueryCondition = this.QueryCondition
                };
            }
            set
            {
                if (value != null)
                {
                    this.TableName = value.TableName;
                    this.CaptionColumn = value.CaptionColumn;
                    this.DescriptionColumn = value.DescriptionColumn;
                    this.ImageColumn = value.ImageColumn;
                    this.QueryCondition = value.QueryCondition;
                }
            }
        }

        public List<string> TableList
        {
            get
            {
                return _context.EnumAllTableInfos().Select(t => { return t.TableName; })?.ToList() ?? new List<string>();
            }
        }

        public List<string> CaptionOrDescriptionColumnList
        {
            get
            {
                if (string.IsNullOrEmpty(TableName))
                {
                    return new List<string>() { "" };
                }
                
                var result = _context.EnumAllTableInfos().FirstOrDefault(t => t.TableName == TableName)?.Columns?.Where(c => 
                c.ColumnKind != TableColumnKind.StatisticsColumn && 
                c.ColumnType != ForguncyTableColumnType.Image && c.ColumnType != ForguncyTableColumnType.Attachment)?.Select(c => c.ColumnName)?.ToList() ?? new List<string>();

                result.Insert(0, "");

                return result;
            }
        }

        public List<string> ImageColumnList
        {
            get
            {
                if (string.IsNullOrEmpty(TableName))
                {
                    return new List<string>();
                }

                return _context.EnumAllTableInfos().FirstOrDefault(t => t.TableName == TableName)?.Columns?.Where(c =>
                c.ColumnType == ForguncyTableColumnType.Image)?.Select(c => c.ColumnName)?.ToList() ?? new List<string>();
            }
        }


        private string _tableName;
        public string TableName
        {
            get
            {
                return _tableName;
            }
            set
            {
                if (_tableName != value)
                {
                    _tableName = value;
                    this.OnPropertyChanged();
                    this.OnPropertyChanged("CaptionOrDescriptionColumnList");
                    this.OnPropertyChanged("ImageColumnList");
                }
            }
        }

        private string _captionColumn;
        public string CaptionColumn
        {
            get
            {
                return _captionColumn;
            }
            set
            {
                if (_captionColumn != value)
                {
                    _captionColumn = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _descriptionColumn;
        public string DescriptionColumn
        {
            get
            {
                return _descriptionColumn;
            }
            set
            {
                if (_descriptionColumn != value)
                {
                    _descriptionColumn = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _imageColumn;
        public string ImageColumn
        {
            get
            {
                return _imageColumn;
            }
            set
            {
                if (_imageColumn != value)
                {
                    _imageColumn = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public object QueryCondition
        {
            get; set;
        }

        internal bool Validate()
        {
            if (string.IsNullOrEmpty(TableName))
            {
                MessageBox.Show(Resource.BindingImagesEditor_TableNameCantBeEmpty);
                return false;
            }

            if (string.IsNullOrEmpty(ImageColumn))
            {
                MessageBox.Show(Resource.BindingImagesEditor_ImageColumnCantBeEmpty);
                return false;
            }

            return true;
        }
    }

}
