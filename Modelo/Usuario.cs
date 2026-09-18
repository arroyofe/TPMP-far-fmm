using System.Text.RegularExpressions;

namespace Nucleo.Modelo
{   /// <summary>
    /// Clase queepresenta a un usuario del sistema.
    /// </summary>
    public class Usuario
    {
        /// <summary>
        /// Obtiene o establece el identificador único del usuario.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario.
        /// </summary>
        ///[Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; }

        /// <summary>
        /// Obtiene o establece los apellidos del usuario.
        /// </summary>
        /// [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        public string Apellidos { get; set; }

        /// <summary>
        /// Obtiene o establece la dirección de correo electrónico del usuario.
        /// </summary>
        /// [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        /// [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        public string CorreoElectronico { get; set; }

        /// <summary>
        /// Se almacena un hash de la contraseña.
        /// </summary>
        /// [Required(ErrorMessage = "La contraseña es obligatoria.")]
        /// [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        /// [MaxLength(16, ErrorMessage = "La contraseña no puede tener más de 16 caracteres.")]
        /// [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,16}$", 
        /// ErrorMessage = "La contraseña debe contener al menos una letra mayúscula, una letra minúscula, un número y un carácter especial.")]
        public string ContrasenaHash { get; private set; }

        /// <summary>
        /// Obtiene un valor que indica si el usuario está activo.
        /// </summary>
        public bool Activo { get; private set; }

        /// <summary>
        /// Obtiene un valor que indica si el usuario tiene credenciales configuradas.
        /// </summary>
        public bool TieneCredencialesConfiguradas { get; private set; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Usuario"/>.
        /// Constructor protegido para permitir la creación de instancias a través de Entity Framework u otros mecanismos de ORM.
        /// </summary>
        protected Usuario()
        {
            Nombre = string.Empty;
            Apellidos = string.Empty;
            CorreoElectronico = string.Empty;
            ContrasenaHash = string.Empty;

        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Usuario"/>.
        /// </summary>
        /// <param name="id">
        /// Identificador único del usuario.
        /// </param>
        /// <param name="nombre">
        /// Nombre del usuario.
        /// </param>
        /// <param name="apellidos">
        /// Apellidos del usuario.
        /// </param>
        /// <param name="correoElectronico">
        /// Dirección de correo electrónico del usuario.
        /// </param>
        /// <param name="contrasenaHash">
        /// Hash de la contraseña del usuario.
        /// </param>
        /// <param name="activo">
        /// Indica si el usuario está activo.
        /// </param>
        /// <param name="tieneCredencialesConfiguradas">
        /// Indica si el usuario tiene credenciales configuradas.
        /// </param>
        public Usuario(
            int id,
            string nombre,
            string apellidos,
            string correoElectronico,
            string contrasenaHash,
            bool activo = true,
            bool tieneCredencialesConfiguradas = false)
        {
            Id = id;
            Nombre = ValidarTexto(nombre, nameof(nombre));
            Apellidos = ValidarTexto(apellidos, nameof(apellidos));
            CorreoElectronico = ValidarTexto(correoElectronico, nameof(correoElectronico));
            ContrasenaHash = contrasenaHash;
            Activo = activo;
            TieneCredencialesConfiguradas = tieneCredencialesConfiguradas;
        }

        /// <summary>
        /// Valida que un texto no sea nulo, vacío o contenga solo espacios en blanco.
        /// Combinado con el parámetro required, asegura que el valor no sea nulo ni vacío.
        /// </summary>
        /// <param name="valor"></param>
        /// <param name="nombreParametro"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private static string ValidarTexto(
        string valor,
        string nombreParametro)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                throw new ArgumentException(
                    "El valor no puede estar vacío.",
                    nombreParametro);
            }

            return valor.Trim();
        }

        /// <summary>
        /// Activa el usuario, estableciendo la propiedad <see cref="Activo"/> en <c>true</c>.
        /// </summary>
        public void Activar()
        {
            Activo = true;
        }

        /// <summary>
        /// Desactiva el usuario, estableciendo la propiedad <see cref="Activo"/> en <c>false</c>.
        /// </summary>
        public void Desactivar()
        {
            Activo = false;
        }

        /// <summary>
        /// Configura las credenciales del usuario, estableciendo el hash de la contraseña.
        /// </summary>
        /// <param name="contrasenaHash"></param>
        /// <exception cref="ArgumentException"></exception>
        public void ConfigurarCredenciales(string contrasenaHash)
        {
            if (string.IsNullOrWhiteSpace(contrasenaHash))
            {
                throw new ArgumentException(
                    "La contraseña no puede estar vacía.",
                    nameof(contrasenaHash));
            }

            ContrasenaHash = contrasenaHash;
            TieneCredencialesConfiguradas = true;
        }

        /// <summary>
        /// Comprueba si una contraseña cumple con los requisitos de seguridad y tiene
        /// el formato mínimo esperado.
        /// </summary>
        /// <param name="contrasena">
        /// Contraseña que se desea validar.
        /// </param>
        /// <returns>
        /// <c>true</c> si la contraseña tiene un formato válido;
        /// en caso contrario, <c>false</c>.
        /// </returns>
        public static bool EsContrasenaValida(string contrasena)
        {
            if (string.IsNullOrEmpty(contrasena))
            {
                return false;
            }

            const int longitudMinima = 8;
            const int longitudMaxima = 16;

            bool longitudValida =
                contrasena.Length >= longitudMinima &&
                contrasena.Length <= longitudMaxima;

            bool contieneMayuscula =
                contrasena.Any(char.IsUpper);

            bool contieneMinuscula =
                contrasena.Any(char.IsLower);

            bool contieneNumero =
                contrasena.Any(char.IsDigit);

            bool contieneCaracterEspecial =
                contrasena.Any(caracter =>
                    !char.IsLetterOrDigit(caracter) &&
                    !char.IsWhiteSpace(caracter));

            bool contieneEspacios =
                contrasena.Any(char.IsWhiteSpace);

            return longitudValida &&
                   contieneMayuscula &&
                   contieneMinuscula &&
                   contieneNumero &&
                   contieneCaracterEspecial &&
                   !contieneEspacios;
        }

        /// <summary>
        /// Comprueba si una dirección de correo electrónico tiene
        /// el formato esperado.
        /// </summary>
        /// <param name="correo">
        /// Dirección de correo que se desea validar.
        /// </param>
        /// <returns>
        /// <c>true</c> si el correo tiene un formato válido;
        /// en caso contrario, <c>false</c>.
        /// </returns>
        public static bool EsCorreoElectronicoValido(string correo)
        {
            if (string.IsNullOrWhiteSpace(correo))
            {
                return false;
            }

            string patron = @"^[\p{L}\d_]+(?:[.-][\p{L}\d_]+)*@[\p{L}][\p{L}\d]*\.[\p{L}]{2}$";

            return Regex.IsMatch(
                correo.Trim(),
                patron,
                RegexOptions.CultureInvariant);
        }

    }
}