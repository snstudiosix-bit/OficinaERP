namespace OficinaERP.Models
{
    public class Peca
    {
        public int Id { get; set; }
        public string Nome { get; set; } = "";
        public string Codigo { get; set; } = "";
        public string Descricao { get; set; } = "";
        public double QuantidadeEstoque { get; set; }
        public double EstoqueMinimo { get; set; }
        public double ValorCusto { get; set; }
        public double ValorVenda { get; set; }
    }
}