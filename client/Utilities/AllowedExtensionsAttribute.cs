using System.ComponentModel.DataAnnotations;

namespace client.Utilities
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;

        public AllowedExtensionsAttribute(string[] extensions)
        {
            _extensions = extensions;
        }

        /// <summary>Validates if the extension of an uploaded file is allowed.</summary>
        /// <param name="value">Is an object that is expected to be an IFormFile.</param>
        /// <param name="ValidationContext">Is an object that provides contextual
        /// information about the validation operation.</param>
        /// <returns>ValidationResult object.</returns>
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName);

                if (!_extensions.Contains(extension.ToLower())) return new ValidationResult("La extensión de la imagen no está permitida!");
            }

            return ValidationResult.Success;
        }
    }
}