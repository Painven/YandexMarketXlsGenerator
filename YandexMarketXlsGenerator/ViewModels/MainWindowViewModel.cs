using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using YandexMarketFileGenerator;
using YandexMarketXlsGenerator.Infrastructure.Commands;
using static YandexMarketFileGenerator.YandexGenerator;

namespace YandexMarketXlsGenerator.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private IRawDataRepository repository;

        private int _startGroupIndex = 2;
        public int StartGroupIndex { get => _startGroupIndex; set => Set(ref _startGroupIndex, value); }

        private string _rawStringData = "";
        public string RawStringData
        {
            get => _rawStringData;
            set
            {
                if (Set(ref _rawStringData, value))
                {
                    repository.Save(_rawStringData);
                }
                RaisePropertyChanged(nameof(LoadedProductLines));
            }
        }

        public int LoadedProductLines
        {
            get
            {
                var linesCount = RawStringData.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Length;
                return Math.Max(0, linesCount - 1);
            }
        }

        private int _selectedIndex;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (Set(ref _selectedIndex, value))
                {
                    Properties.Settings.Default.LastSelectedIndex = _selectedIndex;
                    Properties.Settings.Default.Save();
                }
            }
        }

        private ObservableCollection<TemplateData> _templates;
        public ObservableCollection<TemplateData> Templates { get => _templates; set => Set(ref _templates, value); }

        public TemplateData SelectedTemplate => Templates[SelectedIndex];

        public ICommand GetExportDataCommand { get; }
        public ICommand GetExportSqlCommand { get; }

        public MainWindowViewModel(IRawDataRepository repository) : this()
        {
            this.repository = repository;
            RawStringData = repository.Load();
        }

        public MainWindowViewModel()
        {
            Templates = new ObservableCollection<TemplateData>();
            GetExportDataCommand = new RelayCommand(GetExportData);
            GetExportSqlCommand = new RelayCommand(GetExportSql);

            foreach (var template in YandexGenerator.GetAvailabledTemplateList())
            {
                Templates.Add(template);
            }

            SelectedIndex = Properties.Settings.Default.LastSelectedIndex;
        }

        private void GetExportSql(object obj)
        {
            string manufacturer = SelectedIndex >= 0 ? Templates[SelectedIndex]?.Description : "--------";
            var sql = $@"SELECT d.name, '' as typeFull, '' as typeShort, 'true' as uniquePhrase, model, sku, (SELECT keyword FROM oc_url_alias WHERE query = CONCAT('product_id=', p.product_id)) as url, p.price, '' as customField
            FROM oc_product p
            JOIN oc_product_description d ON p.product_id = d.product_id
            JOIN oc_manufacturer m ON p.manufacturer_id = m.manufacturer_id
            WHERE m.name = '{manufacturer}' AND p.status = 1 AND d.main_product = 0
            ORDER BY d.name";

            Clipboard.SetText(sql);
        }

        private void GetExportData(object obj)
        {
            try
            {
                var products = OpenCartProductLine.ParseList(RawStringData);

                YandexGenerator generator = new YandexGenerator(GetSelectedTemplate());

                var data = generator.BuildExportInformation(products, StartGroupIndex);

                Clipboard.SetText(data);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка\r\n" + ex.Message + "\r\n\r\n" + ex.StackTrace, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private IYandexDirectTemplate GetSelectedTemplate()
        {
            var selectedTemplateType = SelectedTemplate.Type;
            IYandexDirectTemplate selectedTemplate = (IYandexDirectTemplate)Activator.CreateInstance(selectedTemplateType, null);
            return selectedTemplate;
        }
    }
}
