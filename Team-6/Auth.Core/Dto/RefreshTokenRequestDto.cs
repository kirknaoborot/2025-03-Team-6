using System.ComponentModel.DataAnnotations;

namespace Auth.Core.Dto
{
    public class RefreshTokenRequestDto : IValidatableObject
    {
        /// <summary>
        /// Токен доступа
        /// </summary>
        public string AccessToken { get; set; }
        
        /// <summary>
        /// Токен обновления
        /// </summary>
        public string RefreshToken { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(RefreshToken))
            {
                yield return new ValidationResult("RefreshToken is required");
            }

            if (string.IsNullOrWhiteSpace(AccessToken))
            {
                yield return new ValidationResult("AccessToken is required");
            }
        }
    }
}
