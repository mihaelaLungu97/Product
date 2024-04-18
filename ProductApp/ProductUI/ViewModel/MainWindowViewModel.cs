using System.Collections.ObjectModel;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;
using ProductUI.Model;
using ProductUI.ViewModel.Base;

namespace ProductUI.ViewModel
{
    class MainWindowViewModel : NotifyPropertyChanged
    {
        #region Fields

        private readonly ProductAPIClient _apiClient;

        #endregion

        #region Properties

        private ObservableCollection<Product> _productItems;
        public ObservableCollection<Product> ProductItems
        {
            get { return _productItems; }
            set
            {
                _productItems = value;
                RaisePropertyChanged(nameof(ProductItems));
            }
        }

        private int _selectedIndex;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                _selectedIndex = value;
                RaisePropertyChanged(nameof(SelectedIndex));
            }
        }

        private string _nameToBeAdded;
        public string NameToBeAdded
        {
            get { return _nameToBeAdded; }
            set
            {
                _nameToBeAdded = value;
                RaisePropertyChanged(nameof(NameToBeAdded));
            }
        }

        private string _barcodeToBeAdded;
        public string BarcodeToBeAdded
        {
            get { return _barcodeToBeAdded; }
            set
            {
                _barcodeToBeAdded = value;
                RaisePropertyChanged(nameof(BarcodeToBeAdded));
            }
        }

        private string _priceToBeAdded;
        public string PriceToBeAdded
        {
            get { return _priceToBeAdded; }
            set
            {
                _priceToBeAdded = value;
                RaisePropertyChanged(nameof(PriceToBeAdded));
            }
        }

        #endregion

        #region Commands

        private ICommand _readCommand;
        public ICommand ReadCommand
        {
            get
            {
                return _readCommand ??= new CommandHandler(ReadFromApiAsync, () => CanExecute);
            }
        }

        private ICommand _deleteCommand;
        public ICommand DeleteCommand
        {
            get
            {
                return _deleteCommand ??= new CommandHandler(DeleteFromListAsync, () => CanExecute);
            }
        }

        private ICommand _updateCommand;
        public ICommand UpdateCommand
        {
            get
            {
                return _updateCommand ??= new CommandHandler(UpdateItemInListAsync, () => CanExecute);
            }
        }

        private ICommand _createCommand;
        public ICommand CreateCommand
        {
            get
            {
                return _createCommand ??= new CommandHandler(CreateNewProduceItemAsync, () => CanExecute);
            }
        }

        public bool CanExecute => true;

        #endregion

        #region Constructor

        public MainWindowViewModel()
        {
            _apiClient = new ProductAPIClient("https://localhost:7168/api/Products");
        }

        #endregion

        #region Methods

        public async void CreateNewProduceItemAsync()
        {
            int idToBeAdded = 1;

            if (ProductItems.Count is not 0)
            {
                idToBeAdded = ProductItems.Last().Id + 1;
            }

            if (!PriceToBeAdded.All(char.IsDigit) || PriceToBeAdded.Count(c => c == '.') > 1)
            {
                MessageBox.Show("Please add only numbers for the price and only one dot.");
                return;
            }

            var newProduce = new Product
            {
                Id = idToBeAdded,
                Name = NameToBeAdded,
                BarCode = BarcodeToBeAdded,
                Price= PriceToBeAdded
            };

            try
            {
                bool isCreated = await _apiClient.PostProduceItemAsync(newProduce);

                if (isCreated)
                {
                    MessageBox.Show("Item created successfully. Press read to show it");
                }
                else
                {
                    MessageBox.Show("Failed to create the item.");
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Error creating item: {ex.Message}");
            }
        }

        private async void ReadFromApiAsync()
        {
            var productItems = await _apiClient.GetProductItemsAsync();
            ProductItems = [];
            if (productItems != null)
            {
                foreach (var item in productItems)
                {
                    // Check if the item with the same Id already exists
                    if (!ProductItems.Any(existingItem => existingItem.Id == item.Id))
                    {
                        ProductItems.Add(item);
                    }
                }
            }
        }

        private async void UpdateItemInListAsync()
        {
            if (ProductItems is null ||
                ProductItems[SelectedIndex] is null ||
                SelectedIndex < 0)
            {
                MessageBox.Show("Please select an item to update.");
                return;
            }

            var selectedItem = ProductItems[SelectedIndex];

            try
            {
                bool isUpdated = await _apiClient.PutProduceItemAsync(selectedItem);

                if (isUpdated)
                {
                    MessageBox.Show("Item updated successfully.");
                }
                else
                {
                    MessageBox.Show("Failed to update item.");
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Error updating item: {ex.Message}");
            }
        }

        private async void DeleteFromListAsync()
        {
            if (ProductItems is null ||
                ProductItems[SelectedIndex] is null ||
                SelectedIndex < 0)
            {
                MessageBox.Show("Please select an item to remove.");
                return;
            }

            var selectedItem = ProductItems[SelectedIndex];

            var itemIdToDelete = (int)selectedItem.Id;

            try
            {
                bool isDeleted = await _apiClient.DeleteProduceItemAsync(itemIdToDelete);

                if (isDeleted)
                {
                    ProductItems.Remove(selectedItem);
                    MessageBox.Show("Item removed successfully.");
                }
                else
                {
                    MessageBox.Show("Failed to remove item.");
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Error removing item: {ex.Message}");
            }
        }

        #endregion
    }
}
