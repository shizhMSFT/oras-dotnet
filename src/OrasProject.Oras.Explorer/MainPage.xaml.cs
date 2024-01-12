using OrasProject.Oras.Content;
using OrasProject.Oras.Registry;
using System.Text;

namespace OrasProject.Oras.Explorer
{
    public partial class MainPage : ContentPage
    {
        private IRegistry _registry;
        private IRepository _repository;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void referenceEntry_Completed(object sender, EventArgs e)
        {
            var entry = (Entry)this.FindByName("referenceEntry");
            _registry = new Registry.Remote.Registry(entry.Text);

            var picker = (Picker)this.FindByName("repositoryPicker");
            var repositories = await _registry.ListRepositoriesAsync().ToListAsync();
            repositories.Sort();
            picker.ItemsSource = repositories;
        }

        private async void repositoryPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var repositoryPicker = (Picker)sender;
            _repository = await _registry.GetRepositoryAsync((string)repositoryPicker.SelectedItem);

            var tagPicker = (Picker)this.FindByName("tagPicker");
            var tags = await _repository.ListTagsAsync().ToListAsync();
            tags.Sort();
            tagPicker.ItemsSource = tags;
        }

        private async void tagPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var output = (Label)this.FindByName("outputLabel");
            byte[] content;
            var picker = (Picker)sender;
            var manifest = await _repository.FetchAsync((string)picker.SelectedItem);
            using (manifest.Stream)
            {
                content = await manifest.Stream.ReadAllAsync(manifest.Descriptor);
            }
            output.Text = Encoding.Default.GetString(content);
        }
    }
}
