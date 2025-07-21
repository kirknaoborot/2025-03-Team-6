namespace Auth.Core.Dto
{
    public class LoginRequestDto
    {
     
        /// <summary>
        /// Логин пользователя
        /// </summary>
        public string Login { get; set; }
        
        /// <summary>
        /// Пароль
        /// </summary>
        public string Password { get; set; }
    }
}
