namespace EstanciasCore.Areas.Administracion.ViewModels
{
    public class UserDTViewModel
    {
        public string Id { get; set; }
        public string Usuario { get; set; }
        public string NroDocumento { get; set; }
        public string Nombre { get; set; }
        public string Empresa { get; set; }
        public bool Administrador { get; set; }
        public string AdministradorTexto { get; set; }

    }
}
