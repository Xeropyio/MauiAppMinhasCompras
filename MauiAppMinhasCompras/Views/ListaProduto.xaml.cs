using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MauiAppMinhasCompras.Views;

public partial class ListaProduto : ContentPage
{
	ObservableCollection<Produto> lista = new ObservableCollection<Produto>();

	public ListaProduto()
	{
		InitializeComponent();

		lst_produtos.ItemsSource = lista;
	}

    protected async override void OnAppearing()
    {
		try
		{
            lista.Clear();

            List<Produto>  tmp = await App.Db.GetAll();

		    tmp.ForEach(i => lista.Add(i));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private void ToolbarItem_Clicked(object sender, EventArgs e)
    {
		try
		{
			Navigation.PushAsync(new Views.NovoProduto());

		} catch (Exception ex)
		{
			DisplayAlert("Ops", ex.Message, "OK");
		}
    }

    private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            string q = e.NewTextValue;

            lst_produtos.IsRefreshing = true;

            lista.Clear();

            // Obtém a lista completa de produtos do banco de dados
            List<Produto> todosProdutos = await App.Db.GetAll();

            // Filtra a lista com base no texto de busca
            List<Produto> tmp = todosProdutos
                .Where(p =>
                    p.Descricao.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                    p.Categoria?.Contains(q, StringComparison.OrdinalIgnoreCase) == true)
                .ToList();

            tmp.ForEach(i => lista.Add(i));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
        finally
        {
            lst_produtos.IsRefreshing = false;
        }
    }

    private void ToolbarItem_Clicked_1(object sender, EventArgs e)
    {
        // 1. Calcule o total geral dos produtos
        double somaTotal = lista.Sum(i => i.Total);

        // 2. Agrupe os produtos por categoria e some os gastos de cada grupo
        var gastosPorCategoria = lista
            .GroupBy(p => p.Categoria)
            .Select(g => new
            {
                // Usa o nome da categoria ou "Sem Categoria" se for nulo/vazio
                Categoria = string.IsNullOrEmpty(g.Key) ? "Sem Categoria" : g.Key,
                Total = g.Sum(p => p.Total)
            })
            .OrderByDescending(g => g.Total);

        // 3. Monte a mensagem do relatório
        string msg = $"Total geral: {somaTotal:C}\n\n";
        msg += "Gastos por Categoria:\n";

        // Adicione cada categoria e seu total à mensagem
        foreach (var categoria in gastosPorCategoria)
        {
            msg += $"- {categoria.Categoria}: {categoria.Total:C}\n";
        }

        // 4. Exiba o relatório
        DisplayAlert("Relatório de Compras", msg, "OK");
    }

    private async void MenuItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            MenuItem selecionado = sender as MenuItem;

            Produto p = selecionado.BindingContext as Produto;

            bool confirm = await DisplayAlert(
                "Tem Certeza?", $"Remover {p.Descricao}?", "Sim", "Não");
            if (confirm)
            {
                await App.Db.Delete(p.Id);
                lista.Remove(p);
            }
        }
        catch (Exception ex)
		{
			await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private void lst_produtos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        try
        {
            Produto p = e.SelectedItem as Produto;

            Navigation.PushAsync(new Views.EditarProduto
            {
                BindingContext = p,
            });
        }
        catch (Exception ex)
        {
            DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async void lst_produtos_Refreshing(object sender, EventArgs e)
    {
        try
        {
            lista.Clear();

            List<Produto> tmp = await App.Db.GetAll();

            tmp.ForEach(i => lista.Add(i));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        } 
        finally 
        {
            lst_produtos.IsRefreshing = false;
        }
    }
}