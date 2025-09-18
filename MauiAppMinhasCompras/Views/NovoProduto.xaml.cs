using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel; // Adicione esta linha

namespace MauiAppMinhasCompras.Views;

public partial class NovoProduto : ContentPage
{
    // Crie a lista de categorias que será exibida no Picker
    public ObservableCollection<string> Categorias { get; set; } = new ObservableCollection<string>
    {
        "Alimentos",
        "Higiene",
        "Limpeza",
        "Bebidas",
        "Outros"
    };

    public NovoProduto()
    {
        InitializeComponent();

        // Defina o BindingContext para que o Picker possa acessar a lista de categorias
        BindingContext = this;
    }

    private async void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            Produto p = new Produto
            {
                Descricao = txt_descricao.Text,
                Quantidade = Convert.ToDouble(txt_quantidade.Text),
                Preco = Convert.ToDouble(txt_preco.Text),
                // Pegue o valor selecionado no Picker
                Categoria = pkr_categoria.SelectedItem.ToString()
            };

            await App.Db.Insert(p);
            await DisplayAlert("Sucesso!", "Registro Inserido", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }
}