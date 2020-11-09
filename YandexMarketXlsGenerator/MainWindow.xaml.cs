using Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using YandexMarketFileGenerator;
using YandexMarketFileGenerator.Templates;

namespace YandexMarketXlsGenerator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadTemplatesToComboBox();
            UpdateLoadedProductsCountLabel();
        }

        private void LoadTemplatesToComboBox()
        {
            var templates = YandexGenerator.GetAvailabledTemplateList();

            cmbxTemplateType.DisplayMemberPath = "Description";
            cmbxTemplateType.SelectedValuePath = "Type";
            cmbxTemplateType.ItemsSource = templates;
        }

        private void btnGetExportData_Click(object sender, RoutedEventArgs e)
        {
            IYandexDirectTemplate template = null;
            try
            {
                List<OpenCartProductLine> products = LoadProductList(false);
                template = GetSelectedTemplate();

                YandexGenerator generator = new YandexGenerator(template);

                int startIndex = int.Parse(txtStartGroupIndex.Text);
                var data = generator.BuildExportInformation(products, startIndex);

                Clipboard.SetText(data);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Ошибка\r\n" + ex.Message + "\r\n\r\n" +ex.StackTrace, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {

            }
        }

        private void Template_WarningMessageRecieved(object sender, string message)
        {
            MessageBox.Show(message, "Информация", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        private IYandexDirectTemplate GetSelectedTemplate()
        {
            Type selectedTemplateType = (Type)cmbxTemplateType.SelectedValue;
            IYandexDirectTemplate selectedTemplate = (IYandexDirectTemplate)Activator.CreateInstance(selectedTemplateType, null);
            return selectedTemplate;
        }

        private List<OpenCartProductLine> LoadProductList(bool includeWordsWhiteList)
        {
            try
            {
                var lines = txtInput.Text
                    .Split(new string[] { Environment.NewLine }, StringSplitOptions.None)
                    .ToList();

                if(lines.Count() <= 1)
                {
                    throw new FormatException("Список товаров пуст");
                }
                var readedProducts = lines
                    .Skip(1)
                    .Select(line => OpenCartProductLine.Parse(line))
                    .ToList();

                List<OpenCartProductLine> validProducts = readedProducts;

                return validProducts;
            }
            catch
            {
                throw new FormatException("Ошибка форматирования входящих данных");
            }
        }

        private void txtInput_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdateLoadedProductsCountLabel();
            Properties.Settings.Default.Save();
        }

        private void UpdateLoadedProductsCountLabel()
        {
            int loadedProductsCount = 0;
            try
            {
                var lines = txtInput.Text
                    .Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

                if (lines.Count <= 1)
                {
                    throw new FormatException("Список товаров пуст");
                }

                loadedProductsCount = lines.Count;

            }
            catch
            {
                loadedProductsCount = 0;
            }

            if (IsLoaded)
            {
                txtLoadedProductsCount.Text = $"Загружено товаров: {loadedProductsCount}";
            }

        }
    }


}
