namespace AppService.Core.DTOs.Repeticiones
{
    public class AppOrdenProductoRepeticionFilterDto
    {
        public string IdCliente { get; set; }

    }

    public class UpdateProductoOrdenFilterDto
    {
        public long Orden { get; set; }
        public int IdProducto { get; set; }
        public string UsuarioConectado { get; set; }
    }

    public class PuedeModificarProductoOrdenFilterDto
    {
        public string UsuarioConectado { get; set; }
    }
}
