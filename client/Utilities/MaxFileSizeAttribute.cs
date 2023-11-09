using System.ComponentModel.DataAnnotations;

namespace client.Utilities
{
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxFileSize;

        public MaxFileSizeAttribute(int maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }

        /// <summary>Checks if the size of a file is within the maximum allowed
        /// limit.</summary>
        /// <param name="value">The value to be validated. In this case, it is expected
        /// to be an object, but it can be null.</param>
        /// <param name="ValidationContext">Is an object that provides contextual information about the validation operation.</param>
        /// <returns>ValidationResult.</returns>
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile file && (file.Length > (_maxFileSize * 2048 * 2048)))
            {
                return new ValidationResult($"El tamaño de archivo máximo permitido es {_maxFileSize} MB.");
            }

            return ValidationResult.Success;
        }
    }
}