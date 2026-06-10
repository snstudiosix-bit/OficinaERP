namespace OficinaERP.Models
{
    public class Financeiro
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = "";
        public string Descricao { get; set; } = "";
        public double Valor { get; set; }
        public string DataVencimento { get; set; } = "";
        public string DataPagamento { get; set; } = "";
        public string Status { get; set; } = "Pendente";
        public string Observacao { get; set; } = "";
    }
}