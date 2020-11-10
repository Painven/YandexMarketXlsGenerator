﻿using System;
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
                if(Set(ref _rawStringData, value))
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

        private TemplateData _selectedTemplate;
        public TemplateData SelectedTemplate { get => _selectedTemplate; set => Set(ref _selectedTemplate, value); }

        private ObservableCollection<TemplateData> _templates;
        public ObservableCollection<TemplateData> Templates { get => _templates; set => Set(ref _templates, value); }

        public ICommand GetExportDataCommand { get; }

        public MainWindowViewModel(IRawDataRepository repository) : this()
        {
            this.repository = repository;
            RawStringData = repository.Load();
        }

        public MainWindowViewModel()
        {
            Templates = new ObservableCollection<TemplateData>();
            GetExportDataCommand = new RelayCommand(GetExportData);

            foreach (var template in YandexGenerator.GetAvailabledTemplateList())
            {
                Templates.Add(template);
            }

            SelectedTemplate = Templates.First();
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