namespace OficinaERP.Models
{
    public class ComboItem
    {
        public string Id { get; set; } = "";
        public string Nome { get; set; } = "";

        public override string ToString()
        {
            return Nome;
        }
    }
}