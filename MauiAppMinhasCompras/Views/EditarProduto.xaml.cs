using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel; // Adicione esta linha

namespace MauiAppMinhasCompras.Views;

public partial class EditarProduto : ContentPage
{
    // Crie a mesma lista de categorias
    public ObservableCollection<string> Categorias { get; set; } = new ObservableCollection<string>
    {
        "Alimentos",
        "Higiene",
        "Limpeza",
        "Bebidas",
        "Outros"
    };

    public EditarProduto()
    {
        InitializeComponent();

        // Adicione um BindingContext local para o Picker
        pkr_categoria.ItemsSource = Categorias;
    }

    private async void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            Produto produto_anexado = BindingContext as Produto;

            // Atualiza as propriedades do objeto que já está anexado
            produto_anexado.Descricao = txt_descricao.Text;
            produto_anexado.Quantidade = Convert.ToDouble(txt_quantidade.Text);
            produto_anexado.Preco = Convert.ToDouble(txt_preco.Text);

            // **CORREÇÃO:** Verifique se um item foi selecionado no Picker
            if (pkr_categoria.SelectedItem is string categoriaSelecionada)
            {
                produto_anexado.Categoria = categoriaSelecionada;
            }
            else
            {
                // Se nenhum item foi selecionado, defina a categoria como vazia
                produto_anexado.Categoria = string.Empty;
            }

            // Salva as alterações no banco de dados
            await App.Db.Update(produto_anexado);
            await DisplayAlert("Sucesso!", "Registro Atualizado", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    // Sobrescreva o evento OnAppearing para selecionar o item correto no Picker
    protected override void OnAppearing()
    {
        base.OnAppearing();
        Produto produto = BindingContext as Produto;
        if (produto != null && !string.IsNullOrEmpty(produto.Categoria))
        {
            int selectedIndex = Categorias.IndexOf(produto.Categoria);
            if (selectedIndex != -1)
            {
                pkr_categoria.SelectedIndex = selectedIndex;
            }
        }
    }
}