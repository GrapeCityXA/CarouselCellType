using GrapeCity.Forguncy.CellTypes;
using GrapeCity.Forguncy.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for ImagesEditor.xaml
    /// </summary>
    public partial class ImagesEditor : MyUserControl
    {
        IBuilderContext _builderContext;
        public ImagesEditor(IBuilderContext builderContext)
        {
            InitializeComponent();

            _builderContext = builderContext;
            this.DataContext = new ImagesEditorViewModel();
        }
        
        public ImagesEditorViewModel ViewModel
        {
            get
            {
                return this.DataContext as ImagesEditorViewModel;
            }
        }

        private void AddImageButton_Click(object sender, RoutedEventArgs e)
        {
            var newNode = new ImageItemViewModel()
            {
                Caption = GetNewNodeName()
            };

            var index = ViewModel.ImageInfos.Count - 1;
            if (ViewModel.SelectedImageInfo != null)
            {
                index = ViewModel.ImageInfos.IndexOf(ViewModel.SelectedImageInfo);
            }

            ViewModel.ImageInfos.Insert(index + 1, newNode);

            ViewModel.SelectedImageInfo = newNode;
        }

        private void DeleteImageButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedImageInfo == null)
            {
                return;
            }

            var index = ViewModel.ImageInfos.IndexOf(ViewModel.SelectedImageInfo);
            ViewModel.ImageInfos.RemoveAt(index);
            if (ViewModel.ImageInfos.Count == 0)
            {
                ViewModel.ImageInfos.Add(new ImageItemViewModel() { Caption = GetNewNodeName() });
            }

            if (index < ViewModel.ImageInfos.Count)
            {
                ViewModel.SelectedImageInfo = ViewModel.ImageInfos[index];
            }
            else
            {
                ViewModel.SelectedImageInfo = ViewModel.ImageInfos[ViewModel.ImageInfos.Count - 1];
            }
        }

        private void UpImageButton_Click(object sender, RoutedEventArgs e)
        {
            MoveImageInternal(-1);
        }

        private void DownImageButton_Click(object sender, RoutedEventArgs e)
        {
            MoveImageInternal(1);
        }

        private void MoveImageInternal(int offset)
        {
            var index = ViewModel.ImageInfos.IndexOf(ViewModel.SelectedImageInfo);
            if (index + offset < 0)
            {
                return;
            }

            if (index + offset > ViewModel.ImageInfos.Count - 1)
            {
                return;
            }

            ViewModel.ImageInfos.Move(index, index + offset);
        }

        private void SetCommandListHyperlink_Click(object sender, RoutedEventArgs e)
        {
            var window = _builderContext.GetCommandWindow(CommandScope.Cell);

            window.InitCommandEvent += () =>
            {
                return ViewModel.SelectedImageInfo.CommandList;
            };

            window.UpdateCommandEvent += (sender2, commandList) =>
            {
                ViewModel.SelectedImageInfo.CommandList = commandList;
            };

            window.Closed += (sender2, e2) =>
            {
                _builderContext.ShowParentDialog(this);
            };

            _builderContext.HideParentDialog(this);
            window.ShowDialog();
        }

        private void SelectImageHyperlink_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.OpenFileDialog())
            {
                dialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG;*.ICO;*.JPEG)|*.BMP;*.JPG;*.GIF;*.PNG;*.ICO;*.JPEG|All files (*.*)|*.*";
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        string folderPath = _builderContext.ForguncyUploadFilesFolderPath + "\\CarouselCellType\\Images\\";
                        if (!System.IO.Directory.Exists(folderPath))
                        {
                            System.IO.Directory.CreateDirectory(folderPath);
                        }

                        var fileName = System.IO.Path.GetFileName(dialog.FileName);
                        var filePath = folderPath + fileName;
                        System.IO.File.Copy(dialog.FileName, filePath, true);

                        ViewModel.SelectedImageInfo.ImagePath = fileName;
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message);
                    }
                }
            }
        }

        private void DeleteImageIcon_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectedImageInfo.ImagePath = null;
        }

        private string GetNewNodeName()
        {
            int index = 1;
            var defaultName = Resource.CarouselCellType_DefaultImageName;
            while (true)
            {
                if (ViewModel.ImageInfos.Select(item => item.Caption).FirstOrDefault(item => item == string.Format(defaultName, index)) == null)
                {
                    return string.Format(defaultName, index);
                }
                index++;
            }
        }
    }

    public class ImagesEditorViewModel : GrapeCity.Forguncy.CellTypes.CellTypeCommon.PropertyChangedObjectBase
    {
        private ObservableCollection<ImageItemViewModel> _imageInfos;
        public ObservableCollection<ImageItemViewModel> ImageInfos
        {
            get
            {
                if (_imageInfos == null)
                {
                    _imageInfos = new ObservableCollection<ImageItemViewModel>();
                }

                return _imageInfos;
            }
        }

        private ImageItemViewModel _selectedImageInfo;
        public ImageItemViewModel SelectedImageInfo
        {
            get
            {
                return _selectedImageInfo;
            }
            set
            {
                if (_selectedImageInfo != value)
                {
                    _selectedImageInfo = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public List<ImageInfo> Model
        {
            get
            {
                var result = new List<ImageInfo>();
                foreach (var itemViewModel in ImageInfos)
                {
                    var item = new ImageInfo()
                    {
                        Caption = itemViewModel.Caption,
                        Description = itemViewModel.Description,
                        ImagePath = itemViewModel.ImagePath,
                        CommandList = itemViewModel.CommandList
                    };

                    result.Add(item);
                }

                return result;
            }
            set
            {
                ImageInfos.Clear();
                if (value != null)
                {
                    foreach (var item in value)
                    {
                        var itemViewModel = new ImageItemViewModel()
                        {
                            Caption = item.Caption,
                            Description = item.Description,
                            ImagePath = item.ImagePath,
                            CommandList = item.CommandList
                        };

                        ImageInfos.Add(itemViewModel);
                    }
                }

                if (ImageInfos.Count <= 0)
                {
                    ImageInfos.Add(new ImageItemViewModel()
                    {
                        Caption = string.Format(Resource.CarouselCellType_DefaultImageName, 1)
                    });
                }

                SelectedImageInfo = ImageInfos[0];
            }
        }
    }

    public class ImageItemViewModel : GrapeCity.Forguncy.CellTypes.CellTypeCommon.PropertyChangedObjectBase
    {
        public List<Command> CommandList
        {
            get; set;
        }

        private string _caption;
        public string Caption
        {
            get
            {
                return _caption;
            }
            set
            {
                if (_caption != value)
                {
                    _caption = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _description;
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _imagePath;
        public string ImagePath
        {
            get
            {
                return _imagePath;
            }
            set
            {
                if (_imagePath != value)
                {
                    _imagePath = value;
                    this.OnPropertyChanged();
                    this.OnPropertyChanged("DeleteImageIconVisibility");
                }
            }
        }

        public Visibility DeleteImageIconVisibility
        {
            get
            {
                return string.IsNullOrEmpty(ImagePath) ? Visibility.Collapsed : Visibility.Visible;
            }
        }
    }
}
