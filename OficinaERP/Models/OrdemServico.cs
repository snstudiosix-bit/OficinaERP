namespace OficinaERP.Models
{
    public class OrdemServico
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public int VeiculoId { get; set; }
        public string ClienteNome { get; set; } = "";
        public string VeiculoPlaca { get; set; } = "";
        public string DataAbertura { get; set; } = "";
        public string DefeitoInformado { get; set; } = "";
        public string Diagnostico { get; set; } = "";
        public string ServicosExecutados { get; set; } = "";
        public string PecasUtilizadas { get; set; } = "";
        public double ValorMaoDeObra { get; set; }
        public double ValorTotal { get; set; }
        public string Status { get; set; } = "Aberta";
        public string Observacoes { get; set; } = "";
    }
}